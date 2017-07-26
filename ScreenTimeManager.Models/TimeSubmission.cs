using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeManager.Models
{
	// Not saved to DB. Exists for validation
	public class TimeSubmission
	{
		[Required]
		public int RuleBaseId { get; set; }

		[Required]
		[Range(0, 23, ErrorMessage = "The value must be between 0 and 23 hours")]
		public int Hours { get; set; }

		[Required]
		[Range(0, 59, ErrorMessage = "The value must be between 0 and 59 minutes")]
		public int Minutes { get; set; }

	}
}
