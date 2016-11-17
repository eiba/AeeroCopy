using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aeero.Models;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Aeero.Areas.Admin
{
    [Authorize(Roles = "Admin, Manager")]
    public class IngredientController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Ingredient
        public ActionResult Index()
        {
            return View(db.Ingredient.ToList());
        }

        // GET: Admin/Ingredient/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredient.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // GET: Admin/Ingredient/Create
        public ActionResult Create()
        {
            ViewBag.IngredientTypes = db.IngredientType.ToList();
            return View();
        }

        // POST: Admin/Ingredient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Vegetarian,PriceSmall,PriceMedium,PriceLarge,TypeId,isActive")] Ingredient ingredient)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            if (ModelState.IsValid)
            {
                db.Ingredient.Add(ingredient);
                db.SaveChanges();

                if (Request.IsAjaxRequest())
                {
                    var success = true;
                    var message = "Ingrediensen " + ingredient.Name + " ble opprettet.";
                    return RedirectToAction("showIngredients", "ProductManagement", new { success = success, ingredientId = ingredient.IngredientId, message = message,k=k });
                }

                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var success = false;
                var message = "Oops, det oppstod en feil og ingrediensen " + ingredient.Name + " ble ikke oppretet";
                return RedirectToAction("showIngredients", "ProductManagement", new { success = success, ingredientId = ingredient.IngredientId, message=message,k=k });
            }

            return View(ingredient);
        }

        // GET: Admin/Ingredient/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.IngredientTypes = db.IngredientType.ToList();
            Ingredient ingredient = db.Ingredient.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // POST: Admin/Ingredient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IngredientId,Name,Vegetarian,PriceSmall,PriceMedium,PriceLarge,TypeId,isActive")] Ingredient ingredient)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            if (ModelState.IsValid)
            {
                db.Entry(ingredient).State = EntityState.Modified;
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    var success = true;
                    var message = "Ingrediensen " + ingredient.Name + " ble endret.";
                    return RedirectToAction("showIngredients", "ProductManagement", new { success = success, ingredientId = ingredient.IngredientId, message = message,k=k });
                }
                return RedirectToAction("Index");
            }
            if (Request.IsAjaxRequest())
            {
                var success = false;
                var message = "Oops, det oppstod en feils slik at ingrediensen " + ingredient.Name + " ble ikke endret.";
                return RedirectToAction("showIngredients", "ProductManagement", new { success = success, ingredientId = ingredient.IngredientId, message = message,k=k });
            }
            return View(ingredient);
        }

        // GET: Admin/Ingredient/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredient.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // POST: Admin/Ingredient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient ingredient = db.Ingredient.Find(id);
            db.Ingredient.Remove(ingredient);
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                var success = true;
                var message = "Ingrediensen " + ingredient.Name + " ble slettet.";
                return RedirectToAction("showIngredients", "ProductManagement", new { success = success, ingredientId = ingredient.IngredientId, message = message });
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
            Ingredient ingredient= db.Ingredient.Find(id);
            var active = ingredient.isActive;
            var method = "";
            if (!ingredient.Type.isActive)
            {
                var success = false;
                var message = "Ingrediensentypen " + ingredient.Type.TypeName + " må aktiveres før ingrediensen " + ingredient.Name + " kan aktiveres.";
                return RedirectToAction("showIngredients", "ProductManagement", new { success = success, ingredientId = ingredient.IngredientId, message = message, k = k });
            }
            if (active)
            {
                ingredient.isActive = false;
                method = " deaktivert.";
            }
            else
            {
                ingredient.isActive = true;
                method = " aktivert.";
            }
            
            db.Entry(ingredient).State = EntityState.Modified;
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                var success = true;
                var message = "Ingrediensen " + ingredient.Name + method;
                return RedirectToAction("showIngredients", "ProductManagement", new { success = success, ingredientId = ingredient.IngredientId, message = message, k = k });
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
