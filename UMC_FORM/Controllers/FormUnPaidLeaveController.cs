using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UMC_FORM.Controllers
{
    public class FormUnPaidLeaveController : Controller
    {
        // GET: FormUnPaidLeave
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Details()
        {
            return View();
        }
        public ActionResult Print()
        {
            return View();
        }
    }
}