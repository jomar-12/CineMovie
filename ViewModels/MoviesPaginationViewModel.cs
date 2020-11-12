using CineMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.ViewModels
{
    public class MoviesPaginationViewModel : PaginationBase
    {
        public List<Movie> Movies { get; set; }
    }
}