using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScreenTimeManager.Models.Interfaces
{
	public interface IDateTimeCreated
	{
		int TimeHistoryDateId { get; set; }

		[DisplayName("Date Added")]
		[ForeignKey("TimeHistoryDateId")]
		TimeHistoryDate TimeHistoryDate { get; set; }

		//[Required]
		//[DisplayName("Added")]
		//[DisplayFormat(DataFormatString = "{0:h\\:mm\\:ss}")]
		//[Column(TypeName = "Time")]
		//TimeSpan RecordAddedTime { get; set; }

		[Required]
		[DisplayName("Added")]
		DateTime RecordAddedDateTime { get; set; }

		/// <summary>
		/// If true, will skip setting date/time values in implementing class
		/// </summary>
		[NotMapped]
		[ScaffoldColumn(false)]
		bool IsOverrideDateTimeCreated { get; set; }
	}
}