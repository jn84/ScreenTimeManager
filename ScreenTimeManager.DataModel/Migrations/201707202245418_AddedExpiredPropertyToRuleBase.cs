using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class AddedExpiredPropertyToRuleBase : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.RuleBases", "IsExpired", c => c.Boolean(false, false));
		}

		public override void Down()
		{
			DropColumn("dbo.RuleBases", "IsExpired");
		}
	}
}