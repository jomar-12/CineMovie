using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CineMovie.Models
{
    public class Movie
    {
        [Key]
        public string ImdbID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
        public bool ForRent { get; set; }

        [Display(Name = "Rent price")]
        public decimal RentPrice { get; set; }
        public int InStock { get; set; }

        public ICollection<RentedMovie> RentedMovies { get; set; }
        public ICollection<ReservedMovie> ReservedMovies { get; set; }
    }
}