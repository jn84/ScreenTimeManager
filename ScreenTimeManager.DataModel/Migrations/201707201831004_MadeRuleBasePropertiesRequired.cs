using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class MadeRuleBasePropertiesRequired : DbMigration
	{
		public override void Up()
		{
			AlterColumn("dbo.RuleBases", "RuleTitle", c => c.String(false));
			AlterColumn("dbo.RuleBases", "RuleDescription", c => c.String(false));
		}

		public override void Down()
		{
			AlterColumn("dbo.RuleBases", "RuleDescription", c => c.String());
			AlterColumn("dbo.RuleBases", "RuleTitle", c => c.String());
		}
	}
}