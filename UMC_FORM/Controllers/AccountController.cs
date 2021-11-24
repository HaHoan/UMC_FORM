﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Hangfire;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;
using UMC_FORM.Ultils;
using WebMatrix.WebData;

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
        public async Task<ActionResult> Index(Form_User user, string ReturnUrl)
        {
            string message = string.Empty;
            var t1 = UserRepository.ValidateUserAsync(user);
            var t2 = UserRepository.GetUserAsync(user.CODE);
            int userId = await t1;
            var session = await t2;
            Form_Log log = null;
            switch (userId)
            {
                case -2:// Sử dụng Pass mặc định hoặc đăng nhập lần đầu, yêu cầu đổi pass
                    Session["user"] = session;
                    Session["userId"] = session.CODE;
                    log = new Form_Log()
                    {
                        IP_ADDRESS = NetworkHelper.Ip,
                        HOST_NAME = NetworkHelper.Host,
                        OPERATE_TYPE = "Đổi pass",
                        OPERATE_TIME = DateTime.Now,
                        USER_ID = session.ID,
                        USER_NAME = session.CODE,
                        DESCRIPTION = "",
                        EXECUTE_RESULT = (int)EXECUTE_RESULT.CHANGE_PASS,
                        EXECUTE_RESULT_DETAILS = ""
                    };
                    LogRepository.SaveLog(log);
                    return RedirectToAction("ChangePassword", "Account");
                case -1:// Tài khoản không tồn tại
                    message = "User Not Found !. Contact IT(3143)";
                    log = new Form_Log()
                    {
                        IP_ADDRESS = NetworkHelper.Ip,
                        HOST_NAME = NetworkHelper.Host,
                        OPERATE_TYPE = "Tài khoản không tồn tại",
                        OPERATE_TIME = DateTime.Now,
                        USER_ID = session.ID,
                        USER_NAME = session.CODE,
                        DESCRIPTION = "",
                        EXECUTE_RESULT = (int)EXECUTE_RESULT.FAILED,
                        EXECUTE_RESULT_DETAILS = "Cảnh báo spam"
                    };
                    LogRepository.SaveLog(log);
                    break;
                case 1://Tài khoản là admin ==> Chuyển sang phần quản trị
                    Session["user"] = session;
                    Session["userId"] = session.CODE;
                    log = new Form_Log()
                    {
                        IP_ADDRESS = NetworkHelper.Ip,
                        HOST_NAME = NetworkHelper.Host,
                        OPERATE_TYPE = "Đăng nhập",
                        OPERATE_TIME = DateTime.Now,
                        USER_ID = session.ID,
                        USER_NAME = session.CODE,
                        DESCRIPTION = "",
                        EXECUTE_RESULT = (int)EXECUTE_RESULT.SUCCESS,
                        EXECUTE_RESULT_DETAILS = "Admin"
                    };
                    LogRepository.SaveLog(log);
                    return RedirectToAction("Index", "Admin");
                default:
                    Session["user"] = session;
                    Session["userId"] = session.CODE;
                    log = new Form_Log()
                    {
                        IP_ADDRESS = NetworkHelper.Ip,
                        HOST_NAME = NetworkHelper.Host,
                        OPERATE_TYPE = "Đăng nhập",
                        OPERATE_TIME = DateTime.Now,
                        USER_ID = session.ID,
                        USER_NAME = session.CODE,
                        DESCRIPTION = "",
                        EXECUTE_RESULT = (int)EXECUTE_RESULT.SUCCESS,
                        EXECUTE_RESULT_DETAILS = "Normal"
                    };
                    LogRepository.SaveLog(log);
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

            try
            {
                var session = Session["user"] as Form_User;
                if(PASSWORD == session.PASSWORD)
                {
                    ViewBag.MessageError = "Không được sử dụng pass cũ!";
                    return View();
                }
                session.PASSWORD = PASSWORD;
                if (session.PASSWORD.ToUpper() == "UMCVN")
                {
                    ViewBag.MessageError = "Không sử dụng pass mặc định UMCVN";
                    return View();
                }
                UserRepository.Update(session.CODE, PASSWORD);
                var log = new Form_Log()
                {
                    IP_ADDRESS = NetworkHelper.Ip,
                    HOST_NAME = NetworkHelper.Host,
                    OPERATE_TYPE = "Đăng nhập",
                    OPERATE_TIME = DateTime.Now,
                    USER_ID = session.ID,
                    USER_NAME = session.CODE,
                    DESCRIPTION = "",
                    EXECUTE_RESULT = (int)EXECUTE_RESULT.SUCCESS,
                    EXECUTE_RESULT_DETAILS = "Normal"
                };
                LogRepository.SaveLog(log);
                return RedirectToAction("Index", "Home", new { type = SendType.SENDTOME });
            }
            catch (Exception e)
            {
                ViewBag.MessageError = "Có lỗi xảy ra!";
                return View();
            }
        
            
        }

        [HttpGet]
        [CustomAuthFilter]
        public ActionResult Infomation()
        {
            //var session = Session["user"] as Form_User;
            //if (session is null) return RedirectToAction("Index");

            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string emailAddress)
        {
            Tuple<string, string> alert = null;
            if (emailAddress is null || string.IsNullOrEmpty(emailAddress))
            {
                alert = new Tuple<string, string>("alert alert-warning", "Email not empty!");
                ViewBag.alert = alert;
                return View();
            }
            if (ModelState.IsValid)
            {
                var user = UserRepository.GetUserByMail(emailAddress);
                if (user is null)
                {
                    alert = new Tuple<string, string>("alert alert-warning", "Email not exist!");
                    ViewBag.alert = alert;
                    return View();
                }
                var exist = WebSecurity.UserExists(user.CODE);
                // var token = WebSecurity.GeneratePasswordResetToken(user.CODE);
                string token;
                try
                {
                    token = WebSecurity.GeneratePasswordResetToken(user.CODE);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("No account"))
                    {
                        string password = System.Web.Security.Membership.GeneratePassword(6, 0);
                        WebSecurity.CreateAccount(user.CODE, password);

                    }
                    token = WebSecurity.GeneratePasswordResetToken(user.CODE);
                }
                if (token is null)
                {
                    alert = new Tuple<string, string>("alert alert-danger", "Cannot create token! Contact 4143");
                    ViewBag.alert = alert;
                    return View();
                }
                var lnkHref = "<a href='" + Url.Action("ResetPassword", "Account", new { email = emailAddress, code = token }, "http") + "'>Reset Password</a>";
                //HTML Template for Send email
                string subject = "Your changed password";
                string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;
                alert = new Tuple<string, string>("alert alert-success", "Send mail success! Check it out.");
                BackgroundJob.Enqueue(() => MailHelper.SenMailOutlook(emailAddress, subject, body));
            }
            ViewBag.alert = alert;
            return View();
        }
        public ActionResult ResetPassword(string code, string email)
        {
            var model = new ResetPasswordModel();
            model.returnToken = code;
            return View(model);
        }
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            Tuple<string, string> alert = null;
            if (ModelState.IsValid)
            {
                if (model.confirmPassword != model.password)
                {
                    alert = new Tuple<string, string>("alert alert-danger", "Password are not matching");
                    ViewBag.alert = alert;
                    return View();
                }
                var userID = WebSecurity.GetUserIdFromPasswordResetToken(model.returnToken);
                bool resetResponse = WebSecurity.ResetPassword(model.returnToken, model.password);
                if (resetResponse)
                {
                    UserRepository.Update(userID, model.password);
                    alert = new Tuple<string, string>("alert alert-success", "Successfully Changed");
                }
                else
                {
                    alert = new Tuple<string, string>("alert alert-danger", "Something went horribly wrong!");
                }
                ViewBag.alert = alert;
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}
