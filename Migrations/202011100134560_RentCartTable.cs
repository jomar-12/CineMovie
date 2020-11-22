namespace CineMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RentCartTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RentCarts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MovieId = c.String(),
                        DateAdded = c.DateTime(nullable: false),
                        UserId = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            DropColumn("dbo.Movies", "Day");
            DropColumn("dbo.Movies", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Movies", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Movies", "Day", c => c.Int());
            DropForeignKey("dbo.RentCarts", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.RentCarts", new[] { "ApplicationUser_Id" });
            DropTable("dbo.RentCarts");
        }
    }
}
