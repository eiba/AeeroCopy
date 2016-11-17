using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Aeero.Models;
using Microsoft.Ajax.Utilities;
using System.Text.RegularExpressions;

namespace Aeero.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class OrderHistoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var orders = from item in db.Order
                         select item;
            ViewBag.currentPage = 0;
            ViewBag.totalPages = orders.Count() / 2;

            orders = orders.OrderBy(x => x.Id).Take(2);
            
            return View(orders.ToList());
        }

        [HttpPost]
        public ActionResult Filter(string stage, string payed, string modifiedFrom, string modifiedTo, string deliveryFrom,
            string deliveryTo, string includeStage, string includePayed, string includeModified, string includeDelivery, 
            int page, string type, string search, string orderby, string direction, string canceled)
        {
            var orders = FilterQueryable(stage, payed, modifiedFrom, modifiedTo, deliveryFrom, deliveryTo, includeStage, includePayed,
                includeModified, includeDelivery, type, search, canceled);

            if (orderby.Equals("orderId"))
            {
                orders = direction.Equals("ASC") ? 
                    orders.OrderBy(x => x.Id).Skip(page * 2).Take(2) 
                    : orders.OrderByDescending(x => x.Id).Skip(page * 2).Take(2);
            }
            else if (orderby.Equals("orderCustomer"))
            {
                orders = direction.Equals("ASC") ?
                     orders.OrderBy(x => x.CustomerName).Skip(page * 2).Take(2)
                     : orders.OrderByDescending(x => x.CustomerName).Skip(page * 2).Take(2);
            }
            else if (orderby.Equals("orderStage"))
            {
                orders = direction.Equals("ASC") ?
                     orders.OrderBy(x => x.State).Skip(page * 2).Take(2)
                     : orders.OrderByDescending(x => x.State).Skip(page * 2).Take(2);
            }
            else if (orderby.Equals("orderModified"))
            {
                orders = direction.Equals("ASC")
                    ? orders.OrderBy(x => x.LastModified).Skip(page*2).Take(2)
                    : orders.OrderByDescending(x => x.LastModified).Skip(page*2).Take(2);
            }
            else
            {
                orders = orders.OrderBy(x => x.Id).Skip(page * 2).Take(2);
            }

            return PartialView("_OrderHistoryTable", orders.ToList());
        }

        [HttpGet]
        public JsonResult FilterPageCount(string stage, string payed, string modifiedFrom, string modifiedTo, string deliveryFrom,
            string deliveryTo, string includeStage, string includePayed, string includeModified, string includeDelivery, string type, string search, string canceled)
        {
            Debug.WriteLine("FilterPageCount type: " + type + ", search: " + search + " !!");
            return Json(Math.Ceiling((double) FilterQueryable(stage, payed, modifiedFrom, modifiedTo, deliveryFrom, deliveryTo, includeStage,
                    includePayed, includeModified, includeDelivery, type, search, canceled).Count() / 2), JsonRequestBehavior.AllowGet);
        }

        public IQueryable<Order> FilterQueryable(string stage, string payed, string modifiedFrom, string modifiedTo, string deliveryFrom,
            string deliveryTo, string includeStage, string includePayed, string includeModified, string includeDelivery, string type, string search, string canceled)
        {
            var orders = from item in db.Order
                         select item;

            if (!search.IsNullOrWhiteSpace())
            {
                if (type.Equals("CustomerName"))
                {
                    orders = orders.Where(x => x.CustomerName.Contains(search));
                }
                else if (type.Equals("Phone"))
                {
                    orders = orders.Where(x => x.Phone.Contains(search));
                }
                else if (type.Equals("Address"))
                {
                    orders = orders.Where(x => x.Address.Contains(search));
                }
                else if (type.Equals("ZipCode"))
                {
                    orders = orders.Where(x => x.ZipCode.Contains(search));
                }
                else if (type.Equals("Id"))
                {
                    orders = orders.Where(x => x.Id.ToString().Contains(search));
                }
            }

            if (includeStage.Equals("true") && !stage.Equals("all"))
            {
                orders = orders.Where(x => x.State == stage);
            }

            if (includeStage.Equals("true") && canceled.Equals("true"))
            {
                orders = orders.Where(x => x.Canceled);
            }

            if (includePayed.Equals("true") && !payed.Equals("Alle"))
            {
                if (payed.Equals("Betalt"))
                {
                    orders = orders.Where(x => x.IsPaid);
                }
                else if (payed.Equals("Ubetalt"))
                {
                    orders = orders.Where(x => !x.IsPaid);
                }
            }

            if (includeModified.Equals("true"))
            {
                var from = (DateTime?)DateTime.Parse(modifiedFrom);
                var to = (DateTime?)DateTime.Parse(modifiedTo);
                orders =
                    orders.Where(
                        x => Nullable.Compare<DateTime>(x.LastModified, from) >= 0
                        && Nullable.Compare<DateTime>(to, x.DeliveryTime) >= 0);
            }

            if (includeDelivery.Equals("true"))
            {
                var from = DateTime.Parse(deliveryFrom);
                var to = DateTime.Parse(deliveryTo);
                orders =
                    orders.Where(
                        x => DateTime.Compare(x.DeliveryTime, from) >= 0
                        && DateTime.Compare(to, x.DeliveryTime) >= 0);
            }

            return orders;
        }
    }
}