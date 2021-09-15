using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
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
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Form_Process form_Process, string formName, string qqq)
        {
            return View();
        }

    }
}
