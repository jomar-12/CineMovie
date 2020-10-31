using CineMovie.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CineMovie.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Movie()
        {
            ViewBag.AddedMovies = TempData["AddedMovies"];
            return View();
        }

        [HttpPost]
        public ActionResult Movie(string title, string submit, string[] addedmovies)
        {
            switch (submit)
            {
                case "search":
                    RestClient restClient = new RestClient(Properties.Resources.OmdbApiEndpoint);
                    RestRequest restRequest = new RestRequest(Method.GET);
                    restRequest.AddParameter("s", title);
                    restRequest.AddParameter("apikey", Properties.Resources.OmdbApiKey);

                    IRestResponse<MovieRequest> restResponse = restClient.Execute<MovieRequest>(restRequest);

                    if (restResponse.StatusCode == HttpStatusCode.OK && restResponse.Data.Response.Equals("True"))
                    {
                        return View(restResponse.Data);
                    }

                    var errorResult = JsonConvert.DeserializeObject<ApiContentResult>(restResponse.Content);

                    ViewBag.ErrorResult = errorResult.Error;

                    return View();

                case "add":

                    if (addedmovies != null)
                    {
                        var addedMoviesToObjectList = new List<Movie>();

                        foreach (var movie in addedmovies)
                        {
                            addedMoviesToObjectList.Add(JsonConvert.DeserializeObject<Movie>(movie));
                        }

                        TempData["AddedMovies"] = "succesfull";

                        return RedirectToAction("Movie");
                    }

                    ViewBag.ErrorResult = "There's no movie selected";

                    return View();

                default:
                    return View();
            }

        }


    }
}