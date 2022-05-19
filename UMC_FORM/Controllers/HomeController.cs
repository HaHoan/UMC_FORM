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
        private DataContext db = new DataContext();
        private bool IsApprover(Form_Summary item, int index, string userCode)
        {
            using (var db = new DataContext())
            {
                var processNext = db.Form_Procedures.Where(m => m.TICKET == item.TICKET && m.FORM_INDEX == index).ToList();
                foreach (var process in processNext)
                {
                    if (process.APPROVAL_NAME == userCode) return true;
                }


                return false;
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Index(string type, string formKey = "ALL")
        {

            if (type is null)
            {
                type = SendType.SENDTOME;
            }
            var session = Session["user"] as Form_User;
            List<Form_Summary> formSummaries = new List<Form_Summary>();
            List<Form_Summary> list = new List<Form_Summary>();
            if (string.IsNullOrEmpty(formKey) || formKey == "ALL")
            {
                list = db.Form_Summary.ToList();
            }
            else
            {
                list = db.Form_Summary.Where(m => m.TITLE.Contains(formKey)).ToList();
            }
            if (type == SendType.SENDTOME)
            {
                var listNotFinish = list.Where(r => r.IS_FINISH == false).ToList();

                foreach (var item in listNotFinish)
                {
                    var index = item.PROCEDURE_INDEX + 1;// Tìm index của trạm tiếp theo
                    if (IsApprover(item, index, session.CODE))
                    {
                        formSummaries.Add(item);
                    }

                }

            }
            else if (type == SendType.MYREQUEST)
            {
                if(session.ROLE_ID == ROLE.Approval)
                {
                    var result = from p in db.Form_Summary
                                 join c in db.Form_Procedures on p.TICKET equals c.TICKET
                                 where c.APPROVAL_NAME == session.CODE && p.PROCEDURE_INDEX >= c.FORM_INDEX
                                 select new 
                                 {
                                     ID = p.ID,
                                     IS_FINISH = p.IS_FINISH,
                                     IS_REJECT = p.IS_REJECT,
                                     PROCEDURE_INDEX = p.PROCEDURE_INDEX,
                                     TICKET = p.TICKET,
                                     CREATE_USER = p.CREATE_USER,
                                     UPD_DATE = p.UPD_DATE,
                                     TITLE = p.TITLE,
                                     PROCESS_ID = p.PROCESS_ID,
                                     PURPOSE = p.PURPOSE
                                 };
                    foreach(var p in result)
                    {
                        formSummaries.Add(new Form_Summary()
                        {
                            ID = p.ID,
                            IS_FINISH = p.IS_FINISH,
                            IS_REJECT = p.IS_REJECT,
                            PROCEDURE_INDEX = p.PROCEDURE_INDEX,
                            TICKET = p.TICKET,
                            CREATE_USER = p.CREATE_USER,
                            UPD_DATE = p.UPD_DATE,
                            TITLE = p.TITLE,
                            PROCESS_ID = p.PROCESS_ID,
                            PURPOSE = p.PURPOSE
                        });
                    }

                }
                else
                {
                    formSummaries = list.Where(r => r.CREATE_USER == session.CODE)
                     .ToList();
                }

            }
            else if (type == SendType.CANCEL)
            {
                var listReject = list.Where(r => r.IS_REJECT == true).ToList();
                foreach (var item in listReject)
                {
                    var index = item.PROCEDURE_INDEX + 1;// Tìm index của trạm tiếp theo
                    if (IsApprover(item, index, session.CODE))
                    {
                        formSummaries.Add(item);
                    }

                }
            }
            else if (type == SendType.FINISH)
            {
                if (session.POSITION == POSITION.FM || session.POSITION == POSITION.GD)
                {
                    formSummaries = list.Where(r => r.IS_FINISH == true).ToList();
                }
                else
                {
                    var deptHavePermission = db.LCA_PERMISSION.Where(m => m.DEPT == session.DEPT).Select(m => m.PROCESS).ToList();
                    var tickets = db.Form_Procedures.Where(m => m.APPROVAL_NAME == session.CODE || deptHavePermission.Contains(m.FORM_NAME)).Select(m => m.TICKET).ToList();
                    formSummaries = list.Where(m => m.IS_FINISH == true && tickets.Contains(m.TICKET)).ToList();

                }
            }
            else if (type == SendType.FOLLOW)
            {
                var listFollow = list.Where(r => r.IS_FINISH == false).ToList();
                var listPermission = db.LCA_PERMISSION.ToList();
                foreach (var item in listFollow)
                {
                    var listPermission1 = listPermission.Where(m => m.ITEM_COLUMN_PERMISSION == (item.PROCEDURE_INDEX + 1).ToString()
                                      && m.PROCESS == item.PROCESS_ID).ToList();
                    if (listPermission1.Where(m => m.DEPT == session.DEPT).FirstOrDefault() != null)
                    {
                        formSummaries.Add(item);
                    }
                }
                ViewBag.NumberFollowNotYet = formSummaries.Where(m => m.STATUS != STATUS.QUOTED).Count();
            }

            ViewBag.type = type;
            if (ViewBag.NumberFollowNotYet == null)
            {
                list = db.Form_Summary.Where(r => r.IS_FINISH == false).ToList();
                var numberNotYet = 0;
                var listPermission = db.LCA_PERMISSION.ToList();
                foreach (var item in list)
                {
                    if (item.STATUS == STATUS.QUOTED) continue;
                    var listPermission1 = listPermission.Where(m => m.ITEM_COLUMN_PERMISSION == (item.PROCEDURE_INDEX + 1).ToString()
                                         && m.PROCESS == item.PROCESS_ID).ToList();
                    if (listPermission1.Where(m => m.DEPT == session.DEPT).FirstOrDefault() != null)
                    {
                        numberNotYet++;
                    }
                }
                ViewBag.NumberFollowNotYet = numberNotYet;
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
