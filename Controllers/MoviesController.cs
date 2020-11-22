﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CineMovie.Models;
using CineMovie.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

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
        public ActionResult ConfigureChanges(string forRent, int quantity, decimal rentPrice, string MovieId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var movie = db.Movies.Find(MovieId);

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

        [Authorize]
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
                    MovieImdbId = movie.ImdbID
                });
            });

            db.ReservedMovies.AddRange(moviesToReserve);
            db.RentCarts.RemoveRange(moviesInCart);
            db.SaveChanges();

            return RedirectToAction("ReservedMovies", "Movies");
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddMovieToCart(string MovieId, string moviename)
        {
            if (!string.IsNullOrEmpty(MovieId))
            {
                var userId = User.Identity.GetUserId();

                var alreadyReserved = db.ReservedMovies.Where(x => x.UserId == userId).Any(x => x.MovieImdbId == MovieId);

                if (alreadyReserved) return Json("inReserve", JsonRequestBehavior.AllowGet);

                var alreadyInCart = db.RentCarts.Where(x => x.UserId == userId).Any(x => x.ImdbID == MovieId);

                if (alreadyInCart) return Json("inCart", JsonRequestBehavior.AllowGet);

                var movie = new RentCart
                {
                    Id = Guid.NewGuid(),
                    ImdbID = MovieId,
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

        public JsonResult RemoveFromCart(Guid MovieId)
        {
            if (MovieId != null)
            {
                var userid = User.Identity.GetUserId();

                var movieToRemove = db.RentCarts.Where(c => c.UserId == userid && c.Id == MovieId).FirstOrDefault();

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
        public ActionResult ReservedMovies()
        {
            var userId = User.Identity.GetUserId();

            var movies = db.ReservedMovies.Where(x => x.UserId == userId).Include(x => x.Movie).ToList();

            return View(movies);
        }

        public JsonResult RemoveFromReserved(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var userid = User.Identity.GetUserId();

                var reservedMovie = db.ReservedMovies.Where(c => c.UserId == userid && c.MovieImdbId == id).FirstOrDefault();

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

        public ActionResult RemoveSelectedReserved(string[] moviesToRemove)
        {
            if (moviesToRemove != null)
            {
                var userid = User.Identity.GetUserId();

                var reservedToRemove = db.ReservedMovies.Where(x => moviesToRemove.Contains(x.MovieImdbId) && x.UserId == userid).ToList();

                if (reservedToRemove != null)
                {
                    db.ReservedMovies.RemoveRange(reservedToRemove);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("ReservedMovies");
        }

        // ------------------------- RENTED MOVIE ACTIONS -------------------------


        [Authorize(Roles = "Administrator")]
        public ActionResult AdminReservedMovies(string search, DateTime? dateFrom, DateTime? dateTo, int page = 1, int recordsPerPage = 5)
        {
            DateTime dt = new DateTime();

            if (!dateFrom.HasValue && !dateTo.HasValue)
            {
                dateFrom = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);
                dateTo = DateTime.Now;
            }

            dt = dateTo.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            List<ReservedMovie> rentMovies;

            if (string.IsNullOrEmpty(search))
            {
                rentMovies = db.ReservedMovies.Where(x => x.DateOfReservation >= dateFrom && x.DateOfReservation <= dt)
                    .Include(x => x.User).ToList().Skip((page - 1) * recordsPerPage)
                    .Take(recordsPerPage).ToList();
            }
            else
            {
                rentMovies = db.ReservedMovies.Include(x => x.User)
                    .Where(x => x.DateOfReservation >= dateFrom && x.DateOfReservation <= dt && x.User.UserName.ToLower().Contains(search.ToLower()))
                    .ToList().Skip((page - 1) * recordsPerPage)
                    .Take(recordsPerPage).ToList();
            }

            var moviesForRent = rentMovies.Join(db.Movies.ToList(), x => x.MovieImdbId, s => s.ImdbID, (rent, movies) => new ReservedMoviesViewModel
            {
                DateOfReservation = rent.DateOfReservation,
                MovieTitle = movies.Title,
                UserName = rent.User.UserName,
                RentPrice = movies.RentPrice,
                MovieId = movies.ImdbID,
                UserId = rent.UserId
            }).ToList();

            var reservedMoviesPagination = new ReservedMoviesPagination
            {
                ReservedMoviesViewModels = moviesForRent,
                ActualPage = page,
                TotalRegister = db.ReservedMovies.Count(),
                RegisterPerPage = recordsPerPage,
                PaginationValues = new RouteValueDictionary()
            };

            ViewBag.RecordsPerPage = new SelectList(new int[] { 5, 10, 20, 30, 50 }, recordsPerPage);
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.Search = search;

            return View(reservedMoviesPagination);
        }

        [HttpPost]
        public ActionResult AdminReservedMovies(string[] selectableRent)
        {
            if (selectableRent != null)
            {
                var userMovie = new List<UserMovie>();

                foreach (var item in selectableRent)
                {
                    userMovie.Add(JsonConvert.DeserializeObject<UserMovie>(item));
                }

                var reservedMovies = userMovie.Join(db.ReservedMovies, x => new { movId = x.MovieId, userId = x.UserId }, s => new { movId = s.MovieImdbId, userId = s.UserId }, (x, s) => new ReservedMovie
                {
                    Id = s.Id,
                    DateOfReservation = s.DateOfReservation,
                    MovieImdbId = s.MovieImdbId,
                    UserId = s.UserId,
                }).ToList();

                var reservedMoviesToRemove = db.ReservedMovies.ToList().Where(x => reservedMovies.Any(s => s.MovieImdbId == x.MovieImdbId && s.UserId == x.UserId)).ToList();

                var rentedMovies = reservedMovies.Select(x => new RentedMovie
                {
                    DateOfRent = DateTime.Now,
                    MovieImdbId = x.MovieImdbId,
                    UserId = x.UserId
                }).ToList();


                db.RentedMovies.AddRange(rentedMovies);
                db.ReservedMovies.RemoveRange(reservedMoviesToRemove);

                db.SaveChanges();
            }

            return RedirectToAction("AdminReservedMovies");
        }

        public ActionResult RentedMovies()
        {
            var rentedMovies = db.RentedMovies.Include(x => x.User).Include(x => x.Movie).ToList();

            return View(rentedMovies);
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
