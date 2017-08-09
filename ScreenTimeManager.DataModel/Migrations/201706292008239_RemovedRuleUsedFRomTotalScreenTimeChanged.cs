using System.Data.Entity.Migrations;

namespace ScreenTimeManager.DataModel.Migrations
{
	public partial class RemovedRuleUsedFRomTotalScreenTimeChanged : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.TotalScreenTimeChangeds", "RuleUsedId", "dbo.RuleBases");
			DropIndex("dbo.TotalScreenTimeChangeds", new[] {"RuleUsedId"});
			AddColumn("dbo.TotalScreenTimeChangeds", "RuleBase_Id", c => c.Int());
			CreateIndex("dbo.TotalScreenTimeChangeds", "RuleBase_Id");
			AddForeignKey("dbo.TotalScreenTimeChangeds", "RuleBase_Id", "dbo.RuleBases", "Id");
		}

		public override void Down()
		{
			DropForeignKey("dbo.TotalScreenTimeChangeds", "RuleBase_Id", "dbo.RuleBases");
			DropIndex("dbo.TotalScreenTimeChangeds", new[] {"RuleBase_Id"});
			DropColumn("dbo.TotalScreenTimeChangeds", "RuleBase_Id");
			CreateIndex("dbo.TotalScreenTimeChangeds", "RuleUsedId");
			AddForeignKey("dbo.TotalScreenTimeChangeds", "RuleUsedId", "dbo.RuleBases", "Id", true);
		}
	}
}