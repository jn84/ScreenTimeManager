using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScreenTimeManager.Models.Interfaces;

namespace ScreenTimeManager.Models
{
	// Model for Child user add/remove time requests
	public class TotalScreenTimeChangedRequest : IDateTimeCreated
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
		public string ApprovalNote { get; set; }

		public string RequestedBy { get; set; }

		[ScaffoldColumn(false)]
		public int TimeHistoryDateId { get; set; }

		[DisplayName("Date Added")]
		[ForeignKey("TimeHistoryDateId")]
		public virtual TimeHistoryDate TimeHistoryDate { get; set; }

		[Required]
		[DisplayName("Added")]
		[UIHint("TimeSpan")]
		[Column(TypeName = "Time")]
		public TimeSpan RecordAddedTime { get; set; }

		[Required]
		[DisplayName("Added")]
		public DateTime RecordAddedDateTime { get; set; }

		[NotMapped]
		[ScaffoldColumn(false)]
		public bool IsOverrideDateTimeCreated { get; set; }

		[UIHint("BooleanNullable")]
		[DisplayName("Approve")]
		public bool? IsApproved { get; set; }
	}
}