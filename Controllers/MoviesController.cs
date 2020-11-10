using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CineMovie.Models;
using CineMovie.ViewModels;
using Microsoft.AspNet.Identity;

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

        [HttpPost]
        [Authorize]
        public JsonResult AddMovieToCart(string movieid, string moviename)
        {
            if (!string.IsNullOrEmpty(movieid))
            {
                var userId = User.Identity.GetUserId();

                var alreadyInCart = db.RentCarts.Where(x => x.UserId == userId).Any(x => x.ImdbID == movieid);

                if (alreadyInCart) return Json("exist", JsonRequestBehavior.AllowGet);

                var movie = new RentCart
                {
                    Id = Guid.NewGuid(),
                    ImdbID = movieid,
                    DateAdded = DateTime.Now,
                    UserId = userId
                };

                db.RentCarts.Add(movie);
                db.SaveChanges();

                return Json(moviename, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetUsersCartMovies()
        {
            var userId = User.Identity.GetUserId();

            var userMoviesId = db.RentCarts.Where(c => c.UserId == userId).ToList();

            var cartJoinsMovie = userMoviesId.Join(db.Movies, r => r.ImdbID, m => m.ImdbID, (r, m) => new MovieCartViewModel
            {
                MovieCartId = r.Id,
                Poster = m.Poster,
                RentPrice = m.RentPrice,
                Title = m.Title
            }).ToList();

            return PartialView("_CartMovieAdded", cartJoinsMovie);
        }

        public JsonResult RemoveFromCart(Guid movieid)
        {
            if (movieid != null)
            {
                var userid = User.Identity.GetUserId();

                var movieToRemove = db.RentCarts.Where(c => c.UserId == userid && c.Id == movieid).FirstOrDefault();

                db.RentCarts.Remove(movieToRemove);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
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
