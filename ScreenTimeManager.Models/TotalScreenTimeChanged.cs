using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScreenTimeManager.Models.Interfaces;

namespace ScreenTimeManager.Models
{
	public class TotalScreenTimeChanged : IDateTimeCreated
	{
		[Key]
		[ScaffoldColumn(false)]
		public int Id { get; set; }

		[Required]
		[UIHint("Long")]
		public long SecondsAdded { get; set; }

		// The rule used for this entry into the history
		// At least one, but not more than one
		[Required]
		[ScaffoldColumn(false)]
		public int RuleUsedId { get; set; }

		[ForeignKey("RuleUsedId")]
		public virtual RuleBase Rule { get; set; }

		[DataType(DataType.MultilineText)]
		public string RequestNote { get; set; }

		[DataType(DataType.MultilineText)]
		public string SubmissionNote { get; set; }

		public string ApprovedBy { get; set; }

		public string RequestedBy { get; set; }

		[ScaffoldColumn(false)]
		public int TimeHistoryDateId { get; set; }

		[DisplayName("Date Added")]
		[ForeignKey("TimeHistoryDateId")]
		public virtual TimeHistoryDate TimeHistoryDate { get; set; }

		//[Required]
		//[DisplayName("Added")]
		//[UIHint("TimeSpan")]
		//[Column(TypeName = "Time")]
		//public TimeSpan RecordAddedTime { get; set; }

		[Required]
		[DisplayName("Added")]
		[DisplayFormat(DataFormatString = "{0:T}")]
		public DateTime RecordAddedDateTime { get; set; }

		[NotMapped]
		[ScaffoldColumn(false)]
		public bool IsOverrideDateTimeCreated { get; set; }

		// Not sure about this...
		// This could easily complicate things such as calculating the time on the timer
		// For time that is denied, it needs to be excluded from the calculation
		// Fairly well compartmentalized code shouldn't make this too difficult
		// Yet, I can't shake the feeling that this is just lazy
		// At, the same time, there needs to be a way to keep track of denied requests
		// Hmm...
		[Required]
		[ScaffoldColumn(false)]
		public bool IsDenied { get; set; }

		[Required]
		[ScaffoldColumn(false)]
		public bool IsFinalized { get; set; }
	}
}