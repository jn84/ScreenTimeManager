using Microsoft.AspNet.SignalR;
using ScreenTimeManager.Utility;

namespace ScreenTimeManager.Hubs
{
	public class TimerSyncHub : Hub
	{
		public TimerSyncHub()
		{
			TotalScreenTimeManager.TotalScreenTimeChangedNotifier += TotalScreenTimeChangedOrUpdated;
		}

		private void TotalScreenTimeChangedOrUpdated(object sender, TotalScreenTimeChangedEventArgs e)
		{
			UpdateClientsTimerState(e.CurrentTimerState, e.TotalSecondsAvailable);
		}

		private void UpdateClientsTimerState(TimerState state, long totalSeconds)
		{
			if (state == TimerState.Begin || state == TimerState.Running)
			{
				Clients.All.doTimerStateUpdate(true, totalSeconds);
				return;
			}
			Clients.All.doTimerStateUpdate(false, totalSeconds);
		}

		// For clients to call during the initial visit to the application page

		public void SyncTimer()
		{
			UpdateClientsTimerState(TimerState.Stopped, TotalScreenTimeManager.GetTotalTime_Now());
		}


		// A client clicked the start/stop button

		public void ToggleTimerState()
		{
			//ElapsedTimer.ToggleTimer();
		}
	}
}