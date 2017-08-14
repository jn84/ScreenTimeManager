using System.Diagnostics;

namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsDeniedAndIsFinalizedPropertiesToTSTC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TotalScreenTimeChangeds", "IsDenied", c => c.Boolean(nullable: false, defaultValue: false));
			Sql("UPDATE dbo.TotalScreenTimeChangeds SET IsDenied=0");
            AddColumn("dbo.TotalScreenTimeChangeds", "IsFinalized", c => c.Boolean(nullable: false, defaultValue: true));
	        Sql("UPDATE dbo.TotalScreenTimeChangeds SET IsFinalized=1");
		}
        
        public override void Down()
        {
            DropColumn("dbo.TotalScreenTimeChangeds", "IsFinalized");
            DropColumn("dbo.TotalScreenTimeChangeds", "IsDenied");
        }
    }
}
