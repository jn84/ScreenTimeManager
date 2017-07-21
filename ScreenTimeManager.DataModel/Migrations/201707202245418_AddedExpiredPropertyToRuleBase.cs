namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedExpiredPropertyToRuleBase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RuleBases", "IsExpired", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RuleBases", "IsExpired");
        }
    }
}
