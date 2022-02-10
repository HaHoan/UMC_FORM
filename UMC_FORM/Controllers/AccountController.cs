using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
        private void updateVersionJS()
        {
            try
            {
                var version = Bet.Util.Config.GetValue("version");
                string folderJS = (Server.MapPath("~/") + @"Content\js\").Replace("\\", @"\");
                string[] files = Directory.GetFiles(folderJS, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var filePath = Path.GetFileNameWithoutExtension(file);
                    var startIndex = filePath.IndexOf("[");
                    var endIndex = filePath.IndexOf("]");
                    string name = filePath;
                    if (startIndex > 0 && endIndex > 0)
                    {
                        name = filePath.Substring(0, filePath.Length - startIndex);
                        var oldVersion = filePath.Substring(startIndex, endIndex - startIndex);
                        if (oldVersion != version)
                        {
                            System.IO.File.Move(file, $"{name}[{version}].js");
                        }
                    }
                    else
                    {
                        System.IO.File.Move(file, $"{name}[{version}].js");
                    }


                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());
            }

        }
        // GET: Login
        public ActionResult Index(string returnUrl)
        {
            updateVersionJS();
            ViewBag.ReturnUrl = returnUrl;
            var account = checkCookies();
            if (account == null)
            {
                return View();
            }
            else
            {
                return View(account);

            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Form_User user, string ReturnUrl, string rememberPasswordCheck)
        {
            try
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
                        if (!string.IsNullOrEmpty(rememberPasswordCheck))
                        {
                            RememberMe(session.CODE, session.PASSWORD);
                        }
                        else
                        {
                            RemoveRememberMe();
                        }

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
            catch (Exception e)
            {

                return View();
            }
          
        }

        private Form_User checkCookies()
        {
            Form_User account = null;
            var user = string.Empty;
            var password = string.Empty;
            if (Request.Cookies["username"] != null)
            {
                user = Request.Cookies["username"].Value;
            }
            if (Request.Cookies["password"] != null)
            {
                password = Request.Cookies["password"].Value;
            }
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                account = new Form_User()
                {
                    CODE = user,
                    PASSWORD = password
                };
            }
            return account;
        }
        private void RememberMe(string username, string password)
        {
            HttpCookie ckUsername = new HttpCookie("username");
            ckUsername.Expires = DateTime.Now.AddDays(1);
            ckUsername.Value = username;
            Response.Cookies.Add(ckUsername);
            HttpCookie ckPassword = new HttpCookie("password");
            ckPassword.Expires = DateTime.Now.AddDays(1);
            ckPassword.Value = password;
            Response.Cookies.Add(ckPassword);
        }
        private void RemoveRememberMe()
        {
            if(Response.Cookies["username"] != null)
            {
                HttpCookie ckUsername = new HttpCookie("username");
                ckUsername.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckUsername);
            }
            if(Response.Cookies["password"] != null)
            {
                HttpCookie ckPassword = new HttpCookie("password");
                ckPassword.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckPassword);
            }
            
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
                if (PASSWORD == session.PASSWORD)
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
            RemoveRememberMe();
            return RedirectToAction("Index");
        }
    }
}
