﻿using System;
using System.Data.Entity.Migrations;
using System.Linq;
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

		// Why = delegate {}
		// https://stackoverflow.com/questions/289002/how-to-raise-custom-event-from-a-static-class
		public static event TotalScreenTimeChangedEventHandler TotalScreenTimeChangedNotifier = delegate { };

		private static void OnTotalScreenTimeChangedNotify(TotalScreenTimeChangedEventArgs e)
		{
			// Sender is null since "this" is static
			TotalScreenTimeChangedNotifier.Invoke(null, e);
		}

		// ### End Event Code


		// This guy listens to the timer, then notifies the hub about necessary info
		static TotalScreenTimeManager()
		{
			ElapsedTimer.ElapsedTimerNotifier += TimerStateChangedOrUpdated;
		}

		private static void TimerStateChangedOrUpdated(object sender, ElapsedTimerEventArgs e)
		{
			AddOrUpdateTimerEntry(e.State, e.MillisecondsElapsed);

			// Don't need to call the event here since the AddOrUpdate method wil ltake care of that at its closing
			// OnTotalScreenTimeChangedNotify(new TotalScreenTimeChangedEventArgs(e.State));
		}


		// // // // // Timer specific variables
		// If the timer is NOT running, create a new record
		// If the timer IS running, update the last used ScreenTimeHistory object with the new time elapsed
		private static TimerState _timerState = TimerState.Stopped;

		// Nullable to eliminate confusion about whether or not we need to add or update
		private static int? _lastTimeHistoryId = null;

		private static long? _timerBeginTotalSeconds = null;


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

			if (_timerState == TimerState.Begin || _timerState == TimerState.Running)
				OnTotalScreenTimeChangedNotify(new TotalScreenTimeChangedEventArgs(_timerState, GetTotalTime_Now()));
			OnTotalScreenTimeChangedNotify(new TotalScreenTimeChangedEventArgs(_timerState, GetTotalTime_Database()));

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
						_timerBeginTotalSeconds = GetTotalTime_Database();
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
				}

				OnTotalScreenTimeChangedNotify(new TotalScreenTimeChangedEventArgs(_timerState, GetTotalTime_Database()));
			}
		}

		// TODO: Placeholder method. This will take (probably) a long time to execute after many records are added. Implement a running total.
		public static long GetTotalTime_Database()
		{
			using (var ctx = new ScreenTimeManagerContext())
				// This sums the current timer value INCLUDING the currently active timer
				return ctx.TimeChanged.Sum(changed => changed.SecondsAdded);
		}

		public static long GetTotalTime_BeforeTimer()
		{
			if (_timerBeginTotalSeconds != null)
				return (long) _timerBeginTotalSeconds;
			return 0;
		}

		public static long GetTotalTime_Now()
		{
			if (_timerState != TimerState.Running)
				return GetTotalTime_Database();
			return GetTotalTime_BeforeTimer() + ElapsedTimer.GetTimeElapsedInSeconds();

		}

	}

	// This is only used via the timer
	// So when the clients asks for an update, it should get the current count of the timer PLUS the old value from the database (from before the timer started)

	public class TotalScreenTimeChangedEventArgs : EventArgs
	{
		public TimerState CurrentTimerState { get; }

		public long TotalSecondsAvailable { get; }

		public TotalScreenTimeChangedEventArgs(TimerState state, long totalSecondsAvailable)
		{
			CurrentTimerState = state;
			TotalSecondsAvailable = totalSecondsAvailable;
		}
	}
}
