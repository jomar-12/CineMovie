using CineMovie.Models;
using System;
using System.Timers;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Linq;

namespace CineMovie
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly double TimerIntervalInMilliseconds = Convert.ToDouble(WebConfigurationManager.AppSettings["TimerIntervalInMilliseconds"]);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Timer timer = new Timer(TimerIntervalInMilliseconds)
            //{
            //    Enabled = true
            //};

            //timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            //timer.Start();
        }

        // Added the following procedure:
        static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime MyScheduledRunTime = DateTime.Parse(WebConfigurationManager.AppSettings["TimerStartTime"]);
            DateTime CurrentSystemTime = DateTime.Now;
            DateTime LatestRunTime = MyScheduledRunTime.AddMilliseconds(TimerIntervalInMilliseconds);

            using ApplicationDbContext db = new ApplicationDbContext();

            var rentedMovies = db.ReservedMovies.ToList();

            if (rentedMovies.Count > 0)
            {
                var moviesIds = rentedMovies.Select(x => x.MovieImdbId).ToList();
                var moviesToAddInStock = db.Movies.Where(x => moviesIds.Contains(x.ImdbID)).ToList();

                moviesToAddInStock.ForEach(x => x.InStock++);

                db.ReservedMovies.RemoveRange(rentedMovies);
                db.SaveChanges();
            }

        }
    }
}
