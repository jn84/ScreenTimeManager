namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRecordAddedDateTimeColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TotalScreenTimeChangeds", "RecordAddedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));

			Sql("UPDATE dbo.TotalScreenTimeChangeds " +
			    "SET RecordAddedDateTime = " +
			    "	DATEADD(day, DATEDIFF(day, CONVERT(DATE, '19000101', 112), EntriesDate), CAST(RecordAddedTime AS DATETIME2(7)))" +
				"FROM dbo.TotalScreenTimeChangeds " +
			    "	INNER JOIN dbo.TimeHistoryDates " +
			    "	ON dbo.TotalScreenTimeChangeds.TimeHistoryDateId = dbo.TimeHistoryDates.Id");

			AddColumn("dbo.TotalScreenTimeChangedRequests", "RecordAddedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));

	        Sql("UPDATE dbo.TotalScreenTimeChangedRequests " +
				"SET RecordAddedDateTime = " +
	            "	DATEADD(day, DATEDIFF(day, CONVERT(DATE, '19000101', 112), EntriesDate), CAST(RecordAddedTime AS DATETIME2(7)))" +
				"FROM dbo.TotalScreenTimeChangedRequests " +
	            "	INNER JOIN dbo.TimeHistoryDates " +
	            "	ON dbo.TotalScreenTimeChangedRequests.TimeHistoryDateId = dbo.TimeHistoryDates.Id");
		}
        
        public override void Down()
        {
            DropColumn("dbo.TotalScreenTimeChangedRequests", "RecordAddedDateTime");
            DropColumn("dbo.TotalScreenTimeChangeds", "RecordAddedDateTime");
        }
    }
}
