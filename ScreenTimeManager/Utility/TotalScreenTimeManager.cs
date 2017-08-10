using System;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Web.WebPages;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;

namespace ScreenTimeManager.Utility
{
	public static class TotalScreenTimeManager
	{
		// ### Begin Event Code TODO: Move events to partial?

		// Fire an event whenever screen time changes
		public delegate void TotalScreenTimeChangedEventHandler(object sender, TotalScreenTimeChangedEventArgs e);


		// // // // // Timer specific variables
		// If the timer is NOT running, create a new record
		// If the timer IS running, update the last used ScreenTimeHistory object with the new time elapsed
		private static TimerState _timerState = TimerState.Stopped;

		// Nullable to eliminate confusion about whether or not we need to add or update
		private static int? _lastTimeHistoryId;

		// ### End Event Code


		// This guy listens to the timer, then notifies the signalr hub about necessary info
		static TotalScreenTimeManager()
		{
			ElapsedTimer.ElapsedTimerNotifier += TimerStateChangedOrUpdated;
		}

		// Why = delegate {}
		// https://stackoverflow.com/questions/289002/how-to-raise-custom-event-from-a-static-class
		public static event TotalScreenTimeChangedEventHandler TotalScreenTimeChangedNotifier = delegate { };

		private static void OnTotalScreenTimeChangedNotify(TotalScreenTimeChangedEventArgs e)
		{
			// Sender is null since "this" is static
			TotalScreenTimeChangedNotifier.Invoke(null, e);
		}

		private static void TimerStateChangedOrUpdated(object sender, ElapsedTimerEventArgs e)
		{
			AddOrUpdateTimerEntry(e.State, e.MillisecondsElapsed);
		}

		// timeApplied is nullable since not all rules have an input for it (variable rules)
		private static TotalScreenTimeChanged GenerateTotalScreenTimeChanged(RuleBase rule, long? timeAppliedMilliseconds, string submissionNote)
		{
			var timeChanged = new TotalScreenTimeChanged
			{
				RuleUsedId = rule.Id,
				SubmissionNote = submissionNote
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
						// Shouldn't happen. But who knows?
						throw new Exception("Time applied cannot be null for RuleType.Variable or RuleType.Timer: RuleType was " + rule);

					timeChanged.SecondsAdded =
						GetModifiedTimeInSeconds(rule, (long) timeAppliedMilliseconds);
					break;
				default:
					throw new Exception("This should not have happened. RuleType was null or had another unexpected value.");
			}
			return timeChanged;
		}

		public static TotalScreenTimeChanged GenerateTotalScreenTimeChangedApproved(
			RuleBase rule, 
			long? timeAppliedMilliseconds,
			string approvedBy,
			string submissionNote
			)
		{
			TotalScreenTimeChanged tstc =
				GenerateTotalScreenTimeChanged(rule, timeAppliedMilliseconds, submissionNote);

			tstc.ApprovedBy = approvedBy;

			return tstc;
		}

		public static TotalScreenTimeChangedRequest GenerateTotalScreenTimeChangedRequest(
			RuleBase rule,
			long? timeAppliedMilliseconds,
			string requestedBy,
			string submissionNote)
		{
			TotalScreenTimeChanged tstc = GenerateTotalScreenTimeChanged(rule, timeAppliedMilliseconds, submissionNote);

			return new TotalScreenTimeChangedRequest
			{
				SecondsAdded = tstc.SecondsAdded,
				RuleUsedId = tstc.RuleUsedId,
				SubmissionNote = tstc.SubmissionNote,
				RequestedBy = requestedBy
			};
		}

		public static TotalScreenTimeChanged GetApprovedTotalScreenTimeChangedRequest(TotalScreenTimeChangedRequest tstcr, string approvedBy, string approvalNote)
		{
			return new TotalScreenTimeChanged
			{
				SecondsAdded = tstcr.SecondsAdded,
				ApprovedBy = approvedBy,
				RequestedBy = tstcr.RequestedBy,
				RuleUsedId = tstcr.RuleUsedId,
				SubmissionNote = approvalNote,
				RequestNote = tstcr.SubmissionNote
			};
		}

		public static long GetModifiedTimeInMillisecnds(RuleBase rule, long timeAppliedInMilliseconds)
		{
			// Apply the positive or negative factor from RuleModifier
			double modifiedMilliseconds = (int) rule.RuleModifier * timeAppliedInMilliseconds;

			// Calculate the ratio factor to apply to the applied time span
			double ratio = (double) rule.VariableRatioNumerator / rule.VariableRatioDenominator;

			// Get the final result
			// Behold my kindness: we always round up
			return (long) Math.Ceiling(modifiedMilliseconds * ratio);
		}

		public static long GetModifiedTimeInSeconds(RuleBase rule, long timeAppliedInMilliseconds)
		{
			double modifiedSeconds = (int) rule.RuleModifier * timeAppliedInMilliseconds;
			double ratio = (double) rule.VariableRatioNumerator / rule.VariableRatioDenominator;

			// Behold my kindness: we always round up
			return (long) Math.Ceiling(modifiedSeconds * ratio / 1000);
		}

		// If everyong uses this, we'll all be consistent
		public static string FormatTimeSpan(long milliseconds)
		{
			TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds);

			Debug.WriteLine(ts.ToString());
			Debug.WriteLine(ts.TotalMilliseconds);

			return ts.ToString(
				Math.Floor(Math.Abs(ts.TotalDays)) > 0 ? "ddd'd 'hh'h 'mm'm 'ss's '" : "hh'h 'mm'm 'ss's'");
		}

		public static void AddOrUpdateRuleAppliedRequest(TotalScreenTimeChangedRequest changed)
		{
			if (changed == null)
				throw new Exception("Error in add/update TotalScreenTimeChangedRequest entry to database: changed object was null");

			using (var ctx = new ScreenTimeManagerContext())
			{
				ctx.TimeRequests.AddOrUpdate(changed);
				ctx.SaveChanges();
			}
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

			OnTotalScreenTimeChangedNotify(new TotalScreenTimeChangedEventArgs(_timerState, GetTotalTime_Now()));
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
				// 
				// After further consideration, this doesn't seem too bad.
				// As long as the database initialization functions correctly on first start
				// there shouldn't be any issues... unless somehow the rule is edited/deleted

				// It should be the only rule with that RuleType
				RuleBase rule = ctx.Rules.First(r => r.RuleType == RuleType.Timer);

				switch (state)
				{
					case TimerState.Begin:
						// Create a new one
						timeChanged = GenerateTotalScreenTimeChangedApproved(rule, timeElapsedMilliseconds, "Application", "Time used via timer");
						ctx.TimeChanged.Add(timeChanged);
						ctx.SaveChanges();
						// Save a reference to the database entry for the currently running timer
						// This way it can be updated periodically, and closed out with the final value.
						_lastTimeHistoryId = timeChanged.Id;
						_timerState = TimerState.Running;
						break;

					case TimerState.Running:
						// Update the current one
						timeChanged = ctx.TimeChanged.Find(_lastTimeHistoryId);

						// The timerstate claims that the timer is running, but the entry doesn't exist in the database. Boo!
						if (timeChanged == null)
							throw new Exception(
								"Timer state is running, but the TotalScreenTimeChanged object was not found in the database.");

						// Update the entry in the database. 
						timeChanged.SecondsAdded = GetModifiedTimeInSeconds(rule, timeElapsedMilliseconds);
						ctx.SaveChanges();
						break;

					case TimerState.End:
						// Finalize the current one
						timeChanged = ctx.TimeChanged.Find(_lastTimeHistoryId);
						if (timeChanged == null)
							throw new Exception(
								"Timer state is running, but the TotalScreenTimeChanged object was not found in the database.");
						timeChanged.SecondsAdded = GetModifiedTimeInSeconds(rule, timeElapsedMilliseconds);
						ctx.SaveChanges();

						// Reset the state
						_lastTimeHistoryId = null;
						_timerState = TimerState.Stopped;
						break;

					default:
						// Timer state is Stopped or an unknown value. Shouldn't happen.. but who knows
						throw new Exception("Timer state is Stopped or other unhandled value.");
				}

				OnTotalScreenTimeChangedNotify(new TotalScreenTimeChangedEventArgs(_timerState, GetTotalTime_Now()));
			}
		}

		//// TODO: Placeholder method. This will take (probably) a long time to execute after many records have been added. Implement a running total.
		public static long GetTotalTime_Now()
		{
			long dbTotalMinusTimer;
			using (var ctx = new ScreenTimeManagerContext())
			{
				dbTotalMinusTimer = ctx.TimeChanged.Where(tstc => tstc.Id != _lastTimeHistoryId).Sum(tstc => tstc.SecondsAdded);
			}

			// Why + (-val): It makes it clearer (to me) what exactly is happening
			// We add a value to the record, that happens to be a negative value
			// This is how it works in the database, so it's consistent
			// The compiler will convert it to plain subtraction anyway
			return dbTotalMinusTimer + -ElapsedTimer.GetTimeElapsedInSeconds();
		}

		// Utility methods related to handing timer events
		public static long? ConvertHoursMinutesToMilliseconds(int hours, int minutes)
		{
			var span = new TimeSpan(0, hours, minutes, 0);

			return (long) span.TotalMilliseconds;
		}

		public static long? ConvertHoursMinutesToMilliseconds(string hourString, string minuteString)
		{
			long hours, minutes;

			if (hourString.IsEmpty() || minuteString.IsEmpty())
				return null;

			if (!long.TryParse(hourString, out hours) || !long.TryParse(minuteString, out minutes))
				return null;

			return (hours * 60 + minutes) * 60000;
		}

		private static string MinutePlurality(int val)
		{
			return val == 1 ? "minute" : "minutes";
		}


		// Using the given rule, builds a string describing what the rule will do when applied
		// No one needs to construct this information on thier own, they can just pass the rule and use the value given
		public static string BuildRuleDetailString(RuleBase rule)
		{
			string temp = rule.RuleModifier == RuleModifier.Add ? "Earns " : "Deducts ";

			if (rule.RuleType == RuleType.Fixed)
				temp += FormatTimeSpan((long) rule.FixedTimeEarned.TotalMilliseconds);

			else if (rule.RuleType == RuleType.Variable)
				temp += rule.VariableRatioNumerator + " " +
				        MinutePlurality(rule.VariableRatioNumerator) +
				        " for every " + rule.VariableRatioDenominator + " "
				        + MinutePlurality(rule.VariableRatioDenominator) + " applied";
			return temp;
		}
	}

	// This is only used via the timer
	// So when the clients asks for an update, it should get the current count of the timer PLUS the old value from the database (from before the timer started)

	public class TotalScreenTimeChangedEventArgs : EventArgs
	{
		public TotalScreenTimeChangedEventArgs(TimerState state, long totalSecondsAvailable)
		{
			CurrentTimerState = state;
			TotalSecondsAvailable = totalSecondsAvailable;
		}

		public TimerState CurrentTimerState { get; }

		public long TotalSecondsAvailable { get; }
	}
}