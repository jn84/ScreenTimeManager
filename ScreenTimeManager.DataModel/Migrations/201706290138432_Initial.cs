namespace ScreenTimeManager.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RuleBases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RuleType = c.Int(nullable: false),
                        RuleTitle = c.String(),
                        RuleDescription = c.String(),
                        FixedTimeEarned = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TotalScreenTimeChangeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SecondsAdded = c.Long(nullable: false),
                        RecordAddedDateTime = c.DateTime(nullable: false),
                        RuleUsedId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RuleBases", t => t.RuleUsedId, cascadeDelete: true)
                .Index(t => t.RuleUsedId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TotalScreenTimeChangeds", "RuleUsedId", "dbo.RuleBases");
            DropIndex("dbo.TotalScreenTimeChangeds", new[] { "RuleUsedId" });
            DropTable("dbo.TotalScreenTimeChangeds");
            DropTable("dbo.RuleBases");
        }
    }
}
