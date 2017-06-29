using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private HomeService service = new HomeService();

        public ActionResult Index()
        {
            HomeViewModel viewModel = service.GetViewModel();

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "A non-profit web application.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}