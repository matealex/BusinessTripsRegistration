﻿using System;
using System.Web.Mvc;
using BusinessTrips.DataAccessLayer;
using BusinessTrips.Models;
using BusinessTrips.Services;

namespace BusinessTrips.Controllers
{
    public class UserOperationsController : Controller
    {
        [HttpPost]
        public ActionResult Register(UserRegistrationModel userRegistrationModel)
        {
            userRegistrationModel.Save();

            string message = GenerateMessage(userRegistrationModel.RegisterToken);

            Email email = new Email();
            email.SendConfirmatioEmail(userRegistrationModel.Email, message);
            return View("RegisterMailSent");
        }

        private string GenerateMessage(Guid registerTokenGuid)
        {
            string link = "http://"+System.Web.HttpContext.Current.Request.Url.Host;
            link += ":" + System.Web.HttpContext.Current.Request.Url.Port;
            link += "/UserOperations/ConfirmRegistration/?guid=" + registerTokenGuid;
            return link;
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        public ActionResult ConfirmRegistration(string guid)
        {
            RegistrationConfirmationModel registrationConfirmationModel = new RegistrationConfirmationModel();
            registrationConfirmationModel.RequestToken = Guid.Parse(guid);

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