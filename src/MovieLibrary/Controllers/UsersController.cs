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
using MovieLibrary.UserManagement.ViewModels;

namespace MovieLibrary.Controllers
{
	[Authorize(Roles = SecurityConstants.AdministratorRole)]
	public class UsersController : Controller
	{
		private const string TempDataAddedUser = "AddedUser";
		private const string TempDataUpdatedUser = "UpdatedUser";
		private const string TempDataDeletedUser = "DeletedUser";

		private readonly IUserService userService;

		public UsersController(IUserService userService)
		{
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

		[HttpGet]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var users = await userService.GetAllUsers(cancellationToken).ToListAsync(cancellationToken);

			var viewModel = new UserListViewModel(users)
			{
				AddedUser = TempData.GetBooleanValue(TempDataAddedUser),
				UpdatedUser = TempData.GetBooleanValue(TempDataUpdatedUser),
				DeletedUser = TempData.GetBooleanValue(TempDataDeletedUser),
			};

			return View(viewModel);
		}

		[HttpGet]
		public IActionResult RegisterUser()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> RegisterUser(NewUserViewModel model, CancellationToken cancellationToken)
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
		public async Task<IActionResult> EditUser(string id, CancellationToken cancellationToken)
		{
			return await EditUserView(id, cancellationToken);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateUser([FromForm] UserDetailsViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await EditUserView(model.UserId, cancellationToken);
			}

			// TBD: Implement update of user permissions
			TempData[TempDataUpdatedUser] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> ConfirmUserDeletion(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));

			var user = await userService.GetUser(id, cancellationToken);
			var viewModel = new UserViewModel(user);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));

			await userService.DeleteUser(id, cancellationToken);

			TempData[TempDataDeletedUser] = true;

			return RedirectToAction("Index");
		}

		private async Task<IActionResult> EditUserView(string userId, CancellationToken cancellationToken)
		{
			_ = userId ?? throw new ArgumentNullException(nameof(userId));

			var userDetails = await userService.GetUser(userId, cancellationToken);
			var viewModel = new UserDetailsViewModel(userDetails);

			return View("EditUser", viewModel);
		}
	}
}
