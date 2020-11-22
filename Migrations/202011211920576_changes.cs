namespace CineMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RentedMovies", "Movie_ImdbID", "dbo.Movies");
            DropForeignKey("dbo.RentedMovies", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.RentedMovies", new[] { "UserId" });
            DropIndex("dbo.RentedMovies", new[] { "Movie_ImdbID" });
            DropTable("dbo.RentedMovies");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RentedMovies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateOfRent = c.DateTime(nullable: false),
                        MovieId = c.String(),
                        UserId = c.String(maxLength: 128),
                        Movie_ImdbID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.RentedMovies", "Movie_ImdbID");
            CreateIndex("dbo.RentedMovies", "UserId");
            AddForeignKey("dbo.RentedMovies", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.RentedMovies", "Movie_ImdbID", "dbo.Movies", "ImdbID");
        }
    }
}
