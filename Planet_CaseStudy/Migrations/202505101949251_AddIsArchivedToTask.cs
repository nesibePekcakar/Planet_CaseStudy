namespace Planet_CaseStudy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsArchivedToTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "IsArchived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tasks", "IsArchived");
        }
    }
}
