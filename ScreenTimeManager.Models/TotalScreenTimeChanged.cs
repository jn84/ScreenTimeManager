using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeManager.Models
{
	public class TotalScreenTimeChanged
	{
		public int Id { get; set; }

		[Required]
		public long SecondsAdded { get; set; }

		[Required]
		public DateTime RecordAddedDateTime { get; set; }

		// The rule used for this entry into the history
		// At least one, but not more than one
		[Required]
		public int RuleUsedId { get; set; }


		public string SubmissionNote { get; set; }
	}
}
