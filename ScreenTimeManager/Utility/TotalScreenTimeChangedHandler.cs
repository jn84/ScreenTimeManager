using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.DynamicData;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models.Enums;
using ScreenTimeManager.Utility;

namespace ScreenTimeManager.Models.Utility
{
	public static class TotalScreenTimeChangedHandler
	{
		// // // // // Timer specific variables
		// If the timer is NOT running, create a new record
		// If the timer IS running, update the last used ScreenTimeHistory object with the new time elapsed
		private static TimerState _timerState = TimerState.Stopped;

		// Nullable to eliminate confusion about whether or not we need to add or update
		private static int? _lastTimeHistoryId = null;


		// timeApplied is nullable since not all rules have an input for it (variable rules)
		public static TotalScreenTimeChanged GenerateTotalScreenTimeChanged(RuleBase rule, long? timeAppliedMilliseconds)
		{
			var timeChanged = new TotalScreenTimeChanged
			{
				RuleUsedId = rule.Id,
				RecordAddedDateTime = DateTime.Now
			};

			switch (rule.RuleType)
			{

				// Need an omnipresent RuleType for timer history

				case RuleType.Fixed:
					timeChanged.SecondsAdded = (int) rule.RuleModifier * (long) rule.FixedTimeEarned.TotalSeconds;
					break;
				case RuleType.Timer:
				case RuleType.Variable:
					if (timeAppliedMilliseconds == null)
						throw new Exception("Time applied cannot be null for RuleType.Variable or RuleType.Timer: RuleType was " + rule);

					timeChanged.SecondsAdded = GetModifiedTimeInMilliseconds(rule, (long) timeAppliedMilliseconds);
					break;
				default:
					throw new Exception("This should not have happened. RuleType was null or had another unexpected value.");

			}
			return timeChanged;
		}

		private static long GetModifiedTimeInMilliseconds(RuleBase rule, long timeAppliedMilliseconds)
		{
			double modifiedSeconds = (int)rule.RuleModifier * timeAppliedMilliseconds;
			double ratio = (double)rule.VariableRatioNumerator / rule.VariableRatioDenominator;

			// Behold my kindness: we always round up
			return (long) (modifiedSeconds * ratio);
		}

		private static long GetModifiedTimeInSeconds(long timeAppliedInMilliseconds)
		{
			return (long) Math.Ceiling((double) timeAppliedInMilliseconds / 1000);
		}

		//public static TotalScreenTimeChanged GetResult()
		//{
		//	var timeChanged = new TotalScreenTimeChanged
		//	{
		//		RuleUsedId = rule.Id,
		//		RecordAddedDateTime = DateTime.Now
		//	};


		//	switch (rule.RuleType)
		//	{
		//		case RuleType.Fixed:
		//			timeChanged.SecondsAdded = (int) _rule.RuleModifier * (long) _rule.FixedTimeEarned.TotalSeconds;
		//			break;
		//		case RuleType.Variable:
		//			double modifiedSeconds = (int) _rule.RuleModifier * _timeAppliedSeconds;
		//			double ratio = (double) _rule.VariableRatioNumerator / _rule.VariableRatioDenominator;

		//			timeChanged.SecondsAdded = (long) (modifiedSeconds * ratio);
		//			break;
		//		default:
		//			throw new Exception("This should not have happened. RuleType was null or had another unexpected value.");

		//	}
		//	return timeChanged;
		//}

		private static int SaveScreenTimeHistoryEntry()
		{
			return 1;
		}

		public static void AddOrUpdateTimerEntry(TimerState state, long timeElapsedMilliseconds)
		{
			using (var ctx = new ScreenTimeManagerContext())
			{
				TotalScreenTimeChanged timeChanged = null;

				// I don't like this. We rely on the database having the default timer rule.
				// This rule SHOULD exist (and everything is kind of pointless without it) but yuck
				// Either way, we need a reference to this rule
				var rule = ctx.Rules.First(r => r.RuleType == RuleType.Timer);

				// In case someone else needs to know what's happening. Might not be needed
				_timerState = state;

				switch (state)
				{
					// This is a problem. Going to need static rules.

					case TimerState.Begin:
						// Create a new one
						timeChanged = GenerateTotalScreenTimeChanged(rule, timeElapsedMilliseconds);
						ctx.TimeChanged.Add(timeChanged);
						ctx.SaveChanges();

						_lastTimeHistoryId = timeChanged.Id;
						break;
					case TimerState.Running:
						timeChanged = ctx.TimeChanged.Find(_lastTimeHistoryId);
						timeChanged.SecondsAdded = Get

						break;
					case TimerState.End:
						// Finalize the current one

					default:
				}

			}
		}
	}
}

