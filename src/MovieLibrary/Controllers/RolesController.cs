using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Authorization;
using MovieLibrary.Internal;
using MovieLibrary.UserManagement.Interfaces;
using MovieLibrary.UserManagement.ViewModels.Roles;

namespace MovieLibrary.Controllers
{
	[Authorize(Roles = SecurityConstants.AdministratorRole)]
	public class RolesController : Controller
	{
		private const string TempDataAddedRole = "AddedRole";
		private const string TempDataUpdatedRole = "UpdatedRole";
		private const string TempDataDeletedRole = "DeletedRole";

		private readonly IRoleService roleService;

		public RolesController(IRoleService roleService)
		{
			this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
		}

		[HttpGet]
		public async Task<ViewResult> Index(CancellationToken cancellationToken)
		{
			var roles = await roleService.GetAllRoles(cancellationToken).ToListAsync(cancellationToken);

			var viewModel = new RolesListViewModel(roles)
			{
				AddedRole = TempData.GetBooleanValue(TempDataAddedRole),
				UpdatedRole = TempData.GetBooleanValue(TempDataUpdatedRole),
				DeletedRole = TempData.GetBooleanValue(TempDataDeletedRole),
			};

			return View(viewModel);
		}

		[HttpGet]
		public ViewResult CreateRole()
		{
			var viewModel = new NewRoleViewModel
			{
				Permissions = ApplicationPermissions.All.Select(p => new RolePermissionViewModel
				{
					PermissionName = p,
					Assigned = false,
				}).ToList(),
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> CreateRole([FromForm] NewRoleViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return View("CreateRole", model);
			}

			var roleId = await roleService.CreateRole(model.Name, cancellationToken);

			var permissions = model.Permissions
				.Where(p => p.Assigned)
				.Select(p => p.PermissionName);

			await roleService.AssignRolePermissions(roleId, permissions, cancellationToken);

			TempData[TempDataAddedRole] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		public Task<ViewResult> EditRole([FromRoute] string id, CancellationToken cancellationToken)
		{
			return EditRoleView(id, cancellationToken);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateRole([FromForm] RoleDetailsViewModel model, CancellationToken cancellationToken)
		{
			if (!ModelState.IsValid)
			{
				return await EditRoleView(model.RoleId, cancellationToken);
			}

			var assignedPermissions = model.Permissions
				.Where(p => p.Assigned)
				.Select(p => p.PermissionName);

			await roleService.AssignRolePermissions(model.RoleId, assignedPermissions, cancellationToken);

			TempData[TempDataUpdatedRole] = true;

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<ViewResult> ConfirmRoleDeletion([FromRoute] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));

			var role = await roleService.GetRole(id, cancellationToken);
			var viewModel = new RoleViewModel(role);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<RedirectToActionResult> DeleteRole([FromForm] string id, CancellationToken cancellationToken)
		{
			_ = id ?? throw new ArgumentNullException(nameof(id));

			await roleService.DeleteRole(id, cancellationToken);

			TempData[TempDataDeletedRole] = true;

			return RedirectToAction("Index");
		}

		private async Task<ViewResult> EditRoleView(string roleId, CancellationToken cancellationToken)
		{
			_ = roleId ?? throw new ArgumentNullException(nameof(roleId));

			var roleModel = await roleService.GetRole(roleId, cancellationToken);
			var rolePermissions = await roleService.GetRolePermissions(roleId, cancellationToken);
			var allPermissions = ApplicationPermissions.All.ToList();

			var viewModel = new RoleDetailsViewModel(roleModel, rolePermissions, allPermissions);

			return View("EditRole", viewModel);
		}
	}
}
