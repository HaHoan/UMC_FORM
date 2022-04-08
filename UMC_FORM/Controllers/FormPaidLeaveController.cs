using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Authentication;
using UMC_FORM.Models;
using UMC_FORM.Business;
namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [CustomAuthorize("Normal", "ReadOnly")]
    [NoCache]
    public class FormPaidLeaveController : Controller
    {
        private Form_User _sess = new Form_User();
        // GET: FormPaidLeave
        public ActionResult Create()
        {
            using(var db= new DataContext())
            {
                _sess = Session["user"] as Form_User;
                ViewBag.listManager= db.Form_User.Where(m=>m.POSITION==POSITION.MANAGER && m.DEPT== _sess.DEPT).ToList();
                return View();
            }
            
        }
        public ActionResult Detail()
        {
            return View();
        }
        public ActionResult Print()
        {
            return View();
        }

    }
}