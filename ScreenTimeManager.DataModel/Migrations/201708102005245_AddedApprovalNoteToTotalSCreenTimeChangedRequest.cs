namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedApprovalNoteToTotalSCreenTimeChangedRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TotalScreenTimeChangedRequests", "ApprovalNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TotalScreenTimeChangedRequests", "ApprovalNote");
        }
    }
}
