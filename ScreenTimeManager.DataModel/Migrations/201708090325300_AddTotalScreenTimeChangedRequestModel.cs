using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class AddTotalScreenTimeChangedRequestModel : DbMigration
	{
		public override void Up()
		{
			CreateTable(
					"dbo.TotalScreenTimeChangedRequests",
					c => new
					{
						Id = c.Int(false, true),
						SecondsAdded = c.Long(false),
						TimeHistoryDateId = c.Int(false),
						RecordAddedTime = c.Time(false, 7),
						RuleUsedId = c.Int(false),
						SubmissionNote = c.String(),
						RequestedBy = c.String()
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.RuleBases", t => t.RuleUsedId, true)
				.ForeignKey("dbo.TimeHistoryDates", t => t.TimeHistoryDateId, true)
				.Index(t => t.TimeHistoryDateId)
				.Index(t => t.RuleUsedId);

			AddColumn("dbo.TotalScreenTimeChangeds", "ApprovedBy", c => c.String());
			AddColumn("dbo.TotalScreenTimeChangeds", "RequestedBy", c => c.String());
		}

		public override void Down()
		{
			DropForeignKey("dbo.TotalScreenTimeChangedRequests", "TimeHistoryDateId", "dbo.TimeHistoryDates");
			DropForeignKey("dbo.TotalScreenTimeChangedRequests", "RuleUsedId", "dbo.RuleBases");
			DropIndex("dbo.TotalScreenTimeChangedRequests", new[] {"RuleUsedId"});
			DropIndex("dbo.TotalScreenTimeChangedRequests", new[] {"TimeHistoryDateId"});
			DropColumn("dbo.TotalScreenTimeChangeds", "RequestedBy");
			DropColumn("dbo.TotalScreenTimeChangeds", "ApprovedBy");
			DropTable("dbo.TotalScreenTimeChangedRequests");
		}
	}
}