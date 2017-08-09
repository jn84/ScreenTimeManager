using System;

namespace ScreenTimeManager.Models.Interfaces
{
	internal interface IModificationHistory
	{
		DateTime DateModified { get; set; }
		DateTime DateCreated { get; set; }
		bool IsDirty { get; set; }
	}
}