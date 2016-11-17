using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aeero.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Aeero.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class IngredientTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/IngredientTypes
        public ActionResult Index()
        {
            return View(db.IngredientType.ToList());
        }

        // GET: Admin/IngredientTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientType ingredientType = db.IngredientType.Find(id);
            if (ingredientType == null)
            {
                return HttpNotFound();
            }
            return View(ingredientType);
        }

        // GET: Admin/IngredientTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/IngredientTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TypeId,TypeName,TypeMax,TypeUnique,TypeExtras,isActive")] IngredientType ingredientType)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            if (ModelState.IsValid)
            {
                db.IngredientType.Add(ingredientType);
                db.SaveChanges();

                if (Request.IsAjaxRequest())
                {
                    var success = true;
                    var message = "Ingredienstypen " + ingredientType.TypeName + " ble opprettet.";
                    return RedirectToAction("showIngredientTypes", "ProductManagement", new { success = success, ingredientTypeId = ingredientType.TypeId, message = message, k=k });
                }

                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var success = false;
                var message = "Opps, det oppstod en feil så ingredienstypen "+ingredientType.TypeName +" ble ikke opprettet.";
                return RedirectToAction("showIngredientTypes", "ProductManagement", new { success = success,  ingredientTypeId = ingredientType.TypeId, message=message, k=k });
            }

            return View(ingredientType);
        }

        // GET: Admin/IngredientTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientType ingredientType = db.IngredientType.Find(id);
            if (ingredientType == null)
            {
                return HttpNotFound();
            }
            return View(ingredientType);
        }

        // POST: Admin/IngredientTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TypeId,TypeName,TypeMax,TypeUnique,TypeExtras,isActive")] IngredientType ingredientType)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            if (ModelState.IsValid)
            {
                db.Entry(ingredientType).State = EntityState.Modified;
                db.SaveChanges();

                if (Request.IsAjaxRequest())
                {
                    var success = true;
                    var message = "Ingredienstypen " + ingredientType.TypeName + " ble endret.";
                    return RedirectToAction("showIngredientTypes", "ProductManagement", new { success = success, ingredientTypeId = ingredientType.TypeId, message = message,k=k });

                }

                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var success = false;
                var message = "Oops, det oppstod en feil og ingredienstypen " + ingredientType.TypeName + " ble ikke endret.";
                return RedirectToAction("showIngredientTypes", "ProductManagement", new { success = success, ingredientTypeId = ingredientType.TypeId, message = message });

            }
            return View(ingredientType);
        }

        // GET: Admin/IngredientTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientType ingredientType = db.IngredientType.Find(id);
            if (ingredientType == null)
            {
                return HttpNotFound();
            }
            return View(ingredientType);
        }

        // POST: Admin/IngredientTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IngredientType ingredientType = db.IngredientType.Find(id);
            db.IngredientType.Remove(ingredientType);
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                var success = true;
                var message = "Ingredienstypen " + ingredientType.TypeName + " ble slettet.";
                return RedirectToAction("showIngredientTypes", "ProductManagement", new { success = success, ingredientTypeId = ingredientType.TypeId, message = message });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deactivate(int id)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            IngredientType ingredientType = db.IngredientType.Find(id);
            var active = ingredientType.isActive;
            var method = "";
            if (active)
            {
                ingredientType.isActive = false;
                method = " deaktivert.";
            }
            else
            {
                ingredientType.isActive = true;
                method = " aktivert.";
            }
            db.Entry(ingredientType).State = EntityState.Modified;
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                var success = true;
                var message = "Ingredienstypen " + ingredientType.TypeName + method;
                return RedirectToAction("showIngredientTypes", "ProductManagement", new { success = success, ingredientTypeId = ingredientType.TypeId, message = message, k=k });
            }
            return RedirectToAction("Index");
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
