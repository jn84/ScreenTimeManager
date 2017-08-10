namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeIsApprovedPropertyNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TotalScreenTimeChangedRequests", "IsApproved", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TotalScreenTimeChangedRequests", "IsApproved", c => c.Boolean(nullable: false));
        }
    }
}
