using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CineMovie.ViewModels
{
    public class RentedMovieViewModel
    {
        [Display(Name = "Date of Rent")]
        public DateTime DateOfRent { get; set; }

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
                if (DateOfRent >= DateTime.Now.AddDays(-1))
                    return "success";
                else if (DateOfRent >= DateTime.Now.AddDays(-4))
                    return "warning";
                else
                    return "danger";
            }
        }
    }
}