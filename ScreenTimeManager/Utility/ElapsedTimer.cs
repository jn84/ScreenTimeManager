using System;
using System.Diagnostics;
using System.Timers;
using ScreenTimeManager.DataModel.DataContexts;

namespace ScreenTimeManager.Utility
{
	// Should there be subscribers? That way the class can just let everyone know about events like start and stop

	public static class ElapsedTimer
	{
		public delegate void ElapsedTimerEventHandler(object sender, ElapsedTimerEventArgs e);

		// Why = delegate {}
		// https://stackoverflow.com/questions/289002/how-to-raise-custom-event-from-a-static-class
		public static event ElapsedTimerEventHandler ElapsedTimerNotifier = delegate { };

		private static void OnElapsedTimerNotify(ElapsedTimerEventArgs e)
		{
			// Sender is null since "this" is static
			ElapsedTimerNotifier.Invoke(null, e);
		}

		//
		//
		//
		// This is wrong!!!
		// TotalScreenTimeChangedHandler should connect to all the clients, not this one. ElapsedTimer should inform TotalScreenTimeChangedHandler of changes, then the handler informs everyone from there.
		//
		//
		//

		// Timer triggers this, which in turn triggers the event
		private static void OnElapsedTimerEvent(object source, ElapsedEventArgs e)
		{
			OnElapsedTimerNotify(new ElapsedTimerEventArgs(_timerState, GetTimeElapsed()));
		}

		public static int UpdateInterval
		{
			get => _updateInterval;
			set
			{
				if (_timerState == TimerState.Running)
					return;
				_updateInterval = value;
			}
		}

		private static Timer _timer = null;

		private static Stopwatch _stopWatch = null;

		private static int _updateInterval = 10000;

		private static TimerState _timerState = TimerState.Stopped;

		public static bool IsRunning()
		{
			return _timer != null && _timer.Enabled && _stopWatch.IsRunning;
			// If one of these conditions is not like the others, something is broken
			// Maybe it should be checked...
		}

		// Reset the object state to not running
		public static void Reset()
		{
			_timer.Dispose();
			_timer = null;
			_stopWatch = null;
			_timerState = TimerState.Stopped;
		}

		public static void ToggleTimer()
		{
			if (_timerState == TimerState.Running)
			{
				EndTimer();
				return;
			}
			BeginTimer();
		}

		private static void BeginTimer()
		{
			_timer = new Timer(0)
			{
				Interval = UpdateInterval,
				AutoReset = true,
			};

			_timer.Elapsed += OnElapsedTimerEvent;

			_stopWatch = new Stopwatch();

			_stopWatch.Start();
			_timer.Start();

			OnElapsedTimerNotify(new ElapsedTimerEventArgs(TimerState.Begin, 0));

			_timerState = TimerState.Running;
		}

		private static void EndTimer()
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

		public static long GetTimeElapsed()
		{
			if (IsRunning())
				return _stopWatch.ElapsedMilliseconds;

			return 0;
		}
	}

	public class ElapsedTimerEventArgs : EventArgs
	{
		public TimerState State { get; } = TimerState.Stopped;

		public long MillisecondsElapsed { get; } = 0;

		public ElapsedTimerEventArgs(TimerState state, long millisecondsElapsed)
		{
			State = state;
			MillisecondsElapsed = millisecondsElapsed;
		}
	}

	public enum TimerState
	{
		Stopped = 0,
		Begin = 1,
		Running = 2,
		End = 3
	}
}
