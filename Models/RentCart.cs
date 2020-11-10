using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.Models
{
    public class RentCart
    {
        public Guid Id { get; set; }
        public string ImdbID { get; set; }
        public DateTime DateAdded { get; set; }
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}