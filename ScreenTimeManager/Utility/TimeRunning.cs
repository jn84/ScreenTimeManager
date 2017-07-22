using System.Diagnostics;
using System.Timers;
using ScreenTimeManager.DataModel.DataContexts;

namespace ScreenTimeManager.Utility
{
	// Should there be subscribers? That way the class can just let everyone know about events like start and stop

	public static class TimeRunning
	{
		public static bool IsRunning { get; } = false;

		private static Timer _timer = null;

		private static Stopwatch _stopWatch = null;

		public static void BeginTimer()
		{
			_timer = new Timer(0)
			{
				Interval = 10000
			};

			_stopWatch = new Stopwatch();
			

			_stopWatch.Start();
			_timer.Start();

		}

		public static void EndTimer()
		{
			if (_stopWatch != null)
			{
				_stopWatch.Stop();

			}

			if (_timer != null)
			{
				_timer.Stop();
			}


			// Commit the final time
			
			_timer.Dispose();
			_timer = null;
		}

		public static long GetTimeElapsed()
		{
			return _timer.

			return 0;
		}
	}
}
