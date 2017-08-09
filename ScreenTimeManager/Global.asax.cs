using System.Data.Entity;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ScreenTimeManager.DataModel.DataContexts;
using ScreenTimeManager.DataModel.Initializers;

namespace ScreenTimeManager
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			Database.SetInitializer(new ScreenTimeManagerInitializer());

			using (var ctx = new ScreenTimeManagerContext())
			{
				SetSingleUser(ctx);

				ctx.Database.Initialize(true);
			}

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		[Conditional("DEBUG")]
		private void SetSingleUser(ScreenTimeManagerContext ctx)
		{
			ctx.Database.ExecuteSqlCommand(
				TransactionalBehavior.DoNotEnsureTransaction,
				$"ALTER DATABASE [{ctx.Database.Connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
		}
	}
}