using System.Collections.Generic;
using ScreenTimeManager.Models;
using ScreenTimeManager.Models.Enums;

namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

	public sealed class Configuration : DbMigrationsConfiguration<ScreenTimeManager.DataModel.DataContexts.ScreenTimeManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ScreenTimeManager.DataModel.DataContexts.ScreenTimeManagerContext context)
        {

	       
        }
    }
}
