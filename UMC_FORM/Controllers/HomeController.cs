using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    public class HomeController : Controller
    {

        public ActionResult Index(string type, string formKey = "ALL")
        {

            if (type is null)
            {
                type = SendType.SENDTOME;
            }
            var session = Session["user"] as Form_User;
            List<Form_Summary> formSummaries = new List<Form_Summary>();
            if (type == SendType.SENDTOME)
            {
                formSummaries = FormSummaryRepository.GetAllSendToMe(session.CODE);
            }
            else if (type == SendType.MYREQUEST)
            {
                formSummaries = FormSummaryRepository.GetAllMyRequest(session.CODE);
            }
            else if (type == SendType.CANCEL)
            {
                formSummaries = FormSummaryRepository.GetAllRejectToMe(session.CODE);
            }
            else if (type == SendType.FINISH)
            {
                formSummaries = FormSummaryRepository.GetAllFinish(session);
            }
            else if (type == SendType.FOLLOW)
            {
                formSummaries = FormSummaryRepository.GetAllFollow(session.CODE, session.DEPT);
                ViewBag.NumberFollowNotYet = formSummaries.Where(m => m.STATUS != STATUS.QUOTED).Count();
            }

            ViewBag.type = type;
            if (ViewBag.NumberFollowNotYet == null)
            {
                ViewBag.NumberFollowNotYet = FormSummaryRepository.GetNumberFollowNotDone(session);
            }
            return View(formSummaries.OrderByDescending(m => m.UPD_DATE));
        }
        public ActionResult Search(string search)
        {
            var session = Session["user"] as Form_User;
            List<Form_Summary> formSummaries = new List<Form_Summary>();
            var summary = FormSummaryRepository.GetSummaryLike(search);
            return View(summary);
        }

        public ActionResult About()
        {
            ViewBag.Message = "IT Design.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Tel: 3143";
            return View();
        }

    }
}
