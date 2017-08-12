namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsDeniedAndIsFinalizedPropertiesToTSTC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TotalScreenTimeChangeds", "IsDenied", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.TotalScreenTimeChangeds", "IsFinalized", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TotalScreenTimeChangeds", "IsFinalized");
            DropColumn("dbo.TotalScreenTimeChangeds", "IsDenied");
        }
    }
}
