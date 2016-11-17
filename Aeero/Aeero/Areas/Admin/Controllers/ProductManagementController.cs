using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Aeero.Areas.Admin.Models;
using Aeero.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.Provider;

namespace Aeero.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class ProductManagementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        

        // GET: Admin/ProductManagement
        public ActionResult Index()
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var k = userr.show;
            var IngredientTypeModel = new IngredientTypeModelExtended
            {
                IngredientTypes = db.IngredientType.ToList(),
                show = k
            };
            var IngredientModel = new IngredientModelExtended
            {
                Ingredients = db.Ingredient.ToList(),
                show = k
            };
            var PizzaModel = new PizzaModelExtended
            {
                Pizzas = db.Pizza.ToList(),
                show = k
            };
            var model = new ProductsModel
            {
                Ingredients = IngredientModel,
                IngredientTypes = IngredientTypeModel,
                Pizzas = PizzaModel
        };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string type, bool show)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;

            if (type == "Pizzas" && show)
            {
                var model = new PizzaModelExtended
                {
                    Pizzas = db.Pizza.ToList(),
                    show = userr.show
                };
                return PartialView("_PizzasPartial", model);
            }
            if (type == "Ingredients" && show)
            {
                var model = new IngredientModelExtended
                {
                    Ingredients = db.Ingredient.ToList(),
                    show = userr.show
                };
                return PartialView("_IngredientsPartial", model);
            }
            if (type == "IngredientTypes" && show)
            {
                var model = new IngredientTypeModelExtended
                {
                    IngredientTypes = db.IngredientType.ToList(),
                    show = userr.show
                };
                return PartialView("_IngredientTypesPartial", model);
            }
            return PartialView("_EmptyPartial");
            
        }

        [HttpGet]
        public ActionResult showPizzaDetails(int? id)
        {
            Pizza pizza = db.Pizza.Include(s => s.Files).SingleOrDefault(s => s.PizzaId == id);

            return PartialView("_PizzaDetailsPartial",pizza);
            

        }
        [HttpGet]
        public ActionResult showPizzaEdit(int? id)
        {
            Pizza pizza = db.Pizza.Find(id);


            return PartialView("_EditPizzaPartial", pizza);


        }
        [HttpGet]
        public ActionResult showPizzaDelete(int? id)
        {
            Pizza pizza = db.Pizza.Find(id);

            return PartialView("_DeletePizzaPartial", pizza);
        }
        [HttpGet]
        public ActionResult showPizzaUpload(int? id)
        {
            Pizza pizza = db.Pizza.Include(s => s.Files).SingleOrDefault(s => s.PizzaId == id);
            return PartialView("_UploadImgPartial", pizza);
        }

        [HttpGet]
        public ActionResult showPizzaCreate()
        {
            ViewBag.IngredientTypes = db.IngredientType.ToList();
            return PartialView("_AddPizzaPartial", new Pizza());
        }

        [HttpGet]
        public ActionResult showIngredientDetails(int? id)
        {
            Ingredient ingredient = db.Ingredient.Find(id);

            return PartialView("_IngredientDetailsPartial", ingredient);
        }

        [HttpGet]
        public ActionResult showIngredientEdit(int? id)
        {
            Ingredient ingredient = db.Ingredient.Find(id);

            return PartialView("_EditIngredientPartial", ingredient);
        }

        [HttpGet]
        public ActionResult showIngredientDelete(int? id)
        {
            Ingredient ingredient = db.Ingredient.Find(id);

            return PartialView("_DeleteIngredientPartial", ingredient);
        }

        [HttpGet]
        public ActionResult showIngredientCreate()
        {

            return PartialView("_AddIngredientsPartial", new Ingredient());
        }

        [HttpGet]
        public ActionResult showIngredientTypeDetails(int? id)
        {
            IngredientType ingredientType = db.IngredientType.Find(id);

            return PartialView("_IngredientTypeDetailsPartial", ingredientType);
        }

        [HttpGet]
        public ActionResult showIngredientTypeEdit(int? id)
        {
            IngredientType ingredientType = db.IngredientType.Find(id);

            return PartialView("_EditIngredientTypePartial", ingredientType);
        }

        [HttpGet]
        public ActionResult showIngredientTypeDelete(int? id)
        {
            IngredientType ingredientType = db.IngredientType.Find(id);

            return PartialView("_DeleteIngredientTypePartial", ingredientType);
        }

        [HttpGet]
        public ActionResult showIngredientTypeCreate()
        {

            return PartialView("_AddIngredientTypesPartial", new IngredientType());
        }

        public ActionResult showPizzas(bool success,string pizzaId,string message, bool k)
        {
            var model = new PizzaModelExtended
            {
                Pizzas = db.Pizza.ToList(),
                show = k
            };
            if (success)
            {
                ViewBag.SuccessId =pizzaId;
                ViewBag.Success = message;
                return PartialView("_PizzasPartial",model);

            }
            else
            {
                ViewBag.FailId = pizzaId;
                ViewBag.Error = message;
                return PartialView("_PizzasPartial", model);
            }

        }

        public ActionResult showIngredients(bool success ,string ingredientId, string message,bool k)
        {
            var model = new IngredientModelExtended
            {
                Ingredients = db.Ingredient.ToList(),
                show = k
            };
            if (success)
            {
                ViewBag.SuccessId = ingredientId;
                ViewBag.Success = message;
                return PartialView("_IngredientsPartial", model);

            }
            else
            {
                ViewBag.FailId = ingredientId;
                ViewBag.Error = message;
                return PartialView("_IngredientsPartial", model);
            }

        }

        public ActionResult showIngredientTypes(bool success ,string ingredientTypeId, string message,bool k)
        {
            var model = new IngredientTypeModelExtended
            {
                IngredientTypes = db.IngredientType.ToList(),
                show = k
            };
            if (success)
            {
                ViewBag.SuccessId = ingredientTypeId;
                ViewBag.Success = message;
                return PartialView("_IngredientTypesPartial", model);

            }
            else
            {
                ViewBag.FailId = ingredientTypeId;
                ViewBag.Error = message;
                return PartialView("_IngredientTypesPartial", model);
            }

        }

        

        [HttpPost]
        public ActionResult EditShow(bool k, string type)
        {
            var store = new UserStore<ApplicationUser>(db);
            var manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            if (userr == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            userr.show = k;

            IdentityResult result = manager.Update(userr);
            store.Context.SaveChanges();
            if (type == "IngredientType")
            {
                return RedirectToAction("showIngredientTypes2", "ProductManagement", new { k = k });

            }
            if (type == "Ingredient")
            {
                return RedirectToAction("showIngredients2", "ProductManagement", new { k = k });
            }
            return RedirectToAction("showPizzas2", "ProductManagement", new { k = k });

        }
        public ActionResult showIngredientTypes2(bool k)
        {
            var model = new IngredientTypeModelExtended
            {
                IngredientTypes = db.IngredientType.ToList(),
                show = k
            };
            return PartialView("_IngredientTypesPartial", model);
        }

        public ActionResult showIngredients2(bool k)
        {
            var model = new IngredientModelExtended
            {
                Ingredients = db.Ingredient.ToList(),
                show = k
            };
            return PartialView("_IngredientsPartial", model);
        }

        public ActionResult showPizzas2(bool k)
        {
            var model = new PizzaModelExtended
            {
                Pizzas = db.Pizza.ToList(),
                show = k
            };
            return PartialView("_PizzasPartial", model);
        }
    }
  
}