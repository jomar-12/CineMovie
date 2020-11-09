using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CineMovie.Models;

namespace CineMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Movies
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            ViewBag.Success = TempData["Success"];

            return View(db.Movies.OrderBy(x => x.Title).ToList());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(string id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult ConfigureChanges(string forRent, int quantity, decimal rentPrice, string movieId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var movie = db.Movies.Find(movieId);

                if (movie == null)
                {
                    return HttpNotFound();
                }

                movie.ForRent = forRent == "true";
                movie.InStock = quantity;
                movie.RentPrice = rentPrice;

                db.SaveChanges();

                TempData["Success"] = true;
                return RedirectToAction("Index");
            }
        }

        public ActionResult RentMovie()
        {
            var moviesForRent = db.Movies.Where(c => c.ForRent).ToList();

            return View(moviesForRent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
