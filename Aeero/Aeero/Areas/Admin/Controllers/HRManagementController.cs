using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Aeero.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;
using static Aeero.Controllers.ManageController;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using System.Web.UI.WebControls.Expressions;
using Aeero.Areas.Admin.Models;


namespace Aeero.Areas.Admin.Controllers
{

    [Authorize(Roles = "Admin, Manager")]
    public class HRManagementController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        // GET: Admin/HR

        public ActionResult Index(ManageMessageId? message, string search, string orsak)
        {
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            if (userr == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangeProfileSuccess ? "Profilen har blitt endret."
                : message == ManageMessageId.AddProfileSuccess ? "Brukeren har blitt lagt til i databasen"
                : message== ManageMessageId.Error ? "Ulovlig operasjon" +" "+orsak
                : "";

            var results = from s in context.Users
                          where
                          s.firstName.Contains(search) ||
                          s.lastName.Contains(search) ||
                          s.Email.Contains(search) ||
                          s.Phone.Contains(search) ||
                          s.Adress.Contains(search) ||
                          s.zipCode.Contains(search) ||
                          s.City.Contains(search) ||
                          s.UserRole.Contains(search)
                          orderby s.lastName
                          select s;

            if (search == null)
            {
                results = from s in context.Users
                orderby s.lastName
                select s;
                
            }
            
            var searchModel = new SearchModel {search = search};
            var model = new SearchUserViewModel {SearchModel = searchModel, ApplicationUsers = results};            

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string param, string k)
        {
            var check = k;
            if (check == null)
            {
                check = "true";
            }
            var results = resolveUsers(param, check);
            
            var model = new ShowModel
            {
                userss = results,
                check = check
            };

            return PartialView("_ShowUsersPartial", model);
        }

        public ActionResult RegisterEmployee()
        {
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
            if (userr == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterEmployee(RegisterViewModel model, string param)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            ApplicationUser userr = userManager.FindByIdAsync(User.Identity.GetUserId()).Result;
            if (userr == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            var ShowModel = new ShowModel
            {
                userss = resolveUsers(param, "true"),
                check = "true"
            };
            var results = from s in context.Users
                          where
                              s.Email.Contains(model.Email)
                          select s;

            if (results != null)
            {
                foreach (var r in results)
                {
                    if (r.Email == model.Email)
                    {
                        if (Request.IsAjaxRequest())
                        {
                            ViewBag.Error = "Emailen er allerede i bruk.";
                            return PartialView("_ShowUsersPartial", ShowModel);
                        }
                    }
                }
                
            }
            
            if (ModelState.IsValid)
            {           
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    firstName = model.firstName,
                    lastName = model.lastName,
                    Adress = model.Address,
                    City = model.City,
                    zipCode = model.zipCode,
                    Phone = model.Phone,
                    UserRole = model.UserRole,
                    roleNr = ResolveRoleNr(model.UserRole),
                    IsEnabeled = true
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var roleNr = userr.roleNr;
                    if (roleNr > ResolveRoleNr(model.UserRole))
                    {
                        userManager.AddToRole(user.Id, model.UserRole);
                    }
                    else
                    {
                        if (Request.IsAjaxRequest())
                        {

                            ViewBag.Error = "Du kan ikke gi brukere en rang som er like høy eller høyere enn din egen!";
                            return PartialView("_ShowUsersPartial", ShowModel);
                        }
                        return RedirectToAction("Index","HRManagement", new { Message = ManageMessageId.Error });
                    }
                    var UserName = user.firstName +" "+ user.lastName;
                    if (Request.IsAjaxRequest())
                    {
                        
                        ViewBag.Success = "Brukeren "+model.Email+" ble lagt til i databasen.";
                        ViewBag.Id = user.Id;
                        return PartialView("_ShowUsersPartial",ShowModel);
                    }
                    return RedirectToAction("Index", "HRManagement", new { Message = ManageMessageId.AddProfileSuccess, User=UserName});

                }
                
                AddErrors(result);

            }
            if (Request.IsAjaxRequest())
            {
                ViewBag.Error = "Oops, noe gikk galt, sjekk at passordene matcher kriteriene!";
                return PartialView("_ShowUsersPartial", ShowModel);
            }
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new ChangeProfile
            {
                firstName = user.firstName,
                lastName = user.lastName,
                Adress = user.Adress,
                City = user.City,
                zipCode = user.zipCode,
                Phone = user.Phone,
                UserRole = user.UserRole,
                Id = user.Id,
                Email = user.Email
            };
            
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ChangeProfile model, string param, string check)
        {
            
            var k = "";
            var checkedd = check;
            if (check == null)
            {
                checkedd = "true";
            }
            var ShowModel = new ShowModel
            {
                search = param,
                userss = resolveUsers(param, check),
                check = checkedd

            };
            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = await manager.FindByNameAsync(model.UserName);
                ApplicationUser Model = manager.FindByName(model.UserName);

                var results = from s in context.Users
                    where
                        s.Email.Contains(model.Email)
                    select s;
                if (results != null)
                {
                    foreach (var resultk in results)
                    {
                        if (resultk.Id != Model.Id)
                        {
                            if (Request.IsAjaxRequest())
                            {
                                ViewBag.Error = "Emailen er allerede i bruk.";
                                ViewBag.Id = Model.Id;
                                return PartialView("_ShowUsersPartial",ShowModel);
                            }
                            return RedirectToAction("Index", "HRManagement",
                                new {Message = ManageMessageId.Error, orsak = "email er allerede i bruk"});

                        }
                    }
                }


                ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;
                if (userr == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                }
                var roleNr = userr.roleNr;
                var modelRoleNr = ResolveRoleNr(model.UserRole);
                if ((roleNr > ResolveRoleNr(Model.UserRole) || userr.Id == Model.Id))
                {
                    if (roleNr > modelRoleNr || userr.Id == Model.Id)
                    {
                        var rolesForUser = await manager.GetRolesAsync(Model.Id);

                        if (rolesForUser.Count() > 0)
                        {
                            foreach (var item in rolesForUser.ToList())
                            {
                                await manager.RemoveFromRoleAsync(Model.Id, item);
                            }
                        }

                        manager.AddToRole(Model.Id, model.UserRole);
                    }
                    else
                    {
                        if (Request.IsAjaxRequest())
                        {
                            ViewBag.Error = "Oops! du kan ikke tildele roller høyere eller lik din egen!";
                            ViewBag.Id = Model.Id;
                            return PartialView("_ShowUsersPartial", ShowModel);
                        }
                        return RedirectToAction("Index", "HRManagement", new { Message = ManageMessageId.Error });
                    }
                }
                else
                {
                    if (Request.IsAjaxRequest())
                    {
                        ViewBag.Error = "Oops! du har ikke lov til å endre på brukere av høyere rang!";
                        ViewBag.Id = Model.Id;
                        return PartialView("_ShowUsersPartial", ShowModel);
                    }

                    return RedirectToAction("Index", "HRManagement", new {Message = ManageMessageId.Error});
                }
                

                Model.City = model.City;
                Model.Adress = model.Adress;
                Model.zipCode = model.zipCode;
                Model.Phone = model.Phone;
                Model.firstName = model.firstName;
                Model.lastName = model.lastName;
                Model.UserRole = model.UserRole;
                Model.UserName = model.Email;
                Model.roleNr = ResolveRoleNr(model.UserRole);
                Model.Email = model.Email;

                IdentityResult result = await manager.UpdateAsync(Model);
                store.Context.SaveChanges();

                if (result.Succeeded)
                { 

                    if (model.Password != null)
                    {
                        var provider = new DpapiDataProtectionProvider("Aeero");
                        manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("Passwordresetting"));
                        string resetToken = await manager.GeneratePasswordResetTokenAsync(Model.Id);
                        var Presult = await manager.ResetPasswordAsync(Model.Id, resetToken, model.Password);
                        if (!Presult.Succeeded)
                        {
                            if (Request.IsAjaxRequest())
                            {
                                ViewBag.Error = "Passordet må matche kriteriene!";
                                ViewBag.Id = Model.Id;
                                return PartialView("_ShowUsersPartial", ShowModel);
                            }
                            return RedirectToAction("Index", "HRManagement", new { Message = ManageMessageId.Error });

                        }
                    } 
                    if (Request.IsAjaxRequest())
                    {
                        ViewBag.Success = "Brukeren "+model.Email+" ble oppdatert.";
                        ViewBag.Id = model.Id;
                        return PartialView("_ShowUsersPartial", ShowModel);
                    }
                    return RedirectToAction("Index","HRManagement", new { Message = ManageMessageId.ChangeProfileSuccess});
                }
                AddErrors(result);
                
            }
            if (Request.IsAjaxRequest())
            {
                ViewBag.Error = "Oops, noe gikk galt, husk at passordene må være like og møte kriteriene!";
                ViewBag.Id = model.Id;
                return PartialView("_ShowUsersPartial", ShowModel);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Deactivate(string id)
        {
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;

            if (id == null || userr==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.roleNr >= userr.roleNr)
            {
                
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }

            user.LockoutEnabled = true;
            user.LockoutEndDateUtc = new DateTime(9999, 12, 30);
            user.IsEnabeled = false;

            await manager.UpdateAsync(user);
            context.SaveChanges();

            if (Request.IsAjaxRequest())
            {
                var model = new StatusModel
                {
                    Id = id,
                    status = "Aktiver",
                    handling = "Activate"
                };

                ViewBag.Success = "Brukeren " + user.Email + " har blitt deaktivert.";
                ViewBag.Id = user.Id;
                return PartialView("_StatusPartial", model);
            }

            return RedirectToAction("Index","HRManagement");
        }


        public async Task<ActionResult> Activate(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = context.Users.Find(id);
            ApplicationUser userr = manager.FindByIdAsync(User.Identity.GetUserId()).Result;

            if (user == null || userr==null)
            {
                return HttpNotFound();
            }
            if (user.roleNr >= userr.roleNr)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }

            user.LockoutEnabled = false;
            user.IsEnabeled = true;

            await manager.UpdateAsync(user);
            context.SaveChanges();

            if (Request.IsAjaxRequest())
            {
                var model = new StatusModel
                {
                    Id = id,
                    status = "Deaktiver",
                    handling = "Deactivate"
                };

                return PartialView("_StatusPartial", model);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser User = context.Users.Find(id);
            if (User == null)
            {
                return HttpNotFound();
            }
            return View(User);
            
        }

        public IEnumerable<ApplicationUser> resolveUsers(string param, string k)
        {
            var results = from s in context.Users
                          orderby s.lastName
                          select s;

            
            if (k == "false" && param != null)
            {
                results = from s in context.Users
                          where
                          s.firstName.Contains(param) ||
                          s.lastName.Contains(param) ||
                          s.Email.Contains(param) ||
                          s.Phone.Contains(param) ||
                          s.Adress.Contains(param) ||
                          s.zipCode.Contains(param) ||
                          s.City.Contains(param) ||
                          s.UserRole.Contains(param)
                          orderby s.lastName
                          select s;
            }

            if (k == "true" && param == null)
            {
                results = from s in context.Users
                          where
                          s.IsEnabeled.ToString().Contains("true")
                          orderby s.lastName
                          select s;
            }

            if (k == "true" && param != null)
            {
                results = from s in context.Users
                          where
                          (s.firstName.Contains(param) ||
                          s.lastName.Contains(param) ||
                          s.Email.Contains(param) ||
                          s.Phone.Contains(param) ||
                          s.Adress.Contains(param) ||
                          s.zipCode.Contains(param) ||
                          s.City.Contains(param) ||
                          s.UserRole.Contains(param)) &&
                          s.IsEnabeled.ToString().Contains("true")
                          orderby s.lastName
                          select s;

            }
            
            return results;
        } 

        public int ResolveRoleNr(string Role)
        {
            if (Role == "Manager")
            {
                return 3;
            }
            if (Role == "Admin")
            {
                return 2;
            }
            if (Role == "Ansatt")
            {
                return 1;
            }
            return 0;
        }

        public string ResolveUserRole(int roleNr)
        {
            if (roleNr == 3)
            {
                return "Manager";
            }
            if (roleNr == 2)
            {
                return "Admin";
            }
            if (roleNr == 1)
            {
                return "Ansatt";
            }
            return null;
        }

        [HttpGet]
        public ActionResult showEmployeeCreate(string param)
        {
            var model = new RegisterViewModel
            {
                param = param,

            };

            return PartialView("_RegisterEmployeePartial", model);
        }

        [HttpGet]
        public ActionResult showEmployeeDetails(string VueId, string modalId, string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = context.Users.Find(id);
            var ShowDetailsModel = new ShowDetailsModel
            {
                user = user,
                VueId = VueId,
                modalId = modalId
            };

            return PartialView("_DetailsPartial", ShowDetailsModel);
        }
        [HttpGet]
        public ActionResult showEmployeeEdit(string vueIdd, string modalIdd, string check,string search, string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = context.Users.Find(id);
            var ChangeProfileModel = new ChangeProfile
            {
                firstName = user.firstName,
                lastName = user.lastName,
                Adress = user.Adress,
                City = user.City,
                zipCode = user.zipCode,
                Phone = user.Phone,
                Email = user.Email,
                UserName = user.Email,
                Id = user.Id,
                UserRole = user.UserRole,
                vueIdd = vueIdd,
                modalIdd = modalIdd,
                check = check,
                search = search

            };

            return PartialView("_EditPartial", ChangeProfileModel);
        }
    }
}