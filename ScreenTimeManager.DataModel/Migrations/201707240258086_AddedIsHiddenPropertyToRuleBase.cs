using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class AddedIsHiddenPropertyToRuleBase : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.RuleBases", "IsHidden", c => c.Boolean(false, false));
		}

		public override void Down()
		{
			DropColumn("dbo.RuleBases", "IsHidden");
		}
	}
}