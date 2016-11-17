using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aeero.Models;
using Newtonsoft.Json;

namespace Aeero.Areas.Customer.Controllers
{
    class StripeErrorMessage
    {
        public string message { get; set; }
    }
    class StripeError
    {
        public StripeErrorMessage error { get; set; }
    }

    class JsonError
    {
        public string Error { get; set; }
    }

    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customer/Order
        public ActionResult Index()
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
                    Name = i.Ingredient.Name,
                    PriceSmall = i.Ingredient.PriceSmall,
                    PriceMedium = i.Ingredient.PriceMedium,
                    PriceLarge = i.Ingredient.PriceLarge,
                }),
                FileId = m.Files.FirstOrDefault() == null ? 0 : m.Files.FirstOrDefault().FileId
            }).ToList();

            ViewBag.IngredientTypes = db.IngredientType.OrderBy(
                m => m.TypeExtras.ToString() + m.TypeName
            ).Where(i => i.isActive).Select(m => new
            {
                Id = m.TypeId,
                Name = m.TypeName,
                Unique = m.TypeUnique,
                Max = m.TypeMax,
                Extra = m.TypeExtras,
                Ingredients = m.Ingredients.Where(i => i.isActive).Select(i => new
                {
                    Id = i.IngredientId,
                    Name = i.Name,
                    PriceSmall = i.PriceSmall,
                    PriceMedium = i.PriceMedium,
                    PriceLarge = i.PriceLarge,
                })
            }).ToList();

            ViewBag.Hours = db.Hours.ToList();

            var contactEtc = db.Contact.First();
            ViewBag.FixedDeliveryPrice = contactEtc.FixedPriceDelivery;
            ViewBag.StripePublicKey = contactEtc.StripePublicKey;

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

            // Validate order viewmodel:
            var errors = orderVm.Validate();
            if (errors.Count > 0)
            {
                Response.StatusCode = 400;
                return Json(new JsonError { Error = String.Join(" ", errors) });
            }


            var order = orderVm.ToOrder();

            // Validate delivery time server side:
            var weekDay = ((int)order.DeliveryTime.DayOfWeek+6)%7;
            var hours = db.Hours.ToList()[weekDay];
            var startTime = hours.OpeningHours;
            var endTime = hours.ClosingHours;
            if (order.Deliver)
            {
                startTime = hours.DeliveryStart;
                endTime = hours.DeliveryEnd;
            }

            if (startTime == null || endTime == null)
            {
                Response.StatusCode = 400;
                return Json(new JsonError { Error = "Ingen tidspunkt mottatt." });
            }
            var t1 = startTime.Value.Hours * 100 + startTime.Value.Minutes;
            var t2 = endTime.Value.Hours * 100 + endTime.Value.Minutes;

            var checkTime = order.DeliveryTime.Hour*100 + order.DeliveryTime.Minute;
            if (checkTime < t1 || checkTime > t2)
            {
                Response.StatusCode = 400;
                return Json(new JsonError { Error = "Ugyldig leveringstidspunkt." });
            }

            // DANGER, Will Robinson! DANGER! We need to set Discount to 0 so the customer can't get free pizzas!
            // Discount should only be set from the touch panel by employees, but we use the same OrderViewModel.
            order.Discount = 0;

            db.Order.Add(order);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                Response.StatusCode = 400;
                return Json(new JsonError { Error = "Kunne ikke lagre ordre." });
            }

            if (orderVm.PaymentToken != null)
            {
                return ProcessOrderPayment(order, orderVm);
            }

            if (orderVm.SMSCode != null)
            {
                return ProcessOrderSMS(order, orderVm);
            }

            // None of the above confirmation methods detected, so invalid request
            Response.StatusCode = 400;
            return Json(new JsonError { Error = "Ugyldig handling." });
        }

        private ActionResult ProcessOrderPayment(Order order, OrderViewModel orderVm) {
            // Process payment.
            var client = new WebClient();

            var data = new NameValueCollection();
            data["amount"] = ((int)(orderVm.CalculatePrice()*100)).ToString(CultureInfo.InvariantCulture); // Stripe charges are øre-based in NOK, so 100x the price.
            data["currency"] = "nok";
            data["source"] = orderVm.PaymentToken;
            data["description"] = "Ordre-ID: " + order.Id;
            data["receipt_email"] = orderVm.Email;

            client.UseDefaultCredentials = true;
            
            var contactEtc = db.Contact.First();
            client.Credentials = new NetworkCredential(contactEtc.StripePrivateKey, "");
            
            try
            {
                client.UploadValues("https://api.stripe.com/v1/charges", "POST", data);
                //responseString = client.Encoding.GetString(response);
            }
            catch (WebException exception)
            {
                string responseString;
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }

                var respJson = new StripeError();
                JsonConvert.PopulateObject(responseString, respJson);
                Response.StatusCode = 400;
                return Json(new JsonError { Error = respJson.error.message });
            }

            // If we got this far, there were no errors, and we set the order to paid, and resave.
            order.IsPaid = true;
            order.State = order.NextState();
            db.SaveChanges();

            return Json(new
            {
                ReceiptKey = order.ReceiptKey
            });
        }

        private ActionResult ProcessOrderSMS(Order order, OrderViewModel orderVm)
        {

            var codes = db.PhoneCode.Where(m => m.Number == orderVm.Phone && DbFunctions.DiffHours(m.Created, DateTime.UtcNow) < 12).ToList();

            if (codes.Count == 0)
            {
                Response.StatusCode = 400;
                return Json(new JsonError { Error = "Ingen SMS-koder funnet for ditt telefonnummer." });
            }

            var code = codes[0];
            if (code.Code != orderVm.SMSCode)
            {
                Response.StatusCode = 400;
                return Json(new JsonError { Error = "SMS-kode er feil. Vennligst prøv igjen." });
            }

            order.State = order.NextState();
            db.SaveChanges();

            return Json(new
            {
                ReceiptKey = order.ReceiptKey
            });
        }

        [HttpPost]
        public ActionResult SendConfirmationCode(string phoneNumber)
        {
            // Check if number is valid
            if (phoneNumber.Length != 8)
            {
                Response.StatusCode = 400;
                return Json(new JsonError { Error = "Ugyldig telefonnummer." });
            }

            // Check if already verified
            var existingCodes = db.PhoneCode.Count(m => m.Number == phoneNumber && DbFunctions.DiffHours(m.Created, DateTime.UtcNow) < 12);
            if (existingCodes != 0)
            {
                Response.StatusCode = 400;
                return Json(new JsonError { Error = "SMS for bekreftelse er allerede sendt." });
            }

            var phoneCode = new PhoneCode
            {
                Number = phoneNumber,
                Code = PhoneCode.GeneratedCode(),
                Created = DateTime.UtcNow
            };

            db.PhoneCode.Add(phoneCode);
            db.SaveChanges();

            try
            {
                Twilio.SendMessage(phoneNumber,
                    "Vennligst bruk kode " + phoneCode.Code + " for å bekrefte din pizzabestilling.");
            } catch(InvalidOperationException e)
            {
                db.PhoneCode.Remove(phoneCode); // Delete the phone code again since it can't be sent.
                Response.StatusCode = 400;
                return Json(new JsonError { Error = "SMS for bekreftelse er allerede sendt, eller telefonnummeret er ugyldig." });
            }

            return Json(new
            {
                Success = true,
                Code = phoneCode.Code // Just for debugging, SMS should be sent!
            });
        }

        [HttpGet]
        public ActionResult Receipt(string key, bool thanks)
        {
            ViewBag.Thanks = thanks;
            var order = db.Order.Where(o => o.ReceiptKey == key).FirstOrDefault();
            order.share = "https://www."+Request.Url.Host+ "/Customer/Order/Index";
            var twitter = new TwitterModel();
            if (db.Twitter.Find(1) != null)
            {
                order.Twitter = db.Twitter.Find(1);
            }
            if (order != null) {
                return View(order);
            }
            return null;
        }
    }
}