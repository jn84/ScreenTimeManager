namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStartOfDayTotalSecondsToTimeHistoryDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeHistoryDates", "StartOfDayTotalSeconds", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeHistoryDates", "StartOfDayTotalSeconds");
        }
    }
}
