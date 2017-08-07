using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace ScreenTimeManager.Utility
{
	// For scheduling and executing daily maintenance or cleanup operations


	// Send the task, the time you want it executed, and whether or not it should be repeated

	public static class Scheduler
	{
		// Is this even needed?
		private static IEnumerable<Task> _tasks;

		// If frequency is 0, do it only once.
		public static void ScheduleTask(Action<Task> targetTask, DateTime targetTime, TimeSpan frequency)
		{
			var ts = GetTimeSpanUntilExecute(targetTime);

			// Starts the task that we want after the calculated time interval
			var t = Task.Delay(ts.Milliseconds).ContinueWith(targetTask);

			t.Wait();
		}

		private static TimeSpan GetTimeSpanUntilExecute(DateTime timeToExecute)
		{
			// Greater than zero -- t1 is later than t2.
			// timeToExecute must always be in the future.
			// Is it desireable to stop the works if the time is not in the future?
			if (DateTime.Compare(timeToExecute, DateTime.Now) <= 0)
				throw new ArgumentException("timeToExecute must be a DateTime in the future.");

			return timeToExecute - DateTime.Now;
		}

	}
}