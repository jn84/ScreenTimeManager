namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeRuleBasePropertiesRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RuleBases", "RuleTitle", c => c.String(nullable: false));
            AlterColumn("dbo.RuleBases", "RuleDescription", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RuleBases", "RuleDescription", c => c.String());
            AlterColumn("dbo.RuleBases", "RuleTitle", c => c.String());
        }
    }
}
