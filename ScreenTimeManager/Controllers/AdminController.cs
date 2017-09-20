using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using ScreenTimeManager.Models;

namespace ScreenTimeManager.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
    {

	    private ApplicationUserManager _userManager;
	    private ApplicationRoleManager _roleManager;

		// Required -- This is why the properties will populate the values if not done by the other constructor
		public AdminController() { }

	    public AdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
	    {
		    UserManager = userManager;
		    RoleManager = roleManager;
	    }

	    public ApplicationRoleManager RoleManager
	    {
		    get => _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
		    private set => _roleManager = value;
	    }

	    public ApplicationUserManager UserManager
	    {
		    get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
		    private set => _userManager = value;
	    }

		// GET: Admin
		public ActionResult Index()
		{
			return View(new AdminUsersViewModel(UserManager.Users, RoleManager.Roles));
        }

		// GET
		public ActionResult EditRoles(string id)
		{
			string username = "";
				
			username = UserManager.Users.Single(u => u.Id == id).UserName;

			List<string> allRoles = RoleManager.Roles.Select(r => r.Name).ToList();

			IEnumerable<string> userRolesEnumerable = 
				UserManager.Users
				.Single(u => u.Id == id).Roles
				.Select(r => r.RoleId)
				.AsEnumerable();

			List<string> userRoles = new List<string>();

			foreach (var role in userRolesEnumerable)
			{
				userRoles.Add(RoleManager.Roles.Single(r => r.Id == role).Name);
			}

			var vm = new EditRolesViewModel(username, id, allRoles, userRoles);

			Debug.WriteLine(userRoles.Count);

			return PartialView("_EditRolesModal", vm);
		}

		// POST
		[HttpPost]
		[ValidateAntiForgeryToken]
	    public ActionResult EditRoles([Bind(Include = "Username, UserId, UserRoles")] EditRolesViewModel roleData)
		{
			List<string> allRoles = RoleManager.Roles.Select(r => r.Name).ToList();

			foreach (var role in roleData.UserRoles)
				if (!RoleManager.RoleExists(role))
					ModelState.AddModelError("UserRoles", @"No such roll: " + role);

			// How to ensure UserId wasn't forged?
			// TODO: Ensure UserId wasn't forged.

			if (ModelState.IsValid)
			{
				foreach (var role in roleData.UserRoles)
					if (!UserManager.IsInRole(roleData.UserId, role))
						UserManager.AddToRole(roleData.UserId, role);


				foreach (var role in allRoles.Except(roleData.UserRoles))
					if (UserManager.IsInRole(roleData.UserId, role))
						UserManager.RemoveFromRole(roleData.UserId, role);

				return Json(new {success = ModelState.IsValid, redirectUrl = Url.Action("Index")});
			}

			roleData.AllRoles = allRoles;

			return PartialView("_EditRolesModal", roleData);
		}

		public ActionResult DeleteUser(string id)
		{
			return PartialView(
				"_DeleteUserModal", 
				new DeleteUserViewModel { UserId = id, Username = UserManager.FindById(id).UserName });
		}

		[HttpPost]
		[ActionName("DeleteUser")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteUserConfirmed([Bind(Include = "UserId, IsConfirmed")] DeleteUserViewModel deleteUserViewModel)
		{
			if (deleteUserViewModel.IsConfirmed)
				UserManager.Delete(UserManager.FindById(deleteUserViewModel.UserId));

			return Json(new { success = ModelState.IsValid, redirectUrl = Url.Action("Index") });
		}
	}
}