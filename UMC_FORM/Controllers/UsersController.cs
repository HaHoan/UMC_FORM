using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [CustomAuthorize("Admin")]
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            var userId = Session["userId"].ToString();
            var users = UserRepository.GetUsersEx(userId);
            return View(users);
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.depts = DeptRepository.GetDepts();
            ViewBag.roles = RoleRepository.GetRoles();
            ViewBag.positions = UserRepository.GetAllPosition();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Form_User user)
        {
            string message = "";
            ViewBag.depts = DeptRepository.GetDepts();
            ViewBag.roles = RoleRepository.GetRoles();
            var userExist = UserRepository.GetUser(user.CODE);
            if (userExist != null)
            {
                message = "Tài khoản đã tồn tại.";
                ViewBag.message = message;
                return View(user);
            }
            if (UserRepository.Save(user))
            {
                return RedirectToAction("Index");
            }
            message = "Có lỗi trong quá trình lưu dữ liệu";
            ViewBag.message = message;

            return View(user);
        }

        public ActionResult Details(string code)
        {
            if (code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserRepository.GetUser(code);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.role = RoleRepository.GetRole(user.ROLE_ID);
            return View(user);
        }

        [HttpGet]
        public ActionResult Edit(string code)
        {
            if (code == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = UserRepository.GetUser(code);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.depts = DeptRepository.GetDepts();
            ViewBag.roles = RoleRepository.GetRoles();
            ViewBag.positions = UserRepository.GetAllPosition();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Form_User user)
        {
            ViewBag.depts = DeptRepository.GetDepts();
            var userId = UserRepository.GetUser(user.CODE);
            if (userId is null)
            {
                return HttpNotFound();
            }
            if (UserRepository.Update(userId.CODE, user))
            {
                return RedirectToAction("Index");
            }
            return View(user);
        }
        [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string code)
        {
            UserRepository.Remove(code);
            return RedirectToAction("Index");

        }
    }
}
