using System.Collections.Generic;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;

namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ScreenTimeManager.DataModel.DataContexts.ScreenTimeManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ScreenTimeManager.DataModel.DataContexts.ScreenTimeManagerContext context)
        {

	        IList<RuleBase> ruleList = new List<RuleBase>();

	        ruleList.Add(new RuleBase()
	        {
		        RuleType = RuleType.Fixed,
		        RuleTitle = "Cleaned Room",
		        RuleDescription = "Earn 30 minutes for cleaning room",
		        FixedTimeEarned = new TimeSpan(0, 0, 30, 0)
	        });

	        ruleList.Add(new RuleBase()
	        {
		        RuleType = RuleType.Fixed,
		        RuleTitle = "Changed Litter",
		        RuleDescription = "Earn 20 minutes for cleaning/changing all the litter boxes",
		        FixedTimeEarned = new TimeSpan(0, 0, 20, 0)

	        });

	        ruleList.Add(new RuleBase()
	        {
		        RuleType = RuleType.Variable,
		        RuleTitle = "Played Outside",
		        RuleDescription = "For every hour of time spent playing outside, earn 15 minutes",
		        TimeEarningRatio = new Tuple<int, int>(1, 4)
	        });

	        foreach (var rule in ruleList)
		        context.Rules.AddRange(ruleList);
		}
    }
}
