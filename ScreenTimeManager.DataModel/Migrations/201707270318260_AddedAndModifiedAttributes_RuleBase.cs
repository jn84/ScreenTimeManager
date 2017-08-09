using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class AddedAndModifiedAttributes_RuleBase : DbMigration
	{
		public override void Up()
		{
			AlterColumn("dbo.RuleBases", "RuleTitle", c => c.String(false, 63));
		}

		public override void Down()
		{
			AlterColumn("dbo.RuleBases", "RuleTitle", c => c.String(false));
		}
	}
}