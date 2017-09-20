using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScreenTimeManager.Models
{
	// Can't be struct. SelectList doesn't like that.
	public class UserRole
	{
		public string RoleName { get; set; }
		public string RoleId { get; set; }
	}

	public class EditRolesViewModel
	{
		public string Username { get; set; }
		public string UserId { get; set; }
		public List<string> AllRoles { get; set; }
		public List<string> UserRoles { get; set; }

		public EditRolesViewModel() { }

		public EditRolesViewModel(string username, string userId, List<string> allRoles, List<string> userRoles)
		{
			Username = username;
			UserId = userId;
			AllRoles = allRoles;
			UserRoles = userRoles;
		}
	}
}