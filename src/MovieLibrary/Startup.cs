using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieLibrary.Authorization;
using MovieLibrary.Dal.MongoDB;
using MovieLibrary.Internal;
using MovieLibrary.Logic.Extensions;
using MovieLibrary.UserManagement;

namespace MovieLibrary
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<AppSettings>(settings => Configuration.Bind(settings));

			services.AddControllersWithViews(config =>
			{
				var policy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();

				config.Filters.Add(new AuthorizeFilter(policy));
			});

			// Razor pages are required for Microsoft.AspNetCore.Identity.UI
			services.AddRazorPages();

			services.AddBusinessLogic();

			var connectionString = Configuration.GetConnectionString("movieLibraryDatabase");
			if (String.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException("Connection string 'movieLibraryDatabase' is not set");
			}

			services.AddMongoDbDal(connectionString);

			services.AddIdentityMongoDbProvider<MongoUser>(mongoIdentityOptions => mongoIdentityOptions.ConnectionString = connectionString)
				.AddDefaultUI();

			services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
			services.AddScoped<IAuthorizationHandler, DefaultAdminAuthorizationHandler>();
			services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

			services.AddSingleton<IApplicationBootstrapper, ApplicationBootstrapper>();
			services.AddScoped<IApplicationInitializer, UsersInitializer>();
			services.AddScoped<ICompositeApplicationInitializer, CompositeApplicationInitializer>();

			services.AddUserManagement();

			services.AddHsts(options =>
			{
				options.MaxAge = TimeSpan.FromDays(365);
			});
		}

		public static void Configure(IApplicationBuilder app, IApplicationBootstrapper appBootstrapper, IWebHostEnvironment env, ICompositeApplicationInitializer appInitializer)
		{
			appInitializer.Initialize(CancellationToken.None).Wait();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");

				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStatusCodePages();

			app.UseStaticFiles();

			app.UseRouting();

			appBootstrapper.AddAuthenticationMiddleware(app);

			app.UseAuthorization();

			app.UseEndpoints(ConfigureRoutes);
		}

		private static void ConfigureRoutes(IEndpointRouteBuilder endpoints)
		{
			endpoints.MapControllerRoute(
				name: "MoviesToGet",
				pattern: "MoviesToGet",
				defaults: new { controller = "MoviesToGet", action = "Index", pageNumber = 1, });

			endpoints.MapControllerRoute(
				name: "MoviesToGetWithPage",
				pattern: "MoviesToGet/page-{pageNumber:int}",
				defaults: new { controller = "MoviesToGet", action = "Index", });

			endpoints.MapControllerRoute(
				name: "MoviesToSee",
				pattern: "MoviesToSee",
				defaults: new { controller = "MoviesToSee", action = "Index", pageNumber = 1, });

			endpoints.MapControllerRoute(
				name: "MoviesToSeeWithPage",
				pattern: "MoviesToSee/page-{pageNumber:int}",
				defaults: new { controller = "MoviesToSee", action = "Index", });

			endpoints.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			// Razor pages are required for Microsoft.AspNetCore.Identity.UI
			endpoints.MapRazorPages();

			// Disabling users registration built into Identity package.
			endpoints.MapGet("/Identity/Account/Register", RedirectToLoginPage);
			endpoints.MapPost("/Identity/Account/Register", RedirectToLoginPage);
		}

		private static Task RedirectToLoginPage(HttpContext context)
		{
			context.Response.Redirect("/Identity/Account/Login", true);
			return Task.CompletedTask;
		}
	}
}
