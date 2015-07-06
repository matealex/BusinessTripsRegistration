﻿using System;
using System.Web.Mvc;
using BusinessTrips.DataAccessLayer;
using BusinessTrips.Models;
using BusinessTrips.Services;

namespace BusinessTrips.Controllers
{
    public class UserOperationsController : Controller
    {
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Register(UserRegistrationModel userRegistrationModel)
        {
            userRegistrationModel.Save();

            Email email = new Email();
            email.SendConfirmationEmail(userRegistrationModel.Email, userRegistrationModel.ID);

            return View("RegisterMailSent");
        }
      
        public ActionResult ConfirmRegistration(string guid)
        {
            RegistrationConfirmationModel registrationConfirmationModel = new RegistrationConfirmationModel();
            registrationConfirmationModel.ID = Guid.Parse(guid);

            try
            {
                registrationConfirmationModel.Confirm();
            }
            catch
            {
                return View("Register");
            }


            return View("ConfirmRegistration");
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel userModel)
        {
            UserRepository userRepository = new UserRepository();

            if (userRepository.AreCredentialsValid(userModel.Email, userModel.Password))
            {
                return View("AuthenticatedUser");
            }

            return View("UnknownUser");
        }
    }
}