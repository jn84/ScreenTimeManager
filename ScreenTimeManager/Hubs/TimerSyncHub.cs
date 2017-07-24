using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using ScreenTimeManager.Models;
using ScreenTimeManager.Utility;

namespace ScreenTimeManager.Hubs
{
	public class TimerSyncHub : Hub
	{

		public TimerSyncHub()
		{
			TotalScreenTimeManager.TotalScreenTimeChangedNotifier += this.TotalScreenTimeChangedOrUpdated;
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
			UpdateClientsTimerState(ElapsedTimer.State, TotalScreenTimeManager.GetTotalTime_Now());
		}


		// A client clicked the start/stop button

		public void ToggleTimerState()
		{
			ElapsedTimer.ToggleTimer();
		}
	}
}