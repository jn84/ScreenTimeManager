using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class Initial : DbMigration
	{
		public override void Up()
		{
			CreateTable(
					"dbo.RuleBases",
					c => new
					{
						Id = c.Int(false, true),
						RuleType = c.Int(false),
						RuleTitle = c.String(),
						RuleDescription = c.String(),
						FixedTimeEarned = c.Time(false, 7)
					})
				.PrimaryKey(t => t.Id);

			CreateTable(
					"dbo.TotalScreenTimeChangeds",
					c => new
					{
						Id = c.Int(false, true),
						SecondsAdded = c.Long(false),
						RecordAddedDateTime = c.DateTime(false),
						RuleUsedId = c.Int(false)
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.RuleBases", t => t.RuleUsedId, true)
				.Index(t => t.RuleUsedId);
		}

		public override void Down()
		{
			DropForeignKey("dbo.TotalScreenTimeChangeds", "RuleUsedId", "dbo.RuleBases");
			DropIndex("dbo.TotalScreenTimeChangeds", new[] {"RuleUsedId"});
			DropTable("dbo.TotalScreenTimeChangeds");
			DropTable("dbo.RuleBases");
		}
	}
}