using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;

namespace ScreenTimeManager.DataModel.Migrations
{
	public sealed class Configuration : DbMigrationsConfiguration<ScreenTimeManagerContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(ScreenTimeManagerContext context)
		{
#if RELEASE
// use AddOrUpdate to seed here
// Seeds here should only be for production code
	        IList<RuleBase> ruleList = new List<RuleBase>();

	        // Keep this rule in production
			// Nothing works without this rule.
			// IT IS REQUIRED
	        ruleList.Add(new RuleBase()
	        {
		        RuleType = RuleType.Timer,
		        RuleModifier = RuleModifier.Subtract,
		        RuleTitle = "Timer Used",
		        RuleDescription = "Used the timer to deduct time",
		        VariableRatioNumerator = 1,
		        VariableRatioDenominator = 1,
		        IsHidden = true
	        });

	        // Keep this rule in production
	        ruleList.Add(new RuleBase()
	        {
		        RuleType = RuleType.Fixed,
		        RuleModifier = RuleModifier.Add,
		        RuleTitle = "Cleaned Room",
		        RuleDescription = "Earn 30 minutes for cleaning room",
		        FixedTimeEarned = new TimeSpan(0, 0, 30, 0)
	        });


			// If I understand my research correctly, this should seed the database if it's empty, and update the entries (rather than adding new ones) if it is not.
	        foreach (var rule in ruleList)
		        context.Rules.AddOrUpdate(r => new { r.RuleTitle, r.RuleDescription, r.RuleType }, rule);

	        context.SaveChanges();

			#endif
		}
	}
}