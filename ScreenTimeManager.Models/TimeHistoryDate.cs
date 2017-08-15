using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScreenTimeManager.Models
{
	public class TimeHistoryDate
	{
		[Key]
		public int Id { get; set; }

		// Should always be date, at midnight
		[Required]
		[DisplayName("Date Added")]
		[DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
		[Column(TypeName = "Date")]
		public DateTime EntriesDate { get; set; }

		//[ScaffoldColumn(false)]
		//public int StartOfDayTotalSeconds { get; set; }

		public virtual ICollection<TotalScreenTimeChanged> EntriesForThisDate { get; set; }
	}
}