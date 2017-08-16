using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.DataModel.Migrations;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;
using ScreenTimeManager.Models.Interfaces;

namespace ScreenTimeManager.DataModel.Initializers
{
#if DEBUG

	public class ScreenTimeManagerInitializer : DropCreateDatabaseAlways<ScreenTimeManagerContext>
	{
		protected override void Seed(ScreenTimeManagerContext context)
		{
			base.Seed(context);

			Assembly assembly = Assembly.GetExecutingAssembly();
			string resourceName = "ScreenTimeManager.DataModel.Initializers.SeedData.rulebases.csv";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
				{
					CsvReader csvReader = new CsvReader(reader);
					csvReader.Configuration.Delimiter = "|";
					csvReader.Configuration.WillThrowOnMissingField = false;
					csvReader.Configuration.UseNewObjectForNullReferenceProperties = false;
					var rules = csvReader.GetRecords<RuleBase>().ToArray();
					context.Rules.AddOrUpdate(r => r.Id, rules);
				}
			}

			resourceName = "ScreenTimeManager.DataModel.Initializers.SeedData.timehistorydates.csv";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
				{
					CsvReader csvReader = new CsvReader(reader);
					csvReader.Configuration.Delimiter = "|";
					csvReader.Configuration.WillThrowOnMissingField = false;
					csvReader.Configuration.UseNewObjectForNullReferenceProperties = false;
					var timeHistory = csvReader.GetRecords<TimeHistoryDate>().ToArray();
					context.HistoryDates.AddOrUpdate(h => h.Id, timeHistory);
				}
			}

			resourceName = "ScreenTimeManager.DataModel.Initializers.SeedData.totalscreentimechanged.csv";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
				{
					CsvReader csvReader = new CsvReader(reader);
					csvReader.Configuration.Delimiter = "|";
					csvReader.Configuration.WillThrowOnMissingField = false;
					csvReader.Configuration.UseNewObjectForNullReferenceProperties = false;
					var timeChange = csvReader.GetRecords<TotalScreenTimeChanged>().ToArray();
					foreach (TotalScreenTimeChanged totalScreenTimeChanged in timeChange)
					{
						totalScreenTimeChanged.Rule = null;
						totalScreenTimeChanged.TimeHistoryDate = null;
					}

					context.TimeChanged.AddOrUpdate(tc => tc.Id, timeChange);
				}
			}

			resourceName = "ScreenTimeManager.DataModel.Initializers.SeedData.totalscreentimechangedrequest.csv";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
				{
					CsvReader csvReader = new CsvReader(reader);
					csvReader.Configuration.Delimiter = "|";
					csvReader.Configuration.WillThrowOnMissingField = false;
					csvReader.Configuration.UseNewObjectForNullReferenceProperties = false;
					var timeChangeRequests = csvReader.GetRecords<TotalScreenTimeChangedRequest>().ToArray();
					foreach (TotalScreenTimeChangedRequest totalScreenTimeChanged in timeChangeRequests)
					{
						totalScreenTimeChanged.Rule = null;
						totalScreenTimeChanged.TimeHistoryDate = null;
					}
					context.TimeRequests.AddOrUpdate(tcr => tcr.Id, timeChangeRequests);
				}
			}

			// We don't want the framework to use the current date/time
			foreach (var entry in context.ChangeTracker.Entries<IDateTimeCreated>())
			{
				entry.Entity.IsOverrideDateTimeCreated = true;
			}

			context.SaveChanges();


		}
	}

#endif

#if RELEASE
	public class ScreenTimeManagerInitializer : System.Data.Entity.MigrateDatabaseToLatestVersion<ScreenTimeManagerContext, Configuration>
	{
		// https://galleryserverpro.com/using-entity-framework-code-first-migrations-to-auto-create-and-auto-update-an-application/
	}
	#endif
}