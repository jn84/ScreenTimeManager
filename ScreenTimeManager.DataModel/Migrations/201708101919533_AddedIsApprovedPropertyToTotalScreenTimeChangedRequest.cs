namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsApprovedPropertyToTotalScreenTimeChangedRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TotalScreenTimeChangedRequests", "IsApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TotalScreenTimeChangedRequests", "IsApproved");
        }
    }
}
