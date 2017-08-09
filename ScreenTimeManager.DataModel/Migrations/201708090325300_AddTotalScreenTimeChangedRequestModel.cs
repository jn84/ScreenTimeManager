namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTotalScreenTimeChangedRequestModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TotalScreenTimeChangedRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SecondsAdded = c.Long(nullable: false),
                        TimeHistoryDateId = c.Int(nullable: false),
                        RecordAddedTime = c.Time(nullable: false, precision: 7),
                        RuleUsedId = c.Int(nullable: false),
                        SubmissionNote = c.String(),
                        RequestedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RuleBases", t => t.RuleUsedId, cascadeDelete: true)
                .ForeignKey("dbo.TimeHistoryDates", t => t.TimeHistoryDateId, cascadeDelete: true)
                .Index(t => t.TimeHistoryDateId)
                .Index(t => t.RuleUsedId);
            
            AddColumn("dbo.TotalScreenTimeChangeds", "ApprovedBy", c => c.String());
            AddColumn("dbo.TotalScreenTimeChangeds", "RequestedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TotalScreenTimeChangedRequests", "TimeHistoryDateId", "dbo.TimeHistoryDates");
            DropForeignKey("dbo.TotalScreenTimeChangedRequests", "RuleUsedId", "dbo.RuleBases");
            DropIndex("dbo.TotalScreenTimeChangedRequests", new[] { "RuleUsedId" });
            DropIndex("dbo.TotalScreenTimeChangedRequests", new[] { "TimeHistoryDateId" });
            DropColumn("dbo.TotalScreenTimeChangeds", "RequestedBy");
            DropColumn("dbo.TotalScreenTimeChangeds", "ApprovedBy");
            DropTable("dbo.TotalScreenTimeChangedRequests");
        }
    }
}
