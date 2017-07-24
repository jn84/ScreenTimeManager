using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Utility;
using ScreenTimeManager.Utility;

namespace ScreenTimeManager.Hubs
{
	public class TimerSyncHub : Hub
	{

		public TimerSyncHub()
		{
			ElapsedTimer.ElapsedTimerNotifier += this.TimerStateChangedOrUpdated;
			TotalScreenTimeChangedHandler.TotalScreenTimeChangedNotifier += this.TotalScreenTimeChangedOrUpdated;
		}

		private void TimerStateChangedOrUpdated(object sender, ElapsedTimerEventArgs e)
		{
			UpdateClientsTimerState(e.State);
		}

		private void TotalScreenTimeChangedOrUpdated(object sender, TotalScreenTimeChangedEventArgs e)
		{
			UpdateClientsTimerValue(TotalScreenTimeChangedHandler.GetCurrentTimerTotalSeconds());
		}

		private void UpdateClientsTimerState(TimerState state)
		{
			if (state == TimerState.Begin || state == TimerState.Running)
			{
				Clients.All.doTimerStateUpdate(true);
				return;
			}
			Clients.All.doTimerStateUpdate(false);
		}

		private void UpdateClientsTimerValue(long totalSecondsOnTimer)
		{
			Clients.All.doTimerValueUpdate(totalSecondsOnTimer);
		}

		// For clients to call during the initial visit to the application page

		public void SyncTimer()
		{
			UpdateClientsTimerState(ElapsedTimer.State);

			UpdateClientsTimerValue(TotalScreenTimeChangedHandler.GetCurrentTimerTotalSeconds());
		}


		// A client clicked the start/stop button

		public void ToggleTimerState()
		{
			ElapsedTimer.ToggleTimer();
		}
	}
}