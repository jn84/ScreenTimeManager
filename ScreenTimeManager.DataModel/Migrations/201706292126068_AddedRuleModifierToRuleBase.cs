namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRuleModifierToRuleBase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RuleBases", "RuleModifier", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RuleBases", "RuleModifier");
        }
    }
}
