using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CineMovie.ViewModels
{
    public class ReservedMoviesViewModel
    {
        [Display(Name = "Date of reservation")]
        public DateTime DateOfReservation { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Movie title")]
        public string MovieTitle { get; set; }

        [Display(Name = "Rent Price")]
        public decimal RentPrice { get; set; }
        public string MovieId { get; set; }
        public string UserId { get; set; }

        public string LateClassColor
        {
            get
            {
                if (DateOfReservation >= DateTime.Now.AddDays(-1))
                    return "badge badge-success";
                else if (DateOfReservation >= DateTime.Now.AddDays(-4))
                    return "badge badge-warning";
                else
                    return "badge badge-danger";
            }
        }

    }
}