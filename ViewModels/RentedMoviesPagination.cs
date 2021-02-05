using CineMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.ViewModels
{
    public class RentedMoviesPagination : PaginationBase
    {
        public List<RentedMovieViewModel> RentedMovieViewModels { get; set; }
    }
}