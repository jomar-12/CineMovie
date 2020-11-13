using CineMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.ViewModels
{
    public class MovieCartViewModel
    {
        public Guid MovieCartId { get; set; }
        public DateTime DateAdded { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Poster { get; set; }
        public decimal RentPrice { get; set; }
    }
}