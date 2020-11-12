using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CineMovie.Models
{
    public class ReservedMovie
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateOfReservation { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }
        public Movie Movie { get; set; }
        public ApplicationUser User { get; set; }
    }
}