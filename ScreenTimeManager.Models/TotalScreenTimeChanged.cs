using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeManager.Models
{
	public class TotalScreenTimeChanged
	{
		public int Id { get; set; }

		public long SecondsAdded { get; set; }

		public DateTime RecordAddedDateTime { get; set; }

		// The rule used for this entry into the history
		// At least one, but not more than one
		public int RuleUsedId { get; set; }
	}
}
