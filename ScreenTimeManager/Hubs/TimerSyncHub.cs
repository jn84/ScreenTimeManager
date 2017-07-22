using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace ScreenTimeManager.Hubs
{
	public class TimerSyncHub : Hub
	{
		public void UpdateClientsTimerState(bool isRunning, long timeInSeconds)
		{
			Clients.All.doTimerUpdate(isRunning, timeInSeconds);
		}

		// For clients to call during the initial visit to the application page

		public void GetTimerState()
		{
			// Grab the timer state

			Clients.Caller.doTimerUpdate();
		}

		// A client clicked the start/stop button

		public void ToggleTimerState()
		{
			// // // // If there is no TimeRunning object (== null)
			// Server creates a new TimeRunning object (should be IDisposable..?)
			// // TimeRunning has a timer
			// // TimeRunning creates a ScreenTimeHistory object.. ?
			// // The ScreenTimeHistory object is commited to the database with an initial time of zero
			// // Every n ticks of the timer, the ScreenTimeHistory object in the database will be updated with how much time has elapsed
			// // // This is behavior is a failsafe against power outages, etc.
			// // // When the database time is updated, the server should also send out an update to the clients so that they can stay synchronized

			// // // // If there IS a TimeRunning object
			// Finalize the ScreenTimeHistory object
			// Dispose of the TimeRunning object and set the reference to null

		}



		// The server should decide the state of the button and timer
		// 
		// 
		// 
		// 
	}
}