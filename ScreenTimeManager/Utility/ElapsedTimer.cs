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

		// Timer triggers this, which in turn triggers the event
		private static void OnElapsedTimerEvent(object source, ElapsedEventArgs e)
		{
			OnElapsedTimerNotify(new ElapsedTimerEventArgs(IsRunning(), GetTimeElapsed()));
		}

		public static int UpdateInterval
		{
			get => _updateInterval;
			set
			{
				if (IsRunning())
					return;
				_updateInterval = value;
			}
		}

		private static Timer _timer = null;

		private static Stopwatch _stopWatch = null;

		private static int _updateInterval = 10000;

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
		}

		public static void ToggleTimer()
		{
			if (IsRunning())
			{
				EndTimer();
				return;
			}
			BeginTimer();
		}

		public static void BeginTimer()
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

			OnElapsedTimerNotify(new ElapsedTimerEventArgs(IsRunning(), 0));
		}

		public static void EndTimer()
		{
			if (_stopWatch != null && _timer != null)
			{
				_stopWatch?.Stop();
				_timer?.Stop();

				// We let everyone know that the timer has stopped
				OnElapsedTimerNotify(new ElapsedTimerEventArgs(IsRunning(), _stopWatch.ElapsedMilliseconds));

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
		public bool IsRunning { get; } = false;

		public long MillisecondsElapsed { get; } = 0;

		public ElapsedTimerEventArgs(bool isRunning, long millisecondsElapsed)
		{
			IsRunning = isRunning;
			MillisecondsElapsed = millisecondsElapsed;
		}
	}
}
