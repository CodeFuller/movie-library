using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Internal;
using MovieLibrary.UserManagement;
using MovieLibrary.UserManagement.ViewModels;

namespace MovieLibrary.Controllers
{
	[Authorize(Roles = Roles.AdministratorRole)]
	public class UsersController : Controller
	{
		private readonly IUserService userService;

		public UsersController(IUserService userService)
		{
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

		[HttpGet]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var users = await userService.GetAllUsers(cancellationToken).ToListAsync(cancellationToken);

			var viewModel = new UserListViewModel(users);

			return View(viewModel);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id, CancellationToken cancellationToken)
		{
			return await UserDetailsView(id, cancellationToken);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateUser([FromForm] UserDetailsViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await UserDetailsView(model.UserId, cancellationToken);
			}

			var assignedPermissions = model.Permissions
				.Where(p => p.Assigned)
				.Select(p => p.PermissionName);

			await userService.AssignUserPermissions(model.UserId, assignedPermissions, cancellationToken);

			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult CancelUserUpdate()
		{
			return RedirectToAction("Index");
		}

		private async Task<IActionResult> UserDetailsView(string userId, CancellationToken cancellationToken)
		{
			_ = userId ?? throw new ArgumentNullException(nameof(userId));

			var userDetails = await userService.GetUser(userId, cancellationToken);

			var viewModel = new UserDetailsViewModel
			{
				UserId = userId,
				UserName = userDetails.UserName,
				Permissions = userDetails.AllPermissions
					.Select(p => new UserPermissionViewModel
					{
						PermissionName = p,
						Assigned = userDetails.UserPermissions.Contains(p),
					})
					.ToList(),
			};

			return View("Edit", viewModel);
		}
	}
}
