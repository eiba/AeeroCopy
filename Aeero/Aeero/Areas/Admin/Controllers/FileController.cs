using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aeero.Models;

namespace Aeero.Areas.Admin.Controllers
{
    public class FileController : Controller
    {
        // GET: Admin/File
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int id)
        {
            var fileToRetrieve = db.Files.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
    }
}