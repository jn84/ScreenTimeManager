using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.DataModel.Migrations;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;

namespace ScreenTimeManager.DataModel.Initializers
{
#if DEBUG

	public class ScreenTimeManagerInitializer : DropCreateDatabaseAlways<ScreenTimeManagerContext>
	{
		protected override void Seed(ScreenTimeManagerContext context)
		{
			base.Seed(context);

			IList<RuleBase> ruleList = new List<RuleBase>();

			// Keep this rule in production
			ruleList.Add(new RuleBase
			{
				RuleType = RuleType.Timer,
				RuleModifier = RuleModifier.Subtract,
				RuleTitle = "Timer Used",
				RuleDescription = "Used the timer to deduct time",
				VariableRatioNumerator = 1,
				VariableRatioDenominator = 1,
				IsHidden = true,
			});

			// Keep this rule in production
			ruleList.Add(new RuleBase
			{
				RuleType = RuleType.Fixed,
				RuleModifier = RuleModifier.Add,
				RuleTitle = "Cleaned Room",
				RuleDescription = "Earn 30 minutes for cleaning room",
				FixedTimeEarned = new TimeSpan(0, 0, 30, 0)
			});

			ruleList.Add(new RuleBase
			{
				RuleType = RuleType.Fixed,
				RuleModifier = RuleModifier.Add,
				RuleTitle = "Changed Litter",
				RuleDescription = "Earn 20 minutes for cleaning/changing all the litter boxes",
				FixedTimeEarned = new TimeSpan(0, 0, 20, 0)
			});

			ruleList.Add(new RuleBase
			{
				RuleType = RuleType.Variable,
				RuleModifier = RuleModifier.Add,
				RuleTitle = "Played Outside",
				RuleDescription = "For every hour of time spent playing outside, earn 15 minutes",
				VariableRatioNumerator = 1,
				VariableRatioDenominator = 4
			});

			ruleList.Add(new RuleBase
			{
				RuleType = RuleType.Variable,
				RuleModifier = RuleModifier.Subtract,
				RuleTitle = "Stole Time",
				RuleDescription = "For every 1 minute of time stolen, subtract 1.5 minutes",
				VariableRatioNumerator = 3,
				VariableRatioDenominator = 2
			});

			//foreach (var rule in ruleList)
			context.Rules.AddRange(ruleList);

			context.SaveChanges();

			IList<TotalScreenTimeChanged> timeChangeList = new List<TotalScreenTimeChanged>();

			List<RuleBase> allRules = context.Rules.ToList();
			int i = 0;

			timeChangeList.Add(new TotalScreenTimeChanged
			{
				RuleUsedId = allRules[i].Id,
				SecondsAdded = (long) new TimeSpan(0, 0, 30, 0).TotalSeconds,
				IsDenied = false,
				IsFinalized = true
			});

			if (i + 1 == allRules.Count)
				i = 0;
			else
				i++;

			timeChangeList.Add(new TotalScreenTimeChanged
			{
				RuleUsedId = allRules[i].Id,
				SecondsAdded = (long) new TimeSpan(0, 0, 20, 0).TotalSeconds,
				IsDenied = false,
				IsFinalized = true
			});
			if (i + 1 == allRules.Count)
				i = 0;
			else
				i++;

			timeChangeList.Add(new TotalScreenTimeChanged
			{
				RuleUsedId = allRules[i].Id,
				SecondsAdded = (long) new TimeSpan(0, 1, 13, 22).TotalSeconds,
				IsDenied = false,
				IsFinalized = true
			});
			if (i + 1 == allRules.Count)
				i = 0;
			else
				i++;

			timeChangeList.Add(new TotalScreenTimeChanged
			{
				RuleUsedId = allRules[i].Id,
				SecondsAdded = (long) new TimeSpan(0, 0, 30, 0).TotalSeconds,
				IsDenied = false,
				IsFinalized = true
			});
			if (i + 1 == allRules.Count)
				i = 0;
			else
				i++;

			timeChangeList.Add(new TotalScreenTimeChanged
			{
				RuleUsedId = allRules[i].Id,
				SecondsAdded = (long) new TimeSpan(0, 0, 20, 0).TotalSeconds,
				IsDenied = false,
				IsFinalized = true
			});
			if (i + 1 == allRules.Count)
				i = 0;
			else
				i++;

			timeChangeList.Add(new TotalScreenTimeChanged
			{
				RuleUsedId = allRules[i].Id,
				SecondsAdded = (long) new TimeSpan(0, 0, 31, 4).TotalSeconds,
				IsDenied = false,
				IsFinalized = true
			});
			if (i + 1 == allRules.Count)
				i = 0;
			else
				i++;

			context.TimeChanged.AddRange(timeChangeList);

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