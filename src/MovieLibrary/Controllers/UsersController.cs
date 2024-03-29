using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Authorization;
using MovieLibrary.Internal;
using MovieLibrary.UserManagement.Interfaces;
using MovieLibrary.UserManagement.Models;
using MovieLibrary.UserManagement.ViewModels.Users;

namespace MovieLibrary.Controllers
{
	[Authorize(Roles = SecurityConstants.AdministratorRole)]
	public class UsersController : Controller
	{
		private const string TempDataAddedUser = "AddedUser";
		private const string TempDataUpdatedUser = "UpdatedUser";
		private const string TempDataDeletedUser = "DeletedUser";

		private readonly IUserService userService;

		private readonly IRoleService roleService;

		public UsersController(IUserService userService, IRoleService roleService)
		{
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
		}

		[HttpGet]
		public async Task<ViewResult> Index(CancellationToken cancellationToken)
		{
			var users = await userService.GetAllUsers(cancellationToken).ToListAsync(cancellationToken);

			var viewModel = new UsersListViewModel(users)
			{
				AddedUser = TempData.GetBooleanValue(TempDataAddedUser),
				UpdatedUser = TempData.GetBooleanValue(TempDataUpdatedUser),
				DeletedUser = TempData.GetBooleanValue(TempDataDeletedUser),
			};

			return View(viewModel);
		}

		[HttpGet]
		public ViewResult RegisterUser()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> RegisterUser([FromForm] NewUserViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return View("RegisterUser");
			}

			var newUser = new NewUserModel
			{
				Email = model.Email,
				Password = model.Password,
			};

			await userService.CreateUser(newUser, cancellationToken);

			TempData[TempDataAddedUser] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		public Task<ViewResult> EditUser([FromRoute] string id, CancellationToken cancellationToken)
		{
			return EditUserView(id, cancellationToken);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateUser([FromForm] UserDetailsViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await EditUserView(model.UserId, cancellationToken);
			}

			var assignedRoles = model.Roles
				.Where(p => p.Assigned)
				.Select(p => p.RoleName);

			await userService.AssignUserRoles(model.UserId, assignedRoles, cancellationToken);

			TempData[TempDataUpdatedUser] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<ViewResult> ConfirmUserDeletion([FromRoute] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));

			var user = await userService.GetUser(id, cancellationToken);
			var viewModel = new UserViewModel(user);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<RedirectToActionResult> DeleteUser([FromForm] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));

			await userService.DeleteUser(id, cancellationToken);

			TempData[TempDataDeletedUser] = true;

			return RedirectToAction("Index");
		}

		private async Task<ViewResult> EditUserView(string userId, CancellationToken cancellationToken)
		{
			_ = userId ?? throw new ArgumentNullException(nameof(userId));

			var userModel = await userService.GetUser(userId, cancellationToken);
			var userRoles = await userService.GetUserRoles(userModel.Id, cancellationToken);
			var allRoles = await roleService.GetAllRoles(cancellationToken)
				.Select(role => role.RoleName)
				.ToListAsync(cancellationToken);

			var viewModel = new UserDetailsViewModel(userModel, userRoles, allRoles);

			return View("EditUser", viewModel);
		}
	}
}
