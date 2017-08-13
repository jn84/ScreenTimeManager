namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedSubmissionNoteInTSTCR : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TotalScreenTimeChangedRequests", "RequestNote", c => c.String());
            DropColumn("dbo.TotalScreenTimeChangedRequests", "SubmissionNote");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TotalScreenTimeChangedRequests", "SubmissionNote", c => c.String());
            DropColumn("dbo.TotalScreenTimeChangedRequests", "RequestNote");
        }
    }
}
