using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity.Owin;
using ScreenTimeManager.Models;

namespace ScreenTimeManager.Controllers
{
    public class AdminController : Controller
    {

	    private ApplicationUserManager _userManager;
	    private ApplicationRoleManager _roleManager;

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
			foreach (var user in UserManager.Users)
				Debug.WriteLine(user.UserName);
			foreach (var role in RoleManager.Roles)
				Debug.WriteLine(role.Name);


			return View(new AdminUsersViewModel(UserManager.Users, RoleManager.Roles));
        }

	    public ActionResult CreateUser()
	    {
		    return View();
	    }

	    public ActionResult CreateRole()
	    {
		    return View();
	    }

	    public ActionResult EditUserRoles(string id)
	    {
		    return View();
	    }

	    public ActionResult DeleteUser(string id)
		{
			return View();
		}
    }
}