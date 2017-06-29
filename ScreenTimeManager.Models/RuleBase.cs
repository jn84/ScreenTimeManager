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

	    public RuleType RuleType { get; set; }

	    public string RuleTitle { get; set; }
	    public string RuleDescription { get; set; }

	    public TimeSpan FixedTimeEarned { get; set; }
	    
	    public Tuple<int, int> TimeEarningRatio { get; set; }

	    public virtual ICollection<TotalScreenTimeChanged> UsedInChangeEntries { get; set; }
    }
}
