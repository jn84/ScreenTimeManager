namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTupleFromRuleBaseAndReplace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RuleBases", "VariableRatioNumerator", c => c.Int(nullable: false));
            AddColumn("dbo.RuleBases", "VariableRatioDenominator", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RuleBases", "VariableRatioDenominator");
            DropColumn("dbo.RuleBases", "VariableRatioNumerator");
        }
    }
}
