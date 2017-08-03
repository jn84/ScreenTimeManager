namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSubmissionNoteToTotalScreenTimeChangedModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TotalScreenTimeChangeds", "SubmissionNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TotalScreenTimeChangeds", "SubmissionNote");
        }
    }
}
