using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenTimeManager.Models.Interfaces;

namespace ScreenTimeManager.Models
{
	// Model for Child user add/remove time requests
	public class TotalScreenTimeChangedRequest : IDateTimeCreated
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public long SecondsAdded { get; set; }

		public int TimeHistoryDateId { get; set; }

		[DisplayName("Date Added")]
		[ForeignKey("TimeHistoryDateId")]
		public virtual TimeHistoryDate TimeHistoryDate { get; set; }

		[Required]
		[DisplayName("Added")]
		[DisplayFormat(DataFormatString = "{0:h\\:mm\\:ss}")]
		[Column(TypeName = "Time")]
		public TimeSpan RecordAddedTime { get; set; }

		// The rule used for this entry into the history
		// At least one, but not more than one
		[Required]
		public int RuleUsedId { get; set; }

		[ForeignKey("RuleUsedId")]
		public virtual RuleBase Rule { get; set; }

		public string SubmissionNote { get; set; }

		public string RequestedBy { get; set; }
	}
}
