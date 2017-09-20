using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using ScreenTimeManager.Models.Interfaces;

namespace ScreenTimeManager.Utility
{
	public enum TimerState
	{
		Stopped = 0,
		Begin = 1,
		Running = 2,
		End = 3
	}

	public static class TimerManager
	{
		private static Dictionary<Guid, ElapsedTimer> _activeTimers = new Dictionary<Guid, ElapsedTimer>();

		/// <summary>
		/// Generate a unique object id
		/// </summary>
		/// <param name="className"></param>
		/// <param name="objectId"></param>
		/// <param name="o"></param>
		/// <returns></returns>
		public static Guid GenerateOid(IUniqueObject o)
		{
			return GuidUtility.Create(GuidUtility.IsoOidNamespace, o.GetType().ToString() + o.Id);
		}

		// This class should be the only subscriber to ElapsedTimer events
	}
	
	public class ElapsedTimer
	{
		// ### Begin event code TODO: Move events to partial?

		public delegate void ElapsedTimerEventHandler(object sender, ElapsedTimerEventArgs e);

		private Timer _timer;

		private Stopwatch _stopWatch;

		private int _updateInterval = 10000; // 10 seconds

		// ### End event code

		public int UpdateInterval
		{
			get => _updateInterval;
			set
			{
				if (State == TimerState.Running)
					return;
				_updateInterval = value;
			}
		}

		public TimerState State { get; private set; } = TimerState.Stopped;

		// Explanation: "Why = delegate {}"
		// https://stackoverflow.com/questions/289002/how-to-raise-custom-event-from-a-static-class
		public static event ElapsedTimerEventHandler ElapsedTimerNotifier = delegate { };

		private void OnElapsedTimerNotify(ElapsedTimerEventArgs e)
		{
			// Sender is null since "this" is static
			ElapsedTimerNotifier.Invoke(null, e);
		}

		// Timer triggers this, which in turn triggers the event
		private void OnElapsedTimerEvent(object source, ElapsedEventArgs e)
		{
			OnElapsedTimerNotify(new ElapsedTimerEventArgs(State, GetTimeElapsed()));
		}

		public bool IsRunning()
		{
			return _timer != null && _timer.Enabled && _stopWatch.IsRunning;
			// If one of these conditions is not like the others, something is broken
			// Maybe it should be checked...
		}

		// Reset the object state to not running
		public void Reset()
		{
			_timer.Dispose();
			_timer = null;
			_stopWatch = null;
			State = TimerState.Stopped;
		}

		public void ToggleTimer()
		{
			if (State == TimerState.Running)
			{
				EndTimer();
				return;
			}
			BeginTimer();
		}

		private void BeginTimer()
		{
			_timer = new Timer(_updateInterval)
			{
				Interval = UpdateInterval,
				AutoReset = true
			};

			_timer.Elapsed += OnElapsedTimerEvent;

			_stopWatch = new Stopwatch();

			_stopWatch.Start();
			_timer.Start();

			OnElapsedTimerNotify(new ElapsedTimerEventArgs(TimerState.Begin, 0));

			State = TimerState.Running;
		}

		private void EndTimer()
		{
			if (_stopWatch != null && _timer != null)
			{
				_stopWatch?.Stop();
				_timer?.Stop();

				// We let everyone know that the timer has stopped
				OnElapsedTimerNotify(new ElapsedTimerEventArgs(TimerState.End, _stopWatch.ElapsedMilliseconds));

				Reset();
			}
		}

		/// <summary>
		///     Get the current time elapsed of the running timer
		///     If the timer is not running, returns 0
		/// </summary>
		/// <returns>Returns the current time elapsed of the timer in milliseconds</returns>
		public long GetTimeElapsed()
		{
			if (IsRunning())
				return _stopWatch.ElapsedMilliseconds;

			return 0;
		}

		public long GetTimeElapsedInSeconds()
		{
			if (IsRunning())
				return _stopWatch.ElapsedMilliseconds / 1000;

			return 0;
		}
	}

	public class ElapsedTimerEventArgs : EventArgs
	{
		public ElapsedTimerEventArgs(TimerState state, long millisecondsElapsed)
		{
			State = state;
			MillisecondsElapsed = millisecondsElapsed;
		}

		public TimerState State { get; }

		public long MillisecondsElapsed { get; }
	}
}