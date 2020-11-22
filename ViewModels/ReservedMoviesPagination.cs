using CineMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.ViewModels
{
    public class ReservedMoviesPagination : PaginationBase
    {
        public List<ReservedMoviesViewModel> ReservedMoviesViewModels { get; set; }
    }
}