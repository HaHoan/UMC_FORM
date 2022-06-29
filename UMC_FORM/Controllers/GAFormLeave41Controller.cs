using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Authentication;
using UMC_FORM.Models;
using UMC_FORM.Business;
using UMC_FORM.Models.GA;
using Newtonsoft.Json;
using System.Data.Entity;
using Hangfire;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Converters;
using UMC_FORM.Business.GA;
using System.IO;
using UMC_FORM.Ultils;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [NoCache]
    public class GAFormLeave41Controller : Controller, IFormTemplateInterface
    {
        // GET: GAFormLeave41
        private Form_User _sess = new Form_User();
        private void SetUpViewBagForCreate()
        {
            using (var db = new DataContext())
            {
                _sess = Session["user"] as Form_User;
                ViewBag.listManager = UserRepository.GetManagers(null);
                ViewBag.listGroupLeader = UserRepository.GetGroupLeaders(_sess.DEPT);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Details(GA_LEAVE_FORM ticket)
        {
            if (ticket.status == STATUS.ACCEPT)
            {
                return Accept(ticket);
            }
            else if (ticket.status == STATUS.REJECT)
            {
                return Reject(ticket);
            }
            else if (ticket.status == STATUS.DELETE)
            {
                return Delete(ticket);
            }
            return Json(new { result = STATUS.ERROR, message = "Chưa có status này" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateFormPaidLeave41()
        {
            SetUpViewBagForCreate();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateNew(GA_LEAVE_FORM ticket)
        {
            return Create(ticket);
        }
        public JsonResult Create(object obj)
        {
            var createResult = new TicketGALeaveHelperForm41();
            _sess = Session["user"] as Form_User;
            var result = createResult.Create(obj, _sess);
            return Json(new { result = result.result, message = result.message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Accept(object obj)
        {
            var accept = new TicketGALeaveHelperForm41();
            _sess = Session["user"] as Form_User;
            var result = accept.Accept(obj, _sess);
            return Json(new { result = result.result, message = result.message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(object obj)
        {
            var result = TicketGALeaveHelper.Delete(obj);
            return Json(new
            {
                result = result.result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Reject(object obj)
        {
            _sess = Session["user"] as Form_User;
            var result = new TicketGALeaveHelperForm41().Reject(obj, _sess);
            return Json(new { result = result.result, message = result.message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(string ticket)
        {
            using (var db = new DataContext())
            {
                var ticketDb = db.GA_LEAVE_FORM.Where(m => m.TICKET == ticket).FirstOrDefault();
                if (ticketDb == null) return HttpNotFound();

                return RedirectToAction("DetailFormPaidLeave41", new { ticket = ticket });
            }

        }
        public ActionResult DetailFormPaidLeave41(string ticket)
        {
            _sess = Session["user"] as Form_User;
            var model_detail = new TicketGALeaveHelperForm41().GetDetailTicket(ticket, _sess);
            if (model_detail == null) HttpNotFound();
            ViewBag.List_detail = GetTimeLeave(ticket);
            SetUpViewBagForCreate();
            return View(model_detail);
        }
        public ActionResult PrintFormPaidLeave41(string ticket)
        {
            _sess = Session["user"] as Form_User;
            var model_detail = new TicketGALeaveHelperForm41().GetDetailTicket(ticket, _sess);
            if (model_detail == null) HttpNotFound();
            ViewBag.List_detail = GetTimeLeave(ticket);
            SetUpViewBagForCreate();
            return View(model_detail);
        }
        public static List<GA_LEAVE_FORM_ITEM_DETAIL> GetTimeLeave(string ticket)
        {
            using (var db = new DataContext())
            {
                var detail = new List<GA_LEAVE_FORM_ITEM_DETAIL>();
                var list_GAFORM = db.GA_LEAVE_FORM.Where(m => m.TICKET == ticket).OrderByDescending(m => m.ORDER_HISTORY).FirstOrDefault();
                var list_GAFORMITEM = db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == list_GAFORM.ID).ToList();
                foreach (var items in list_GAFORMITEM)
                {
                    var list_GAFORMITEMDETAIL = db.GA_LEAVE_FORM_ITEM_DETAIL.Where(m => m.GA_LEAVE_FORM_ITEM_ID == items.ID).ToList();

                    foreach (var item_detail in list_GAFORMITEMDETAIL)
                    {
                        detail.Add(item_detail);
                    }
                }

                return detail.GroupBy(x => x.TIME_LEAVE).Select(y => y.First()).ToList();
            }
        }
    }
}