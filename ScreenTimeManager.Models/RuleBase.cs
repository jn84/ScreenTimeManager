using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeManager.Models
{
    public class RuleBase
    {
	    public TimeSpan TimeEarned { get; set; }

		// A ratio (or fraction) object
		// Tuple<numerator, denominator>
		public Tuple<int, int> TimeEarningRatio { get; set; } 
	}

	public class RuleFixed
	{
		
	}

	public class RuleVariable
	{

		// The time applied to the rule
		// This value is multiplied by TimeEarningRatio
		public TimeSpan TimeApplied { get; set; }
		
	}
}
