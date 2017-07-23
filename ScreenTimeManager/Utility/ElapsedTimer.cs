using System.Diagnostics;
using System.Timers;
using ScreenTimeManager.DataModel.DataContexts;

namespace ScreenTimeManager.Utility
{
	// Should there be subscribers? That way the class can just let everyone know about events like start and stop

	public static class ElapsedTimer
	{
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
			return _timer != null && _timer.Enabled;
		}

		public static void BeginTimer()
		{
			_timer = new Timer(0)
			{
				Interval = UpdateInterval,
				// method to call
			};

			_stopWatch = new Stopwatch();
			

			_stopWatch.Start();
			_timer.Start();

		}

		public static void EndTimer()
		{
			_stopWatch?.Stop();
			_timer?.Stop();


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
