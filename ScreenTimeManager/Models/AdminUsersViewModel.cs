using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ScreenTimeManager.Models
{
	public class AdminUsersViewModel
	{
		private IQueryable<ApplicationUser> _users;
		private IQueryable<IdentityRole> _roles;

		public AdminUsersViewModel() { }

		public AdminUsersViewModel(IQueryable<ApplicationUser> users, IQueryable<IdentityRole> roles)
		{
			Roles = roles;
			Users = users;
		}

		public IQueryable<ApplicationUser> Users { get;}
		public IQueryable<IdentityRole> Roles { get; }
	}
}