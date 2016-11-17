using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aeero.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}