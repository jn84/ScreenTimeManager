using ScreenTimeManager.Models.Enums;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScreenTimeManager.Models
{

	// User RuleBase combined with input to create a TotalScreenTimeChanged entry
	// These rules will be listed in the rule use page
	public class RuleBase
    {
		[Key]
	    public int Id { get; set; }

	    [Required]
	    public bool IsExpired { get; set; }

		[Required]
		public bool IsHidden { get; set; }

		[Required]
	    public RuleType RuleType { get; set; }
		[Required]
	    public RuleModifier RuleModifier { get; set; }

		[Required]
	    public string RuleTitle { get; set; }
		[Required]
	    public string RuleDescription { get; set; }

		[Required]
	    public TimeSpan FixedTimeEarned { get; set; }

	    [Required]
	    public int VariableRatioNumerator { get; set; }
		[Required]
	    public int VariableRatioDenominator { get; set; }

		public virtual ICollection<TotalScreenTimeChanged> UsedInChangeEntries { get; set; }
    }
}
