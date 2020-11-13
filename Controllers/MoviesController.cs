using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
        public ActionResult Index(string search, int page = 1, int recordsPerPage = 8)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var filterMovies = db.Movies.Where(x => x.Title.Contains(search)).OrderBy(x => x.Title).ToList();

                var filterMoviesPagination = new MoviesPaginationViewModel
                {
                    Movies = filterMovies,
                    ActualPage = page,
                    TotalRegister = filterMovies.Count(),
                    RegisterPerPage = recordsPerPage,
                    PaginationValues = new RouteValueDictionary()
                };

                if (filterMovies.Count == 0)
                {
                    ViewBag.FilterNotFoundMovies = "No movies were found with this title";
                }

                return View(filterMoviesPagination);
            }

            ViewBag.Success = TempData["Success"];

            var movies = db.Movies.OrderBy(x => x.Title).ToList()
                .Skip((page - 1) * recordsPerPage)
                .Take(recordsPerPage).ToList();

            var movieList = new MoviesPaginationViewModel
            {
                Movies = movies,
                ActualPage = page,
                TotalRegister = db.Movies.Count(),
                RegisterPerPage = recordsPerPage,
                PaginationValues = new RouteValueDictionary()
            };

            return View(movieList);
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

        public ActionResult RentMovie(string search, int page = 1, int recordsPerPage = 8)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var filterRentMovies = db.Movies.Where(x => x.ForRent && x.Title.Contains(search)).OrderBy(x => x.Title).ToList();

                var filterRentMoviesPagination = new MoviesPaginationViewModel
                {
                    Movies = filterRentMovies,
                    ActualPage = page,
                    TotalRegister = filterRentMovies.Count(),
                    RegisterPerPage = recordsPerPage,
                    PaginationValues = new RouteValueDictionary()
                };

                if (filterRentMovies.Count == 0)
                {
                    ViewBag.FilterNotFoundMovies = "No movies were found with this title";
                }

                return View(filterRentMoviesPagination);
            }

            var moviesForRent = db.Movies.Where(c => c.ForRent).OrderBy(x => x.Title).ToList()
                .Skip((page - 1) * recordsPerPage)
                .Take(recordsPerPage).ToList();

            var moviesForRentPagiantion = new MoviesPaginationViewModel
            {
                Movies = moviesForRent,
                ActualPage = page,
                TotalRegister = db.Movies.Where(x => x.ForRent).Count(),
                RegisterPerPage = recordsPerPage,
                PaginationValues = new RouteValueDictionary()
            };

            return View(moviesForRentPagiantion);
        }

        // ------------------------- CART ACTIONS -------------------------

        [Authorize]
        public ActionResult ReserveMovies()
        {
            var userId = User.Identity.GetUserId();

            var moviesInCart = db.RentCarts.Where(x => x.UserId == userId).ToList();

            var moviesToReserve = new List<ReservedMovie>();

            moviesInCart.ForEach(movie =>
            {
                moviesToReserve.Add(new ReservedMovie
                {
                    Id = Guid.NewGuid(),
                    DateOfReservation = DateTime.Now.ToLocalTime(),
                    UserId = movie.UserId,
                    MovieId = movie.ImdbID
                });
            });

            db.ReservedMovies.AddRange(moviesToReserve);
            db.RentCarts.RemoveRange(moviesInCart);
            db.SaveChanges();

            return RedirectToAction("ReservedMovies", "Movies");
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

                int moviecount = (int)Session["CartMovieCount"] + 1;
                Session["CartMovieCount"] = moviecount;

                return Json(new { moviecount, moviename }, JsonRequestBehavior.AllowGet);
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

                if (movieToRemove == null) return Json("", JsonRequestBehavior.AllowGet);

                db.RentCarts.Remove(movieToRemove);
                var success = db.SaveChanges() > 0;

                int moviecount = (int)Session["CartMovieCount"] - 1;
                Session["CartMovieCount"] = moviecount;

                return Json(new { success, moviecount }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        // ------------------------- RESERVED MOVIE ACTIONS -------------------------

        [Authorize]
        public ActionResult ReservedMovies(string[] moviesToRemove)
        {
            var userId = User.Identity.GetUserId();

            var movies = db.ReservedMovies.Where(x => x.UserId == userId).Join(db.Movies, c => c.MovieId, s => s.ImdbID, (res, mov) => new MovieCartViewModel
            {
                DateAdded = res.DateOfReservation,
                ImdbId = mov.ImdbID,
                Poster = mov.Poster,
                RentPrice = mov.RentPrice,
                Title = mov.Title,
            }).ToList();

            return View(movies);
        }

        public JsonResult RemoveFromReserved(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var userid = User.Identity.GetUserId();

                var reservedMovie = db.ReservedMovies.Where(c => c.UserId == userid && c.MovieId == id).FirstOrDefault();

                if (reservedMovie != null)
                {
                    db.ReservedMovies.Remove(reservedMovie);
                    db.SaveChanges();

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json("");
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
