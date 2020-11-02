namespace CineMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class instockProp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "InStock", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "InStock");
        }
    }
}
