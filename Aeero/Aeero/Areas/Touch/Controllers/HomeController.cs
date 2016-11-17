using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Aeero.Areas.Touch.Models;
using Aeero.Models;
using System.Net;
using System.Web.UI.WebControls;

namespace Aeero.Areas.Touch.Controllers
{
    [Authorize(Roles = "Ansatt, Admin, Manager")]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult NewOrder()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            ViewBag.Pizzas = db.Pizza.Where(p => p.isActive).Select(m => new
            {
                Id = m.PizzaId,
                Name = m.Name,
                PriceSmall = m.PriceSmall,
                PriceMedium = m.PriceMedium,
                PriceLarge = m.PriceLarge,
                Ingredients = m.Ingredients.Where(i => i.Ingredient.isActive).Select(i => new
                {
                    Id = i.Ingredient.IngredientId,
                    Count = i.Count,
                    Name = i.Ingredient.Name
                }),
            }).ToList();

            ViewBag.IngredientTypes = db.IngredientType.Where(p => p.isActive).Select(m => new
            {
                Id = m.TypeId,
                Name = m.TypeName,
                Unique = m.TypeUnique,
                Ingredients = m.Ingredients.Where(i => i.isActive).Select(i => new
                {
                    Id = i.IngredientId,
                    Name = i.Name,
                    PriceSmall = i.PriceSmall,
                    PriceMedium = i.PriceMedium,
                    PriceLarge = i.PriceLarge,
                })
            }).ToList();

            ViewBag.FixedDeliveryPrice = db.Contact.First().FixedPriceDelivery;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(OrderViewModel orderVm)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(ModelState.Keys.SelectMany(key => ModelState[key].Errors));
            }

            var order = orderVm.ToOrder();
            db.Order.Add(order);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    Error = "Error saving to DB."
                });
            }

            return Json(new
            {
                Id = order.Id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            var order = db.Order.Find(id);
            if (order.State == "new" || order.State == "confirmed")
            {
                order.Canceled = true;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                return Json(new
                {
                    Success = true
                });
            }

            Response.StatusCode = 400;
            return Json(new
            {
                Success = false
            });
        }

        public ActionResult Index()
        {
            var orders = db.Order.Where(o => o.Canceled == false && o.State != "done").OrderBy(o => o.DeliveryTime).ToList();
            return View(orders);
        }

        [HttpGet]
        public ActionResult ShowMap(int? id)
        {
            Order order = db.Order.Find(id);
            var resturant = db.Contact.Find(1);
            var origin = resturant.Address + ", " + resturant.ZipCode + " " + resturant.City;
            MapModel model = new MapModel
            {
                Order = order,
                OriginAdress = origin
            };
            return PartialView("_showMapPartial", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int? id, int? pizzaid, string type)
        {

            if (id == null || type == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (type == "Bekreft") // the order should move one step down the chain
            {
                order.State = order.NextState();
                foreach (var lines in order.Lines)
                {
                    lines.PizzaState = order.NextState();
                }
            }
            if (type == "Reset") // setting the order back to start. 
            {
                foreach (var lines in order.Lines)
                {
                    lines.PizzaState = order.PrevState();
                }
                order.State = order.PrevState();
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    Error = "Error saving to DB."
                });
            }
            var orders = db.Order.Where(o => o.Canceled == false && o.State != "done").OrderBy(o => o.DeliveryTime).ToList();
            return View(orders);
        }

        //[ValidateAntiForgeryToken]
        public ActionResult Preparation(bool showAll = false)
        {
            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.Canceled == false)
                .Where(o => o.State == "preparation");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }

            return View(orders.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Preparation(int? id, int? pizzaId, bool showAll = false)
        {
            if (id == null || pizzaId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            OrderLine pizza = null;
            int counter = 0;
            foreach (var lines in order.Lines) // just so that a pizza alone can enter the oven without all to be done
            {
                if (lines.Id == pizzaId)
                {
                    pizza = lines;
                    lines.PizzaState = "oven";
                }
                if (lines.PizzaState == "preparation" || lines.PizzaState == null)
                {
                    lines.PizzaState = "preparation";
                    counter++;
                }
            }
            if (counter == 0) // there are no pizzas still in preperation.
            {
                order.State = order.NextState();
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    Error = "Error saving to DB."
                });
            }
            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.Canceled == false)
                .Where(o => o.State == "preparation");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }


        public ActionResult Oven(bool showAll = false)
        {
            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.Canceled == false)
                .Where(o => o.State == "oven");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Oven(int? id, bool showAll = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.State = order.NextState();
            foreach (var lines in order.Lines) // just setting all the pizzas to ready.
            {
                lines.PizzaState = order.State;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    Error = "Error saving to DB."
                });
            }

            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.Canceled == false)
                .Where(o => o.State == "oven");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }

        public ActionResult Delivery(bool showAll = false)
        {
            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.Canceled == false)
                .Where(o => o.State == "delivery");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delivery(int? id, bool showAll = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.State = order.NextState();
            foreach (var lines in order.Lines) // just setting all the pizzas to Done.
            {
                lines.PizzaState = order.State;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    Error = "Error saving to DB."
                });
            }
            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.Canceled == false)
                .Where(o => o.State == "delivery");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }

        public ActionResult Ready(bool showAll = false)
        {
            var today = DateTime.Today;

            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.State == "ready");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ready(int? id, bool showAll = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.State = order.NextState();
            foreach (var lines in order.Lines) // just setting all the pizzas to Done.
            {
                lines.PizzaState = order.State;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    Error = "Error saving to DB."
                });
            }
            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.Canceled == false)
                .Where(o => o.State == "ready");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }


        public ActionResult Done(bool showAll = false)
        {
            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.State == "done");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Done(int? id, bool showAll = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.State = order.PrevState();
            foreach (var lines in order.Lines) // just setting all the pizzas to ready.
            {
                lines.PizzaState = order.State;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    Error = "Error saving to DB."
                });
            }

            var today = DateTime.Today;
            var orders = db.Order
                .OrderBy(o => o.DeliveryTime)
                .Where(o => o.State == "done");

            if (!showAll)
            {
                orders = orders.Where(o => o.DeliveryTime.Year == today.Year && o.DeliveryTime.Month == today.Month && o.DeliveryTime.Day == today.Day);
            }
            return View(orders.ToList());
        }



    }
}
