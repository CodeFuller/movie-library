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

			var connectionString = GetConnectionString();
			services.AddMongoDbDal(connectionString);
			services.AddIdentityMongoDbProvider<MongoUser>(mongoIdentityOptions => mongoIdentityOptions.ConnectionString = connectionString).AddDefaultUI();

			services.AddPermissionBasedAuthorization();
			services.AddUserManagement();

			services.AddSingleton<IApplicationBootstrapper, ApplicationBootstrapper>();
			services.AddScoped<ICompositeApplicationInitializer, CompositeApplicationInitializer>();

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
				app.UseExceptionHandler("/Errors/UnhandledException");
				app.UseStatusCodePagesWithReExecute("/Errors/ErrorCode", "?statusCode={0}");

				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			appBootstrapper.AddAuthenticationMiddleware(app);

			app.UseAuthorization();

			app.UseEndpoints(ConfigureRoutes);
		}

		private string GetConnectionString()
		{
			var connectionStringName = Configuration["connectionStringName"];
			if (String.IsNullOrEmpty(connectionStringName))
			{
				throw new InvalidOperationException("Connection string name is not set. Define it via setting 'connectionStringName'");
			}

			var connectionString = Configuration.GetConnectionString(connectionStringName);
			if (String.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException($"Connection string '{connectionStringName}' is not set. Define it via setting 'connectionStrings:{connectionStringName}'");
			}

			return connectionString;
		}

		private static void ConfigureRoutes(IEndpointRouteBuilder endpoints)
		{
			const string moviesControllerSegment = "{controller:regex(^MoviesToGet|MoviesToSee$)}";

			endpoints.MapControllerRoute(
				name: "MoviesFirstPage",
				pattern: moviesControllerSegment,
				defaults: new { action = "Index", pageNumber = 1, });

			endpoints.MapControllerRoute(
				name: "MoviesSpecificPage",
				pattern: $"{moviesControllerSegment}/page-{{pageNumber:int}}",
				defaults: new { action = "Index", });

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
