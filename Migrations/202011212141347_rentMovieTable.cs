namespace CineMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rentMovieTable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ReservedMovies", name: "Movie_ImdbID", newName: "MovieImdbId");
            RenameIndex(table: "dbo.ReservedMovies", name: "IX_Movie_ImdbID", newName: "IX_MovieImdbId");
            CreateTable(
                "dbo.RentedMovies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateOfRent = c.DateTime(nullable: false),
                        MovieImdbId = c.String(maxLength: 128),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movies", t => t.MovieImdbId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MovieImdbId)
                .Index(t => t.UserId);
            
            DropColumn("dbo.ReservedMovies", "MovieId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReservedMovies", "MovieId", c => c.String());
            DropForeignKey("dbo.RentedMovies", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RentedMovies", "MovieImdbId", "dbo.Movies");
            DropIndex("dbo.RentedMovies", new[] { "UserId" });
            DropIndex("dbo.RentedMovies", new[] { "MovieImdbId" });
            DropTable("dbo.RentedMovies");
            RenameIndex(table: "dbo.ReservedMovies", name: "IX_MovieImdbId", newName: "IX_Movie_ImdbID");
            RenameColumn(table: "dbo.ReservedMovies", name: "MovieImdbId", newName: "Movie_ImdbID");
        }
    }
}
