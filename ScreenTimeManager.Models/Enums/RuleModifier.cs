using System.ComponentModel.DataAnnotations;

namespace ScreenTimeManager.Models.Enums
{
	public enum RuleModifier
	{
		[Display(Name = "Add Time")] Add = 1,

		[Display(Name = "Remove Time")] Subtract = -1
	}
}