﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ScreenTimeManager.Startup))]
namespace ScreenTimeManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			ConfigureAuth(app);
	        app.MapSignalR();
		}
    }
}
