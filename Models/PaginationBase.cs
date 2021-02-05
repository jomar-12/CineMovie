using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace CineMovie.Models
{
    public class PaginationBase
    {
        public int ActualPage { get; set; }
        public int TotalRegister { get; set; }
        public int RegisterPerPage { get; set; }
        public RouteValueDictionary PaginationValues { get; set; }
    }
}