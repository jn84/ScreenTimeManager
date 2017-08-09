using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class RemoveTupleFromRuleBaseAndReplace : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.RuleBases", "VariableRatioNumerator", c => c.Int(false));
			AddColumn("dbo.RuleBases", "VariableRatioDenominator", c => c.Int(false));
		}

		public override void Down()
		{
			DropColumn("dbo.RuleBases", "VariableRatioDenominator");
			DropColumn("dbo.RuleBases", "VariableRatioNumerator");
		}
	}
}