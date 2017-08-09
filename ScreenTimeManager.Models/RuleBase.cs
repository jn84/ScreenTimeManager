using ScreenTimeManager.Models.Enums;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
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
		[DisplayName("Type")]
	    public RuleType RuleType { get; set; }
		[Required(ErrorMessage = "The rule must add or subtract time")]
	    public RuleModifier RuleModifier { get; set; }

		[DisplayName("Title")]
		[Required(ErrorMessage = "The rule must have a title")]
		[MaxLength(63, ErrorMessage = "The title must be less than 63 characters")]
	    public string RuleTitle { get; set; }

		[DisplayName("Description")]
		[Required(ErrorMessage = "The rule must have a description")]
		[DataType(DataType.MultilineText)]
	    public string RuleDescription { get; set; }


		// I don't like the minimum being zero, but it keeps giving me trouble otherwise
	    [Required]
		[DisplayName("Time Applied")]
	    [Range(typeof(TimeSpan), "00:00:00", "23:59:59", 
			ErrorMessage = "Must be more than 0 and less than one day")]
	    public TimeSpan FixedTimeEarned { get; set; } = TimeSpan.Parse("00:00:01");

	    [Required(ErrorMessage = "Both ratio values must be populated")]
	    [Range(1, 60, ErrorMessage = "Ratio values must be between 1 and 60")]
	    public int VariableRatioNumerator { get; set; } = 1;

		[Required(ErrorMessage = "Both ratio values must be populated")]
		[Range(1, 60, ErrorMessage = "Ratio values must be between 1 and 60")]
	    public int VariableRatioDenominator { get; set; } = 1;

		public virtual ICollection<TotalScreenTimeChanged> UsedInChangeEntries { get; set; }

	    public static RuleBase Create()
	    {
			return new RuleBase()
			{
				RuleTitle = "",
				RuleDescription = "",
				RuleModifier = RuleModifier.Add,
				VariableRatioNumerator = 1,
				VariableRatioDenominator = 1,
				FixedTimeEarned = TimeSpan.FromTicks(0),
			};
		}
    }
}
