using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScreenTimeManager.Models
{
	public class DeleteUserViewModel
	{
		public string Username { get; set; }
		public string UserId { get; set; }

		[DisplayName("Check this box to confirm deletion of user")]
		public bool IsConfirmed { get; set; }
	}
}