using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class AddedRuleModifierToRuleBase : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.RuleBases", "RuleModifier", c => c.Int(false));
		}

		public override void Down()
		{
			DropColumn("dbo.RuleBases", "RuleModifier");
		}
	}
}