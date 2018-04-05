﻿using kdh.Utils;
using kdh.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kdh.ViewModels;
using System.Web.Security;

namespace kdh.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        HospitalContext context = new HospitalContext();

        public ActionResult Index()
        {
            // Todo: decide which page will be the top
            return RedirectToAction("PatientList");
        }

        // GET: Registration
        public ActionResult PatientList()
        {

            try
            {
                string sessionId = Session["id"].ToString();

                string userAccount = context.Users.Where(q => q.Id.ToString() == sessionId).SingleOrDefault().Email;
                ViewBag.UserEmail = userAccount;

                // patients (and users) from database with values
                List<Patient> patients = context.Patients.ToList();

                // goes to view. now it's empty
                List<PatientVM> patientsVM = new List<PatientVM>();

                // assign each patient data from db to patientsVM(list)
                foreach (var p in patients)
                {
                    // assigning required field
                    PatientVM patientVM = new PatientVM
                    {
                        Id = p.Id,
                        FullName = p.FirstName + " " + p.LastName,
                        Gender = p.Gender,
                    };

                    // assign if patient has an User account
                    if (p.User != null)
                    {
                        patientVM.Email = p.User.Email;
                        patientVM.UserId = p.User.Id;
                    }
                    else patientVM.Email = "N/A";

                    patientsVM.Add(patientVM);
                }

                return View(patientsVM.OrderBy(q => q.FullName));

            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }

            return View("~/Views/Errors/Details.cshtml");
        }

        // Read a speciific patient 
        [HttpGet]
        public ActionResult PatientDetails(Guid id)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(id.ToString()))
                {
                    return RedirectToAction("PatientList");
                }

                // get data from db
                Patient patient = context.Patients.SingleOrDefault(q => q.Id == id);

                if (patient != null)
                {
                    // assigining value from db to VM
                    PatientRegistrationVM registrationVM = new PatientRegistrationVM();
                    registrationVM.FirstName = patient.FirstName;
                    registrationVM.LastName = patient.LastName;
                    registrationVM.Gender = patient.Gender;
                    registrationVM.HealthCardNumber = (String.IsNullOrEmpty(patient.HealthCardNumber)) ? "N/A" : patient.HealthCardNumber;
                    registrationVM.Address1 = (String.IsNullOrEmpty(patient.Address1)) ? "N/A" : patient.Address1;
                    registrationVM.Address2 = (String.IsNullOrEmpty(patient.Address1)) ? "N/A" : patient.Address2;
                    registrationVM.City = (String.IsNullOrEmpty(patient.City)) ? "N/A" : patient.City;
                    registrationVM.Province = (String.IsNullOrEmpty(patient.Province)) ? "N/A" : patient.Province;
                    registrationVM.PostalCode = (String.IsNullOrEmpty(patient.PostalCode)) ? "N/A" : patient.PostalCode;
                    registrationVM.DateOfBirth = patient.DateOfBirth;
                    registrationVM.Phone = (String.IsNullOrEmpty(patient.Phone)) ? "N/A" : patient.Phone;
                    registrationVM.Email = "N/A";

                    if (patient.User != null)
                    {
                        registrationVM.Email = patient.User.Email;
                    }



                    ViewBag.Id = id;
                    return View(registrationVM);
                }

                return RedirectToAction("PatientList");

            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");

        }

        // Add a patients
        [HttpGet]
        public ActionResult AddPatient()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPatient(PatientRegistrationVM registrationVM) // represents what user submitted through the form
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Guid patientId = Guid.NewGuid();
                    Guid userId = Guid.NewGuid();

                    // Create an object of Patient
                    Patient p = new Patient
                    {
                        Id = patientId,
                        FirstName = registrationVM.FirstName,
                        LastName = registrationVM.LastName,
                        HealthCardNumber = registrationVM.HealthCardNumber,
                        Gender = registrationVM.Gender,
                        Address1 = registrationVM.Address1,
                        Address2 = registrationVM.Address2,
                        City = registrationVM.City,
                        Province = registrationVM.Province,
                        PostalCode = registrationVM.PostalCode,
                        DateOfBirth = registrationVM.DateOfBirth,
                        Phone = registrationVM.Phone,
                        EmailToken = TokenGenerator.GenerateEmailToken(),
                        Status = "active"
                    };


                    // If email address is provided, create a user account
                    if (!String.IsNullOrWhiteSpace(registrationVM.Email))
                    {
                        User u = new User
                        {
                            Id = userId,
                            Email = registrationVM.Email,
                            Role = "patient"
                        };
                        p.UserId = userId;

                        Mailer.SendEmail(u.Email, p.EmailToken);

                        context.Users.Add(u);
                    }

                    context.Patients.Add(p);

                    context.SaveChanges();
                    return RedirectToAction("PatientList");
                }

                return View(registrationVM);

            }
            catch (DbUpdateException DbException)
            {
                ViewBag.DbExceptionMessage = ErrorHandler.DbUpdateHandler(DbException);
            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");

        }

        // Update a Patient
        [HttpGet]
        public ActionResult EditPatient(Guid id)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(id.ToString()))
                {
                    return RedirectToAction("PatientList");
                }

                // get data from db
                Patient patient = context.Patients.SingleOrDefault(q => q.Id == id);

                if (patient != null)
                {
                    // assigining value from db to VM
                    PatientRegistrationVM registrationVM = new PatientRegistrationVM
                    {
                        FirstName = patient.FirstName,
                        LastName = patient.LastName,
                        HealthCardNumber = patient.HealthCardNumber,
                        Gender = patient.Gender,
                        Address1 = patient.Address1,
                        Address2 = patient.Address2,
                        City = patient.City,
                        Province = patient.Province,
                        PostalCode = patient.PostalCode,
                        DateOfBirth = patient.DateOfBirth,
                        Phone = patient.Phone,
                    };

                    if (patient.User != null)
                    {
                        registrationVM.Email = patient.User.Email;
                    }

                    ViewBag.Id = id;
                    return View(registrationVM);
                }

                return RedirectToAction("PatientList");

            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPatient(PatientRegistrationVM registrationVM, Guid id) // id in patients table
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // get data from db and assign new value to patient obj
                    Patient patient = context.Patients.SingleOrDefault(q => q.Id == id);
                    patient.FirstName = registrationVM.FirstName;
                    patient.LastName = registrationVM.LastName;
                    patient.HealthCardNumber = registrationVM.HealthCardNumber;
                    patient.Gender = registrationVM.Gender;
                    patient.Address1 = registrationVM.Address1;
                    patient.Address2 = registrationVM.Address2;
                    patient.City = registrationVM.City;
                    patient.Province = registrationVM.Province;
                    patient.PostalCode = registrationVM.PostalCode;
                    patient.DateOfBirth = registrationVM.DateOfBirth;
                    patient.Phone = registrationVM.Phone;

                    context.SaveChanges();
                    return RedirectToAction("PatientList");
                }

                return RedirectToAction("PatientList");

            }
            catch (DbUpdateException DbException)
            {
                ViewBag.DbExceptionMessage = ErrorHandler.DbUpdateHandler(DbException);
            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");

        }

        // DeleteUser Patient's Portal Account
        [HttpGet]
        public ActionResult DeleteUser(Guid id) // id in users table
        {
            try
            {
                if (String.IsNullOrWhiteSpace(id.ToString()))
                {
                    return RedirectToAction("PatientList");
                }

                User u = context.Users.SingleOrDefault(q => q.Id == id);
                Patient p = context.Patients.SingleOrDefault(q => q.UserId == id);


                if (p != null)
                {
                    // assigining value from db to VM
                    PatientVM patientVM = new PatientVM
                    {
                        FullName = p.FirstName + " " + p.LastName,
                    };

                    if (p.User != null)
                    {
                        patientVM.Email = p.User.Email;
                    }

                    ViewBag.Id = id;
                    return View(patientVM);
                }

                return RedirectToAction("PatientList");

            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }

            return View("~/Views/Errors/Details.cshtml");


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(PatientRegistrationVM registrationVM, Guid id) // id in users table
        {
            try
            {
                User u = context.Users.SingleOrDefault(q => q.Id == id);
                Patient p = context.Patients.SingleOrDefault(q => q.UserId == id);

                context.Users.Remove(u);
                p.UserId = null;
                context.SaveChanges();

                return RedirectToAction("PatientList");
            }
            catch (DbUpdateException DbException)
            {
                ViewBag.DbExceptionMessage = ErrorHandler.DbUpdateHandler(DbException);
            }
            catch (Exception e)
            {
                ViewBag.ExceptionMessage = e.Message;
            }
            return View("~/Views/Errors/Details.cshtml");

        }

        // Remote Validation
        // return false if email adress is in use
        [HttpPost]
        public JsonResult IsAvailableEmail(string email)
        {
            bool result = context.Users.Any(q => q.Email.ToLower() == email.ToLower());
            return Json(!result);
        }
    }
}