namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAndModifiedAttributes_RuleBase : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RuleBases", "RuleTitle", c => c.String(nullable: false, maxLength: 63));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RuleBases", "RuleTitle", c => c.String(nullable: false));
        }
    }
}
