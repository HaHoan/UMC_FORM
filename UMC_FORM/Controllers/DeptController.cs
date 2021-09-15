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
    public class DeptController : Controller
    {
        // GET: Dept
        public ActionResult Index()
        {
            var depts = DeptRepository.GetAll();
            return View(depts);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Form_Dept entity)
        {
            string message = "";

            var deptExist = DeptRepository.GetDept(entity.DEPT);
            if (deptExist != null)
            {
                message = "Phòng ban đã tồn tại.";
                ViewBag.message = message;
                return View(deptExist);
            }
            if (DeptRepository.Save(entity))
            {
                return RedirectToAction("Index");
            }
            message = "Có lỗi trong quá trình lưu dữ liệu";
            ViewBag.message = message;

            return View(entity);
        }

        public ActionResult Details(string deptCode)
        {
            if (deptCode == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dept = DeptRepository.GetDept(deptCode);
            return View(dept);
        }

        [HttpGet]
        public ActionResult Edit(string deptCode)
        {
            if (deptCode == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dept = DeptRepository.GetDept(deptCode);
            return View(dept);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Form_Dept entity)
        {
            string message = "";
            if (entity == null || entity.DEPT == null)
            {
                return HttpNotFound();
            }
            var dept = DeptRepository.GetDept(entity.DEPT);
            if (dept is null)
            {
                return HttpNotFound();
            }
            if (DeptRepository.Save(entity))
            {
                return RedirectToAction("Index");
            }
            message = "Có lỗi trong quá trình lưu dữ liệu";
            ViewBag.message = message;
            return View(entity);
        }


        [HttpPost, ActionName("DeptDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string deptCode)
        {
            string message = "";
            if (DeptRepository.Remove(deptCode))
            {
                return RedirectToAction("Index");
            }
            message = "Có lỗi trong quá trình xóa dữ liệu";
            ViewBag.message = message;
            return View();

        }


    }
}
