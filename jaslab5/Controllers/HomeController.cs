using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jaslab5.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cabins()
        {
            ViewBag.Message = "Cabins page.";
            return View();
        }
        
        public ActionResult Passengers()
        {
            ViewBag.Message = "Passengers page.";
            return View();
        }
    }
}