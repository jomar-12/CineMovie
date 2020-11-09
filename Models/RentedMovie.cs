using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.Models
{
    public class RentedMovie
    {
        public string Id { get; set; }
        public DateTime DateOfRent { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
        public Movie Movie { get; set; }
        public ApplicationUser User { get; set; }
    }
}