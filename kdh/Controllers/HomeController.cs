﻿using kdh.Models;
using kdh.Utils;
using kdh.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace kdh.Controllers
{
    public class HomeController : Controller
    {
        HospitalContext db = new HospitalContext();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(AccountLoginVM vm)
        {
            try
            {
                string password = Hasher.ToHashedStr(vm.Password);
                var u = db.Users.SingleOrDefault(q => q.Email == vm.Email && q.Password == password);

                // if username(email) and password are correct
                if (u != null && u.Role == "admin")
                {
                    FormsAuthentication.SetAuthCookie(u.Id.ToString(), false);
                    Session["id"] = u.Id;

                    // --- temp: where do you want to redirect admin?
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username or password. Please confirm your login information.");
                }

                return View("Login");

            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");


        }

        public ActionResult Logout()
        {
            try
            {
                if (Session["id"] != null)
                {
                    Session.Abandon();
                    FormsAuthentication.SignOut();
                }
                return RedirectToAction("Login","Home");

            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");
        }


    }
}