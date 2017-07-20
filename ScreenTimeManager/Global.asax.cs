using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ScreenTimeManager.DataModel.DataContexts;

namespace ScreenTimeManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
	        Database.SetInitializer(new DataModel.Initializers.ScreenTimeManagerInitializer());

	        using (ScreenTimeManagerContext ctx = new ScreenTimeManagerContext())
	        {
		        ctx.Database.ExecuteSqlCommand(
					TransactionalBehavior.DoNotEnsureTransaction,
			        $"ALTER DATABASE [{ctx.Database.Connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
				ctx.Database.Initialize(true);
	        }

			AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
