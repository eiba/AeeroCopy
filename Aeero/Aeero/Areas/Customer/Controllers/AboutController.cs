using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aeero.Models;

namespace Aeero.Areas.Customer.Controllers
{
    public class AboutController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customer/About
        public ActionResult Index()
        {
            ViewBag.Hours = db.Hours.ToList();
            var facebook = new FacebookModel();
            var twitter = new TwitterModel();
            if (db.Facebook.Find(1) != null)
            {
                facebook = db.Facebook.Find(1);
            }
            if (db.Twitter.Find(1) != null)
            {
                twitter = db.Twitter.Find(1);
            }
            var AboutModel = new AboutModel
            {
                Contact = db.Contact.First(),
                Facebook = facebook,
                Twitter = twitter
            };
            return View(AboutModel);
        }
    }
}