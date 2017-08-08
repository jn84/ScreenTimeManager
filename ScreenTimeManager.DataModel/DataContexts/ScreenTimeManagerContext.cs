using System;
using System.Data.Entity;
using System.Linq;
using ScreenTimeManager.Models;

namespace ScreenTimeManager.DataModel.DataContexts
{
	public class ScreenTimeManagerContext : DbContext
	{
		public DbSet<TotalScreenTimeChanged> TimeChanged { get; set; }
		public DbSet<RuleBase> Rules { get; set; }
		public DbSet<TimeHistoryDate> HistoryDates { get; set; }


		public ScreenTimeManagerContext() : base("name=DefaultConnection")
		{
			
		}

		// Can we fit all the date handling code here?
		public override int SaveChanges()
		{
			foreach (var timeChange in ChangeTracker.Entries<TotalScreenTimeChanged>()
				.Where(m => m.State.Equals(EntityState.Added))
				.Select(e => e.Entity))
			{
				// If today's date is not in the table, we need to add it
				var date = HistoryDates.FirstOrDefault(hd => hd.EntriesDate == DateTime.Today);
				if (date == null)
				{
					using (var ctx = new ScreenTimeManagerContext())
					{
						// Spin up a new context so only the TimeHistoryDate will be added, 
						// and we can grab its newly created id for the TotalScreenTimeChanged entity
						date = ctx.HistoryDates.Create();
						date.EntriesDate = DateTime.Today;
						ctx.HistoryDates.Add(date);
						ctx.SaveChanges();
					}
				}

				HistoryDates.Attach(date);

				timeChange.TimeHistoryDateId = date.Id;
				timeChange.TimeHistoryDate = date;

				timeChange.RecordAddedTime = DateTime.Now.TimeOfDay;
			}

			return base.SaveChanges();
		}
	}
}
