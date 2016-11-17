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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace Aeero.Areas.Admin
{
    [Authorize(Roles = "Admin, Manager")]
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Contacts
        public ActionResult Index()
        {
            var contactModel = new Contact();
            var hoursModel = new Hours();
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
            var pmViewModel = new PRpartial()
            {
                Contact = contactModel,
                Hours = hoursModel,
                Facebook = facebook,
                Twitter = twitter,
            };

            pmViewModel.IndexContact = GetContactDetails();
            pmViewModel.IndexHours = GetHoursDetails();

            return View(pmViewModel);
        }


        public IEnumerable<Contact> GetContactDetails()
        {
            return (from i in db.Contact.AsEnumerable()
                    select new Contact()
                    {
                        ContactId = i.ContactId,
                        Phone = i.Phone,
                        Email = i.Email,
                        Address = i.Address,
                        ZipCode = i.ZipCode,
                        City = i.City,
                        FixedPriceDelivery = i.FixedPriceDelivery,
                        StripePublicKey = i.StripePublicKey,
                        StripePrivateKey = i.StripePrivateKey,
                        TwilioAccountSid = i.TwilioAccountSid,
                        TwilioAuthToken = i.TwilioAuthToken
                    }).ToList();
        }

        public IEnumerable<Hours> GetHoursDetails()
        {
            return (from i in db.Hours.AsEnumerable()
                    select new Hours()
                    {
                        HoursId = i.HoursId,
                        Day = i.Day,
                        OpeningHours = i.OpeningHours,
                        ClosingHours = i.ClosingHours,
                        DeliveryStart = i.DeliveryStart,
                        DeliveryEnd = i.DeliveryEnd
                    }).ToList();
        }

        // POST: Admin/Contacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactEdit([Bind(Include = "ContactId,Phone,Email,Address,ZipCode,City,FixedPriceDelivery,StripePublicKey,StripePrivateKey,TwilioAuthToken,TwilioAccountSid")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult HoursEdit([Bind(Include = "HoursId,Day,OpeningHours,ClosingHours,DeliveryStart,DeliveryEnd")] Hours hours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return null;
        }

        // POST: Admin/Contacts/HoursCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoursCreate([Bind(Include = "Day,OpeningHours,ClosingHours,DeliveryStart,DeliveryEnd")] Hours hours)
        {
            if (ModelState.IsValid)
            {
                db.Hours.Add(hours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return null;
        }

        // POST: Admin/Contacts/HoursDelete/5
        [HttpPost]
        public ActionResult HoursDelete(int id)
        {
            Debug.WriteLine(id);
            Hours hours = db.Hours.Find(id);
            db.Hours.Remove(hours);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditUrl([Bind(Include = "Id,Url,isActive")] FacebookModel facebook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facebook).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        [HttpPost]
        public ActionResult AddUrl([Bind(Include = "Id,Url,isActive")] FacebookModel facebook)
        {
            facebook.isActive = true;
            if (ModelState.IsValid)
            {
                db.Facebook.Add(facebook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeactivateFacebook(int Id)
        {
    
            FacebookModel facebook = db.Facebook.Find(Id);
            var active = facebook.isActive;
            if (active)
            {
                facebook.isActive = false;
            }
            else
            {
                facebook.isActive = true;
            }

            db.Entry(facebook).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditTwitterUrl([Bind(Include = "Id,Url,UserName,isActive")] TwitterModel twitter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(twitter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        [HttpPost]
        public ActionResult AddTwitterUrl([Bind(Include = "Id,Url,UserName,isActive")] TwitterModel twitter)
        {
            twitter.isActive = true;
            if (ModelState.IsValid)
            {
                db.Twitter.Add(twitter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return null;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeactivateTwitter(int Id)
        {

            TwitterModel Twitter = db.Twitter.Find(Id);
            var active = Twitter.isActive;
            if (active)
            {
                Twitter.isActive = false;
            }
            else
            {
                Twitter.isActive = true;
            }

            db.Entry(Twitter).State = EntityState.Modified;
            db.SaveChanges();
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
