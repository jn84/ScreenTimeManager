using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class AddedSubmissionNotePropertyToTotalScreenTimeChanged : DbMigration
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