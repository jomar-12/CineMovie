using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CineMovie.Models
{
    public class MovieViewModel
    {
        public string imdbID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
    }

    public class MovieRequest
    {
        public List<MovieViewModel> Search { get; set; }
        public string totalResults { get; set; }
        public string Response { get; set; }
    }
}