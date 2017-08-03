using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;

namespace ScreenTimeManager.DataModel.Initializers
{
	public class ScreenTimeManagerInitializer : System.Data.Entity.CreateDatabaseIfNotExists<ScreenTimeManagerContext>
	{
		protected override void Seed(ScreenTimeManagerContext context)
		{
			base.Seed(context);

			IList<RuleBase> ruleList = new List<RuleBase>();

			// Keep this rule in production
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

			context.Rules.AddRange(ruleList);

			context.SaveChanges();
		}
	}
}
