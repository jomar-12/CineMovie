namespace CineMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingMovieProps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movies", "ForRent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Movies", "RentPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Movies", "Day", c => c.Int());
            AddColumn("dbo.Movies", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "Discriminator");
            DropColumn("dbo.Movies", "Day");
            DropColumn("dbo.Movies", "RentPrice");
            DropColumn("dbo.Movies", "ForRent");
        }
    }
}
