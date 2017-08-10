using System.ComponentModel.DataAnnotations;

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

		[DataType(DataType.MultilineText)]
		public string Note { get; set; }

		public static TimeSubmission Create(int ruleId)
		{
			return new TimeSubmission
			{
				Hours = 0,
				Minutes = 0,
				Note = "",
				RuleBaseId = ruleId
			};
		}
	}
}