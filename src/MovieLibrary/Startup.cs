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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

			services.AddSingleton<IApplicationInitializer, RolesInitializer>();

			services.AddUserManagement();
		}

		public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApplicationInitializer appInitializer)
		{
			appInitializer.Initialize(CancellationToken.None).Wait();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStatusCodePages();

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");

				// Razor pages are required for Microsoft.AspNetCore.Identity.UI
				endpoints.MapRazorPages();

				// Disabling users registration built into Identity package.
				endpoints.MapGet("/Identity/Account/Register", RedirectToLoginPage);
				endpoints.MapPost("/Identity/Account/Register", RedirectToLoginPage);
			});
		}

		private static Task RedirectToLoginPage(HttpContext context)
		{
			context.Response.Redirect("/Identity/Account/Login", true);
			return Task.CompletedTask;
		}
	}
}
