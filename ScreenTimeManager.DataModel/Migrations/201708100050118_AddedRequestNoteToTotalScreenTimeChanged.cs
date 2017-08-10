namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRequestNoteToTotalScreenTimeChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TotalScreenTimeChangeds", "RequestNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TotalScreenTimeChangeds", "RequestNote");
        }
    }
}
