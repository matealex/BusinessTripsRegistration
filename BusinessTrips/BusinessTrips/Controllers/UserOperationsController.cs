﻿using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BusinessTrips.DAL.Attribute;
using BusinessTrips.DAL.Exception;
using BusinessTrips.DAL.Model.User;
using BusinessTrips.DAL.Storage;
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
            if (!ModelState.IsValid)
            {
                return View("Register");
            }

            userRegistrationModel.Save();

            var email = new Email();
            email.SendUserRegistrationEmail(userRegistrationModel.Id, userRegistrationModel.Email);

            return View("RegistrationSuccessful");
        }

        public ActionResult ConfirmRegistration(string guid)
        {
            var registrationConfirmationModel = new RegistrationConfirmationModel { Id = Guid.Parse(guid) };

            registrationConfirmationModel.Confirm();

            return View("RegistrationConfirmationSuccessful");
        }

        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("GetUserBusinessTrips", "BusinessTrip");
            }
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel userModel)
        {
            if (!userModel.Authenthicate())
            {
                return View("InvalidUser");
            }

            FormsAuthentication.SetAuthCookie(userModel.Id.ToString(), false);
            return RedirectToAction("GetUserBusinessTrips", "BusinessTrip");
        }

        [RoleAuthorize(Role.Regular, Role.Hr)]
        public ActionResult Logout()
        {
            string cookieName = ConfigurationManager.AppSettings["Cookie"];

            if (Request.Cookies[cookieName] == null)
            {
                return RedirectToAction("Login");
            }

            var cookie = new HttpCookie(cookieName)
            {
                Expires = DateTime.Now.AddDays(-1d)
            };

            Response.Cookies.Add(cookie);

            return RedirectToAction("Login");
        }

        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (ModelState.IsValid)
            {
                var email = new Email();
                forgotPasswordModel = forgotPasswordModel.ToForgotPasswordModelBy(forgotPasswordModel.Email);

                if (forgotPasswordModel != null)
                {
                    email.SendForgotPasswordEmail(forgotPasswordModel.Id, forgotPasswordModel.Email);
                }

                return View("ForgotPasswordEmailSent");
            }
            return View("ForgotPassword");
        }

        public ActionResult SetNewPassword(string guid)
        {
            var userSetNewPasswordModel = new SetNewPasswordModel() { Id = Guid.Parse(guid) };
            return View("SetNewPassword", userSetNewPasswordModel);
        }

        [HttpPost]
        public ActionResult SetNewPassword(SetNewPasswordModel userSetNewPasswordModel)
        {
            if (ModelState.IsValid)
            {
                userSetNewPasswordModel.SetPassword();

                return View("PasswordSet");
            }
            return View("SetNewPassword");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is UserNotFoundException || filterContext.Exception is FormatException || filterContext.Exception is ArgumentNullException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = View("ErrorEncountered");
            }

            base.OnException(filterContext);
        }
    }
}