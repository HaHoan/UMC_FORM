using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;

namespace UMC_FORM.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Form_User user, string ReturnUrl)
        {
            int userId = UserRepository.ValidateUser(user);
            var session = UserRepository.GetUser(user.CODE);
            string message = string.Empty;

            switch (userId)
            {
                case -2:// Sử dụng Pass mặc định, yêu cầu đổi pass
                    Session["user"] = session;
                    Session["userId"] = session.CODE;
                    return RedirectToAction("ChangePassword", "Account");
                case -1:// Tài khoản không tồn tại
                    message = "User Not Found !. Contact IT(3143)";
                    break;
                case 1://Tài khoản là admin ==> Chuyển sang phần quản trị
                    Session["user"] = session;
                    Session["userId"] = session.CODE;
                    return RedirectToAction("Index", "Admin");
                default:
                    Session["user"] = session;
                    Session["userId"] = session.CODE;
                    if (string.IsNullOrEmpty(ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home", new { type = SendType.SENDTOME });
                    }
                    else
                    {
                        return Redirect(ReturnUrl);
                    }
            }
            ViewBag.message = message;
            return View(user);
        }

        [CustomAuthFilter]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            //var session = Session["user"] as Form_User;

            //if (session != null)
            //{
            //    Form_User user = UserRepository.GetUser(session.CODE);
            //    return View(user);
            //}

            //if(session is null)     return RedirectToAction("Index");

            return View();
        }

        [CustomAuthFilter]
        [HttpPost]
        public ActionResult ChangePassword(string PASSWORD)
        {

            string oldPass = Request["password1"].ToString();
            if (oldPass != PASSWORD)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                var session = Session["user"] as Form_User;
                session.PASSWORD = PASSWORD;
                UserRepository.Update(session.CODE, PASSWORD);
                return RedirectToAction("Index", "Home", new { type = SendType.SENDTOME });
            }
            return View();
        }

        [HttpGet]
        [CustomAuthFilter]
        public ActionResult Infomation()
        {
            //var session = Session["user"] as Form_User;
            //if (session is null) return RedirectToAction("Index");

            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}
