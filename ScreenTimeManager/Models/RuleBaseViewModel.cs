using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScreenTimeManager.Models
{
	public class RuleBaseViewModel
	{
		public IEnumerable<RuleBase> Rules { get; set; }
		public IEnumerable<TotalScreenTimeChangedRequest> Requests { get; set; }
	}
}