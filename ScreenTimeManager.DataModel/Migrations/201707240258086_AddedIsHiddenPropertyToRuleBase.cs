namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsHiddenPropertyToRuleBase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RuleBases", "IsHidden", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RuleBases", "IsHidden");
        }
    }
}
