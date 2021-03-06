namespace CineMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReservedMovieTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReservedMovies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DateOfReservation = c.DateTime(nullable: false),
                        MovieId = c.String(),
                        UserId = c.String(maxLength: 128),
                        Movie_MovieId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movies", t => t.Movie_MovieId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.Movie_MovieId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReservedMovies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReservedMovies", "Movie_MovieId", "dbo.Movies");
            DropIndex("dbo.ReservedMovies", new[] { "Movie_MovieId" });
            DropIndex("dbo.ReservedMovies", new[] { "UserId" });
            DropTable("dbo.ReservedMovies");
        }
    }
}
