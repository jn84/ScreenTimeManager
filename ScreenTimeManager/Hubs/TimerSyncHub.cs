using Microsoft.AspNet.SignalR;
using ScreenTimeManager.Utility;

namespace ScreenTimeManager.Hubs
{
	// So this class can be further abstracted. When a client update is necessary,
	// this class will recieve a nicely packaged object with all the info needed to send out to the clients.
	// All the client should need to do is parse out the information and apply it to the necessary timers.

	// On the client side, each timer button should its related GUID, probably as an HTML id tag
	// When a timer start/stop button is pressed, the signal to toggle the timer,
	// along with the GUID, should be passed to THIS class (via AJAX)

	// On the client side, we're currently looking at:
	/////	$("button#btn-timer-toggle").click(function() {
	/////		timerHub.server.toggleTimerState();
	/////	});
	// Which calls the ToggleTimerState method in this class.
	// So, ToggleTimerState should be changed to accept a string (GUID)
	// In turn, it should notify TimerManager that there's been a change in the timer situation
	// TimerManager should then start/stop (set up/dispose) of the correct timer, then notify the 
	// relevant classes

	// Current questions:
	// Since the main timer is a special case, how will be handled?
	//		Who "owns" the main timer? Should it always exist in TimerManager's list of timers?
	// Who should be in charge of generating the GUID for the main timer?
	//		What should the main timer's GUID be based on? (The seed)
	// How do we make TotalScreenTimeManager aware of the main timer?
	//		As of now, it's /always/ aware, but that will have to change since the timer object might not always exist.
	// The main timer GUID mayb have to be a special case, with a globally known GUID

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
			// To be replaced with TimerManager

			// This should be notifying about ALL timers now.
			UpdateClientsTimerState(ElapsedTimer.State, TotalScreenTimeManager.GetTotalTime_Now());
		}


		// A client clicked the start/stop button

		public void ToggleTimerState()
		{
			// To be replaced with TimerManager
			// Will need to pass along the GUID of the timer that needs to be started/stopped.
			// The main timer will probably need to be a special case.
			// The main timer is the only timer than needs to be concerned with TotalScreenTimeManager
			ElapsedTimer.ToggleTimer();
		}
	}
}