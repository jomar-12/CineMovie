namespace CineMovie.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingMovieTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Movies",
                c => new
                {
                    ImdbID = c.String(nullable: false, maxLength: 128),
                    Title = c.String(),
                    Year = c.String(),
                    Type = c.String(),
                    Poster = c.String(),
                })
                .PrimaryKey(t => t.ImdbID);

        }

        public override void Down()
        {
            DropTable("dbo.Movies");
        }
    }
}
