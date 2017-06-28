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

		public override int SaveChanges()
		{
			foreach (var timeChange in ChangeTracker.Entries<TotalScreenTimeChanged>()
				.Where(m => m.State.Equals(EntityState.Added))
				.Select(e => e.Entity))
			{
				timeChange.RecordAddedDateTime = DateTime.Now;
			}

			return base.SaveChanges();
		}
	}
}
