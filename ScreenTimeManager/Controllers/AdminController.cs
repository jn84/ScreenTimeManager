﻿using System;
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

			// Methods are looking for role names

			Debug.WriteLine("-------------------------------------------");
			foreach (var role in roleData.UserRoles)
				Debug.WriteLine(role);
			Debug.WriteLine("-------------------------------------------");
			foreach (var role in allRoles.Except(roleData.UserRoles))
				Debug.WriteLine(role);
			Debug.WriteLine("-------------------------------------------");

			// this is a security hole. Someone could change the user id in form before submitting.

			var removeResult = UserManager.RemoveFromRoles(roleData.UserId, allRoles.ToArray());
			Debug.WriteLine(removeResult.Succeeded ? "Remove roles succeeded" : "Remove roles failed");
			foreach (var error in removeResult.Errors)
				Debug.WriteLine(error);

			var addResult = UserManager.AddToRoles(roleData.UserId, roleData.UserRoles.ToArray());
			Debug.WriteLine(addResult.Succeeded ? "Add roles succeeded" : "Add roles failed");
			foreach (var error in addResult.Errors)
				Debug.WriteLine(error);



			// Check if "user" is assigned to the correct roles.


			//UserManager.Update(user);



			return Json(new { success = ModelState.IsValid, redirectUrl = Url.Action("Index") });
		}

		public ActionResult DeleteUser(string id)
		{
			return View();
		}

		[HttpPost]
		[ActionName("DeleteUser")]
		public ActionResult DeleteUserConfirmed(string id)
	    {
		    return View();
	    }
	}
}