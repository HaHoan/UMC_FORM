﻿using System;
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
        private DataContext db = new DataContext();
        private bool IsApprover(Form_Summary item, int index, string userCode)
        {
            var processNext = ProcessRepository.GetProcessName(item.PROCESS_ID).FirstOrDefault(r => r.FORM_INDEX == index);
            if (processNext != null)
            {
                var stationNoNext = processNext.STATION_NO;
                var users = db.Form_Stations.Where(r => r.STATION_NO == stationNoNext && r.PROCESS == item.PROCESS_ID);// Tim users approval
                foreach (var user in users)
                {
                    if (user.USER_ID == userCode)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Index(SendType? type)
        {
            if (type is null)
            {
                type = SendType.SENDTOME;
            }
            var session = Session["user"] as Form_User;
            List<Form_Summary> formSummaries = new List<Form_Summary>();
            var list = new List<Form_Summary>();
            switch (type)
            {
                case SendType.SENDTOME:
                    list = db.Form_Summary.Where(r => r.IS_FINISH == false).ToList();
                    foreach (var item in list)
                    {
                        var index = item.PROCEDURE_INDEX + 1;// Tìm index của trạm tiếp theo
                        if (IsApprover(item, index, session.CODE))
                        {
                            formSummaries.Add(item);
                        }

                    }
                    ViewBag.type = 1;
                    break;
                case SendType.MYREQUEST:
                    formSummaries = db.Form_Summary.Where(r => r.CREATE_USER == session.CODE && r.IS_FINISH == false && r.IS_REJECT == false)
                       .ToList();

                    ViewBag.type = 2;
                    break;
                case SendType.CANCEL:
                    ViewBag.type = 3;
                    list = db.Form_Summary.Where(r => r.IS_REJECT == true).ToList();
                    foreach (var item in list)
                    {
                        var index = item.PROCEDURE_INDEX + 1;// Tìm index của trạm tiếp theo
                        if (IsApprover(item, index, session.CODE))
                        {
                            formSummaries.Add(item);
                        }

                    }
                    break;
                case SendType.FINISH:
                    ViewBag.type = 4;
                    formSummaries = db.Form_Summary.Where(r => r.IS_FINISH == true && r.CREATE_USER == session.CODE).ToList();
                    break;

                default:
                    break;
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
