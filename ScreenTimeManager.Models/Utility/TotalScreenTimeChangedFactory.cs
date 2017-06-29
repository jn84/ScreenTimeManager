using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenTimeManager.Models.Enums;

namespace ScreenTimeManager.Models.Utility
{
	class TotalScreenTimeChangedFactory
	{
		private readonly long _timeAppliedSeconds = 0;

		private readonly RuleBase _rule;

		private TotalScreenTimeChangedFactory() { }

		public TotalScreenTimeChangedFactory(RuleBase rule, long? timeApplied)
		{
			if (timeApplied != null)
				_timeAppliedSeconds = (long)timeApplied;
			_rule = rule;
		}

		public TotalScreenTimeChanged GetResult()
		{
			var timeChanged = new TotalScreenTimeChanged
			{
				RuleUsedId = _rule.Id,
				RecordAddedDateTime = DateTime.Now
			};


			switch (_rule.RuleType)
			{
				case RuleType.Fixed:
					timeChanged.SecondsAdded = (int) _rule.RuleModifier * (long)_rule.FixedTimeEarned.TotalSeconds;
					break;
				case RuleType.Variable:
					double modifiedSeconds = (int) _rule.RuleModifier * _timeAppliedSeconds;
					double ratio = (double) _rule.VariableRatioNumerator / _rule.VariableRatioDenominator;

					timeChanged.SecondsAdded = (long) (modifiedSeconds * ratio);
					break;
				default:
					throw new Exception("This should not have happened. RuleType was null or had another unexpected value.");

			}
			return timeChanged;
		}
	}
}
