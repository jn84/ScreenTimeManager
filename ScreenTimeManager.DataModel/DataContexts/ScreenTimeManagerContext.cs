using System;
using System.Data.Entity;
using System.Linq;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Interfaces;

namespace ScreenTimeManager.DataModel.DataContexts
{
	public class ScreenTimeManagerContext : DbContext
	{
		public ScreenTimeManagerContext() : base("name=DefaultConnection") { }

		public DbSet<TotalScreenTimeChanged> TimeChanged { get; set; }
		public DbSet<RuleBase> Rules { get; set; }
		public DbSet<TimeHistoryDate> HistoryDates { get; set; }
		public DbSet<TotalScreenTimeChangedRequest> TimeRequests { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Properties<DateTime>()
				.Configure(c => c.HasColumnType("datetime2"));

			base.OnModelCreating(modelBuilder);
		}

		// Can we fit all the date handling code here?
		public override int SaveChanges()
		{
			// This should be interface based

			foreach (IDateTimeCreated entry in ChangeTracker.Entries<IDateTimeCreated>()
				.Where(m => m.State.Equals(EntityState.Added))
				.Select(e => e.Entity))
			{
				// In some circumstances (seeding specific data for debugging), we don't
				// want the date/time to be set to NOW.
				// this is a separate routine for readability and explanation
				if (entry.IsOverrideDateTimeCreated)
					continue;

				// If today's date is not in the table, we need to add it
				TimeHistoryDate date = HistoryDates.FirstOrDefault(hd => hd.EntriesDate == DateTime.Today);
				if (date == null)
				{
					using (var ctx = new ScreenTimeManagerContext())
					{
						int newSum = 0;

						// Get the last (usually yesterday) TimeHistoryDate
						var historyDate =
							ctx.HistoryDates.OrderByDescending(hd => hd.EntriesDate).FirstOrDefault();

						date = ctx.HistoryDates.Create();

						// Since there was a previous day's entry, sum everything from that day
						// and place the result as the beginning sum for today's date.
						if (historyDate != null)
						{
							// What if the timer is running when we get here?
							// Stop it, then immediately start it? Should work.
							// But we don't (can't) reference the startup project here.
							// This is quite the conundrum.
							// What a mess I've made.
							// Maybe the timer should automatically stop/restart just before/after midnight.
							// Doing anything not date/time related here is a BIG NO NO

							newSum =
								historyDate.StartOfDayTotalSeconds
								+ (int)historyDate.EntriesForThisDate
								.Where(e => !e.IsDenied && e.IsFinalized) // ensure we don't add denied entires!
								.Sum(e => e.SecondsAdded);
						}

						date.StartOfDayTotalSeconds = newSum;
						date.EntriesDate = DateTime.Today;
						ctx.HistoryDates.Add(date);
						ctx.SaveChanges();
					}
				}

				HistoryDates.Attach(date);

				entry.TimeHistoryDateId = date.Id;
				entry.TimeHistoryDate = date;

				entry.RecordAddedDateTime = DateTime.Now;
			}

			return base.SaveChanges();
		}
	}
}