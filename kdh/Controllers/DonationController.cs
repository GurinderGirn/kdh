﻿using kdh.Models;
using kdh.Utils;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kdh.Controllers
{
    public class DonationController : Controller
    {
        HospitalContext db = new HospitalContext();
        // GET: Donation
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            try
            {
                List<DonationContact> donors = db.DonationContacts.ToList();
                return View(donors);
            }
            catch (Exception ex)
            {
                ViewBag.ExceptionMessage = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: Donation/Details/5
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Details(int id)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(id.ToString()))
                {
                    return RedirectToAction("Index");
                }
                // get data from db
                DonationContact donor = db.DonationContacts.SingleOrDefault(q => q.DonorId == id);
                return View(donor);
            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");
        }

        // GET: Donation/Create
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ExceptionMessage = ex.Message;
            }
            return View("~/Views/Errors/Details.cshtml");
        }

        // POST: Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonationContact donor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    donor.DonationDate = DateTime.Now;
                    db.DonationContacts.Add(donor);
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                return View(donor);
            }
            catch (DbUpdateException uex)
            {
                ViewBag.DbExceptionMessage = ErrorHandler.DbUpdateHandler(uex);
            }
            catch (Exception ex)
            {
                ViewBag.ExceptionMessage = ex.Message;
            }
            return View("~/Views/Errors/Details.cshtml");
        }

        // GET: Donation/Edit/5
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("Index");
                }
                DonationContact donor = db.DonationContacts.SingleOrDefault(c => c.DonorId == id);
                if (donor == null)
                {
                    return RedirectToAction("Index");
                }
                return View(donor);
            }
            catch (Exception genericException)
            {
                ViewBag.ExceptionMessage = genericException.Message;
            }

            return View("~/Views/Errors/Details.cshtml");
        }

        // POST: Donation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DonationContact donor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(donor).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(donor);
            }
            catch (DbUpdateException uex)
            {
                ViewBag.DbExceptionMessage = ErrorHandler.DbUpdateHandler(uex);
                //ViewBag.DbExceptionMessage = uex.Message;
            }
            catch (SqlException sqlException)
            {
                ViewBag.SqlExceptionMessage = sqlException.Message;
            }
            catch (Exception genericException)
            {
                ViewBag.ExceptionMessage = genericException.Message;
            }
            return View("~/Views/Errors/Details.cshtml");
        }

        // GET: Donation/Delete/5
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            try
            {
                DonationContact donor = db.DonationContacts.SingleOrDefault(c => c.DonorId == id);
                if (donor == null)
                {
                    return RedirectToAction("Index");
                }
                return View(donor);
            }
            catch (SqlException sqlException)
            {
                ViewBag.SqlExceptionMessage = sqlException.Message;
            }
            catch (Exception genericException)
            {
                ViewBag.ExceptionMessage = genericException.Message;
            }
            return View("~/Views/Errors/Details.cshtml");
        }

        // POST: Donation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FormCollection form)
        {
            try
            {
                int id = Convert.ToInt32(form["Donorid"]); //This is getting from an input inside <form> with Name="Id"
                DonationContact donor = db.DonationContacts.Find(id);
                db.DonationContacts.Remove(donor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbException)
            {
                ViewBag.DbExceptionMessage = ErrorHandler.DbUpdateHandler(dbException);
            }
            catch (SqlException sqlException)
            {
                ViewBag.SqlExceptionMessage = sqlException.Message;
            }
            catch (Exception genericException)
            {
                ViewBag.ExceptionMessage = genericException.Message;
            }
            return View("~/Views/Errors/Details.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult FindDonors(string DonorName)
        {
            try
            {
                // goes to view. now it's empty
                List<DonationContact> donors = new List<DonationContact>();
                if (!String.IsNullOrEmpty(DonorName))
                {
                    // patients (and users) from database with values
                    donors = db.DonationContacts.ToList().FindAll(q => q.FirstName.ToLower().Contains(DonorName) || q.LastName.ToLower().Contains(DonorName));
                    int count = donors.Count();
                    ViewBag.CountResult = count + " Donor(s) are found.";
                }
                else
                {
                    donors = db.DonationContacts.ToList();
                    ViewBag.CountResult = " Displaying all Donors.";
                    ViewBag.SearchError = "Please enter a search keyword.";
                }
                return View(donors.OrderBy(q => q.FirstName));
            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }

            return View("~/Views/Errors/Details.cshtml");

        }
        public ActionResult MakeDonation()
        {
            string stripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            ViewBag.StripePublishKey = stripePublishKey;
            return View();
        }
        public ActionResult Charge(string stripeEmail, string stripeToken, DonationContact donor)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    donor.DonationDate = DateTime.Now;
                    int amount = Convert.ToInt32(donor.Amount * 100);
                    ViewBag.Email = stripeEmail;
                    ViewBag.Token = stripeToken;
                    StripeCustomerService customers = new StripeCustomerService();
                    StripeChargeService charges = new StripeChargeService();

                    //create new customer
                    StripeCustomer customer = customers.Create(new StripeCustomerCreateOptions
                    {
                        Email = stripeEmail,
                        SourceToken = stripeToken
                    });

                    ViewBag.customerId = customer.Id;
                    //Create a charge
                    StripeCharge charge = charges.Create(new StripeChargeCreateOptions
                    {
                        Amount = amount,
                        Description = "Sample charge",
                        Currency = "usd",
                        CustomerId = customer.Id
                    });
                    
                }
        
            return View();
            }
            catch (Exception ex)
            {
                ViewBag.ExceptionMessage = ex.Message;
            }
            return View("~/Views/Errors/Details.cshtml"); 
            /* try
             {
                 if (ModelState.IsValid)
                 {
                     donor.DonationDate = DateTime.Now;
                     int amount = Convert.ToInt32(donor.Amount * 100);
                     ViewBag.Email = stripeEmail;
                     ViewBag.Token = stripeToken;
                     StripeCustomerService customers = new StripeCustomerService();
                     StripeChargeService charges = new StripeChargeService();

                     //create new customer
                     StripeCustomer customer = customers.Create(new StripeCustomerCreateOptions
                     {
                         Email = stripeEmail,
                         SourceToken = stripeToken
                     });
                     ViewBag.customerId = customer.Id;

                     //Create a charge
                     StripeCharge charge = charges.Create(new StripeChargeCreateOptions
                     {
                         Amount = amount,
                         Description = "Sample charge",
                         Currency = "usd",
                         CustomerId = customer.Id
                     });

                     ViewBag.chargeId = charge.Id;
                     donor.PaymentId = ViewBag.chargeId;
                     db.DonationContacts.Add(donor);
                     db.SaveChanges();
                 }
                 return View();
             }
             catch (Exception ex)
             {
                 ViewBag.ExceptionMessage = ex.Message;
             }
             return View("~/Views/Errors/Details.cshtml");*/
        }


    }
}
