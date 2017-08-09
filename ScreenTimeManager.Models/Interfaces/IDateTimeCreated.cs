﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTimeManager.Models.Interfaces
{
	public interface IDateTimeCreated
	{
		int TimeHistoryDateId { get; set; }

		[DisplayName("Date Added")]
		[ForeignKey("TimeHistoryDateId")]
		TimeHistoryDate TimeHistoryDate { get; set; }

		[Required]
		[DisplayName("Added")]
		[DisplayFormat(DataFormatString = "{0:h\\:mm\\:ss}")]
		[Column(TypeName = "Time")]
		TimeSpan RecordAddedTime { get; set; }
	}
}