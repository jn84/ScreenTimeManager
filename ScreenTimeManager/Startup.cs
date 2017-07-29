using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using ScreenTimeManager.Models;

[assembly: OwinStartup(typeof(ScreenTimeManager.Startup))]
namespace ScreenTimeManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			ConfigureAuth(app);
	        app.MapSignalR();

			CreateRolesandUsers();
		}

	    private void CreateRolesandUsers()
	    {

		    if (CreateRole("Admin"))
		    {
			    CreateNewUserInRole("TheDad", "jmnims@gmail.com", "default", "Admin");
		    }

		    if (CreateRole("Parent"))
		    {
			    CreateNewUserInRole("TheMom", "johnstone87@gmail.com", "default", "Parent");
		    }

		    if (CreateRole("Child"))
		    {
			    CreateNewUserInRole("TheKid", "anims06@gmail.com", "default", "Child");
		    }
	    }

	    private bool CreateRole(string roleName)
	    {
		    var ctx = new ApplicationDbContext();

		    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));

		    if (!roleManager.RoleExists(roleName))
			    if (roleManager.Create(new IdentityRole(roleName)) == IdentityResult.Failed())
				    return false;

		    return true;
	    }

	    private void CreateNewUserInRole(string username, string email, string password, string role)
	    {
		    ApplicationDbContext context = new ApplicationDbContext();

		    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

			var user = new ApplicationUser
		    {
			    UserName = username,
			    Email = email
		    };

		    var chkUser = userManager.Create(user, password);

		    //Add default User to Role Admin   
		    if (chkUser.Succeeded)
		    {
			    var result1 = userManager.AddToRole(user.Id, role);
		    }
		}
	}
}
