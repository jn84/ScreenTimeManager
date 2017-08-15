namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStartOfDayTotalSecondsToTimeHistoryDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeHistoryDates", "StartOfDayTotalSeconds", c => c.Int(nullable: false));

			// Now we need to populate this column
			// Populate the earliest entry with 0
			Sql("UPDATE dbs.TimeHistoryDates " +
			    "SET StartOfDayTotalSeconds = 0 " +
			    "WHERE ");
		}

		public override void Down()
        {
            DropColumn("dbo.TimeHistoryDates", "StartOfDayTotalSeconds");
        }
    }
}
