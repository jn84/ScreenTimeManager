using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class AddedTimeHistoryDateTableAndModifiedRelationships : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.TotalScreenTimeChangeds", "RuleBase_Id", "dbo.RuleBases");
			DropIndex("dbo.TotalScreenTimeChangeds", new[] {"RuleBase_Id"});

			// The FK column RuleBase_Id was never actually used.. oops. All entries are null
			// All the FK data is actually stored in RuleUsedId
			// In other words, we can safely drop this column, and just make RuleUsedId the new FK entry
			DropColumn("dbo.TotalScreenTimeChangeds", "RuleBase_Id");

			// We don't need to rename this column, since we've deleted it
			// RenameColumn(table: "dbo.TotalScreenTimeChangeds", name: "RuleBase_Id", newName: "RuleUsedId");

			CreateTable(
					"dbo.TimeHistoryDates",
					c => new
					{
						Id = c.Int(false, true),
						EntriesDate = c.DateTime(false, storeType: "date")
					})
				.PrimaryKey(t => t.Id);

			// Here, we need to add the rows in TimeHistoryDates based on the data in TotalScreenTimeChanged table

			Sql(@"INSERT INTO dbo.TimeHistoryDates ( EntriesDate )
					SELECT DISTINCT CAST(RecordAddedDateTime as date)
					FROM dbo.TotalScreenTimeChangeds;");

			AddColumn("dbo.TotalScreenTimeChangeds", "TimeHistoryDateId", c => c.Int(false));

			// Here, the TotalScreenTimeChanged entries need to have the appropriate TimeHistoryDateId applied

			Sql(@"UPDATE tstc
					SET tstc.TimeHistoryDateId = thd.Id
					FROM dbo.TotalScreenTimeChangeds AS tstc
						INNER JOIN dbo.TimeHistoryDates AS thd
						ON CAST(tstc.RecordAddedDateTime AS date) = thd.EntriesDate;");

			AddColumn("dbo.TotalScreenTimeChangeds", "RecordAddedTime", c => c.Time(false, 7));

			// RecordAddedTime should be populated with the same row's RecordAddedDateTime column's time value

			Sql(@"UPDATE TotalScreenTimeChangeds 
					SET RecordAddedTime = CAST(RecordAddedDateTime AS time);");

			AlterColumn("dbo.TotalScreenTimeChangeds", "RuleUsedId", c => c.Int(false));
			CreateIndex("dbo.TotalScreenTimeChangeds", "TimeHistoryDateId");
			CreateIndex("dbo.TotalScreenTimeChangeds", "RuleUsedId");
			AddForeignKey("dbo.TotalScreenTimeChangeds", "TimeHistoryDateId", "dbo.TimeHistoryDates", "Id", true);
			AddForeignKey("dbo.TotalScreenTimeChangeds", "RuleUsedId", "dbo.RuleBases", "Id", true);
			DropColumn("dbo.TotalScreenTimeChangeds", "RecordAddedDateTime");
		}

		// This will break everything.
		public override void Down()
		{
			AddColumn("dbo.TotalScreenTimeChangeds", "RecordAddedDateTime", c => c.DateTime(false));
			DropForeignKey("dbo.TotalScreenTimeChangeds", "RuleUsedId", "dbo.RuleBases");
			DropForeignKey("dbo.TotalScreenTimeChangeds", "TimeHistoryDateId", "dbo.TimeHistoryDates");
			DropIndex("dbo.TotalScreenTimeChangeds", new[] {"RuleUsedId"});
			DropIndex("dbo.TotalScreenTimeChangeds", new[] {"TimeHistoryDateId"});
			AlterColumn("dbo.TotalScreenTimeChangeds", "RuleUsedId", c => c.Int());
			DropColumn("dbo.TotalScreenTimeChangeds", "RecordAddedTime");
			DropColumn("dbo.TotalScreenTimeChangeds", "TimeHistoryDateId");
			DropTable("dbo.TimeHistoryDates");
			RenameColumn("dbo.TotalScreenTimeChangeds", "RuleUsedId", "RuleBase_Id");
			AddColumn("dbo.TotalScreenTimeChangeds", "RuleUsedId", c => c.Int(false));
			CreateIndex("dbo.TotalScreenTimeChangeds", "RuleBase_Id");
			AddForeignKey("dbo.TotalScreenTimeChangeds", "RuleBase_Id", "dbo.RuleBases", "Id");
		}
	}
}