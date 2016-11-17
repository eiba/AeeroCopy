using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aeero.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebGrease.Css.Extensions;

namespace Aeero.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class PizzasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
 

        // POST: Admin/Pizzas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PizzaId,SizeId,Name,PriceSmall,PriceMedium,PriceLarge,Ingredients,isActive")] Pizza pizza, HttpPostedFileBase upload)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            if (ModelState.IsValid)
            {
                // Only insert to DB ingredients which has a positive count.
                pizza.Ingredients = pizza.Ingredients.Where(m => m.Count > 0).ToList();

                db.Pizza.Add(pizza);
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                {
                    var success = true;
                    var message = "Pizzaen " + pizza.Name + " ble lagt til i databasen.";
                    return RedirectToAction("showPizzas","ProductManagement",new{success=success,pizzaId=pizza.PizzaId, message=message,k=k});
                }
                return null;
            }
            if (Request.IsAjaxRequest())
            {
                var success = false;
                var message = "Oops, det oppstod en feil og pizzaen "+pizza.Name+" ble ikke opprettet";
                return RedirectToAction("showPizzas", "ProductManagement", new { success = success, pizzaId=pizza.PizzaId, message=message,k=k});
            }

            return null;
        }

        // GET: Admin/Pizzas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pizza pizza = db.Pizza.Find(id);
            if (pizza == null)
            {
                return HttpNotFound();
            }
            

            ViewBag.IngredientTypes = db.IngredientType.ToList();

            
            ViewBag.Ingredients = db.Ingredient.ToList();
            var pizzaIngredients = pizza.Ingredients.ToList();
            ViewBag.PizzaIngredients = pizzaIngredients;

            // If this fails, there are more instances of the same ingredient added to this pizza. That means there's a bug somewhere. Don't change this line!
            ViewBag.UsedIngredientIds = pizzaIngredients.ToDictionary(pi => pi.IngredientId, pi => pi);

            return null;
        }

        // POST: Admin/Pizzas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PizzaId,SizeId,Name,PriceSmall,PriceMedium,PriceLarge,Ingredients,isActive")] Pizza pizza)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                {
                    var success = false;
                    var message = "Det oppstod en feil og pizzaen " + pizza.Name + " ble ikke endret.";
                    return RedirectToAction("showPizzas", "ProductManagement", new { success = success, pizzaId = pizza.PizzaId,message=message,k=k });
                }
                return RedirectToAction("Edit", pizza.PizzaId);
            }

            // Mark all previously saved ingredients that now have a count of 0 as deleted
            pizza.Ingredients.Where(m => m.Count == 0 && m.PizzaIngredientId != 0).ForEach(m => db.Entry(m).State = EntityState.Deleted);

            // Get the new list of active ingredients, and set as added / modified based on if it was saved previously
            pizza.Ingredients = pizza.Ingredients.Where(m => m.Count > 0).ToList();
            pizza.Ingredients.ForEach(m => db.Entry(m).State = m.PizzaIngredientId == 0 ? EntityState.Added : EntityState.Modified);

            // Set pizza state to modified
            db.Entry(pizza).State = EntityState.Modified;

            // Save and redirect
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                var success = true;
                var message = "Pizzaen " + pizza.Name + " ble endret.";
                return RedirectToAction("showPizzas", "ProductManagement", new { success = success, pizzaId = pizza.PizzaId, message= message,k=k});
            }
            return null;
        }

        // GET: Admin/Pizzas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pizza pizza = db.Pizza.Find(id);
            if (pizza == null)
            {
                return HttpNotFound();
            }
            return null;
        }

        // POST: Admin/Pizzas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pizza pizza = db.Pizza.Find(id);
            db.Pizza.Remove(pizza);
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                var success = true;
                var message = "Pizzaen " + pizza.Name + " ble slettet.";
                return RedirectToAction("showPizzas", "ProductManagement", new { success = success, pizzaId = pizza.PizzaId, message = message });
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImg(int? id, HttpPostedFileBase upload)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pizza = db.Pizza.Find(id);
            
                if (upload != null && upload.ContentLength > 0)
                {
                    if (pizza.Files.Any(f => f.FileType == FileType.Picture))
                    {
                        db.Files.Remove(pizza.Files.First(f => f.FileType == FileType.Picture));
                    }
                    var avatar = new File
                    {
                        FileName = System.IO.Path.GetFileName(upload.FileName),
                        FileType = FileType.Picture,
                        ContentType = upload.ContentType
                    };
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        avatar.Content = reader.ReadBytes(upload.ContentLength);
                    }
                    pizza.Files = new List<File> {avatar};
                }
                db.Entry(pizza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "ProductManagement");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deactivate(int id)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            Pizza Pizza = db.Pizza.Find(id);
            var active = Pizza.isActive;
            var method = "";
            if (active)
            {
                Pizza.isActive = false;
                method = " deaktivert.";
            }
            else
            {
                Pizza.isActive = true;
                method = " aktivert.";
            }

            db.Entry(Pizza).State = EntityState.Modified;
            db.SaveChanges();
            if (Request.IsAjaxRequest())
            {
                var success = true;
                var message = "Pizzaen " + Pizza.Name + method;
                return RedirectToAction("showPizzas", "ProductManagement", new { success = success, pizzaId = Pizza.PizzaId, message = message, k = k });
            }
            return RedirectToAction("Index","ProductManagement");
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
