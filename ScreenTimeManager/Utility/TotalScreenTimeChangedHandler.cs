using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
		// ### Begin Event Code TODO: Move events to partial?

		// Fire an event whenever screen time changes
		public delegate void TotalScreenTimeChangedEventHandler(object sender, TotalScreenTimeChangedEventArgs e);

		// Why = delegate {}
		// https://stackoverflow.com/questions/289002/how-to-raise-custom-event-from-a-static-class
		public static event TotalScreenTimeChangedEventHandler TotalScreenTimeChangedNotifier = delegate { };

		private static void OnTotalScreenTimeChangedNotify(TotalScreenTimeChangedEventArgs e)
		{
			// Sender is null since "this" is static
			TotalScreenTimeChangedNotifier.Invoke(null, e);
		}

		// ### End Event Code


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

					timeChanged.SecondsAdded = 
							GetModifiedTimeInSeconds(rule, (long) timeAppliedMilliseconds);
					break;
				default:
					throw new Exception("This should not have happened. RuleType was null or had another unexpected value.");

			}
			return timeChanged;
		}

		private static long GetModifiedTimeInSeconds(RuleBase rule, long timeAppliedInMilliseconds)
		{
			double modifiedSeconds = (int)rule.RuleModifier * timeAppliedInMilliseconds;
			double ratio = (double)rule.VariableRatioNumerator / rule.VariableRatioDenominator;

			// Behold my kindness: we always round up
			return (long)Math.Ceiling((modifiedSeconds * ratio) / 1000);
		}

		public static void AddOrUpdateRuleAppliedEntry(TotalScreenTimeChanged changed)
		{
			if (changed == null)
				throw new Exception("Error in add/update TotalScreenTimeChanged entry to database: changed object was null");

			using (var ctx = new ScreenTimeManagerContext())
			{
				ctx.TimeChanged.AddOrUpdate(changed);
				ctx.SaveChanges();
			}
		}

		public static void AddOrUpdateTimerEntry(TimerState state, long timeElapsedMilliseconds)
		{
			using (var ctx = new ScreenTimeManagerContext())
			{
				TotalScreenTimeChanged timeChanged = null;

				// I don't like this. We rely on the database having the default timer rule.
				// This rule SHOULD exist (and everything is kind of pointless without it) but yuck
				// Either way, we need a reference to this rule, and it needs to be present in the database
				//// for historical purposes
				var rule = ctx.Rules.First(r => r.RuleType == RuleType.Timer);

				switch (state)
				{
					case TimerState.Begin:
						// Create a new one
						timeChanged = GenerateTotalScreenTimeChanged(rule, timeElapsedMilliseconds);
						ctx.TimeChanged.Add(timeChanged);
						ctx.SaveChanges();
						_lastTimeHistoryId = timeChanged.Id;
						_timerState = TimerState.Running;
						break;

					case TimerState.Running:
						// Update the current one
						timeChanged = ctx.TimeChanged.Find(_lastTimeHistoryId);
						if (timeChanged == null)
							throw new Exception("Timer state is listed as running, but the TotalScreenTimeChanged object was not found in the database.");
						timeChanged.SecondsAdded = GetModifiedTimeInSeconds(rule, timeElapsedMilliseconds);
						ctx.SaveChanges();
						break;

					case TimerState.End:
						// Finalize the current one
						timeChanged = ctx.TimeChanged.Find(_lastTimeHistoryId);
						if (timeChanged == null)
							throw new Exception("Timer state is running, but the TotalScreenTimeChanged object was not found in the database.");
						timeChanged.SecondsAdded = GetModifiedTimeInSeconds(rule, timeElapsedMilliseconds);
						ctx.SaveChanges();

						_lastTimeHistoryId = null;
						_timerState = TimerState.Stopped;
						break;

					default:
						// Timer state is Stopped or an unknown value. Shouldn't happen.. but who knows
						throw new Exception("Timer state is Stopped or other unhandled value.");
						break;

				}

				OnTotalScreenTimeChangedNotify(new TotalScreenTimeChangedEventArgs(GetCurrentTimerTotalSeconds()));
			}
		}

		// TODO: Placeholder method. This will take (probably) a long time to execute after many records are added. Implement a running total.
		public static long GetCurrentTimerTotalSeconds()
		{
			using (var ctx = new ScreenTimeManagerContext())
				return ctx.TimeChanged.Sum(changed => changed.SecondsAdded);
		}
	}

	public class TotalScreenTimeChangedEventArgs : EventArgs
	{
		public long TotalSecondsAvailable { get; }

		public TotalScreenTimeChangedEventArgs(long totalSecondsAvailable)
		{
			TotalSecondsAvailable = totalSecondsAvailable;
		}
	}
}

