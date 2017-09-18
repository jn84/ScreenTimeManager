namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRecordAddedTimeColumns : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TotalScreenTimeChangeds", "RecordAddedTime");
            DropColumn("dbo.TotalScreenTimeChangedRequests", "RecordAddedTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TotalScreenTimeChangedRequests", "RecordAddedTime", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.TotalScreenTimeChangeds", "RecordAddedTime", c => c.Time(nullable: false, precision: 7));
        }
    }
}
