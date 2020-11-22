using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.Models
{
    public class RentedMovie
    {
        public int Id { get; set; }
        public DateTime DateOfRent { get; set; }
        public string MovieImdbId { get; set; }
        public string UserId { get; set; }
        public Movie Movie { get; set; }
        public ApplicationUser User { get; set; }
    }
}