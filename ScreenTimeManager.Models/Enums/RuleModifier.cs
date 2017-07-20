using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeManager.Models.Enums
{
	public enum RuleModifier
	{
		[Display(Name = "Add Time")]
		Add = 1,

		[Display(Name = "Remove Time")]
		Subtract = -1
	}
}
