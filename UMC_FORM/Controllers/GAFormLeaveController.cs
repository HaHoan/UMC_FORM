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
using System.Threading.Tasks;
using System.IO;
using UMC_FORM.Ultils;
using UMC_FORM.Business.GA;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [NoCache]
    public class GAFormLeaveController : Controller, IFormTemplateInterface
    {
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

        public ActionResult CreateFormPaidLeave()
        {
            SetUpViewBagForCreate();
            return View();
        }
        public ActionResult CreateFormUnPaidLeave()
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

        public JsonResult Create(object obj)
        {
            var createResult = new TicketGALeaveHelperForm35();
            _sess = Session["user"] as Form_User;
            var result = createResult.Create(obj, _sess);
            return Json(new { result = result.result, message = result.message}, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(object obj)
        {
            var result = TicketGALeaveHelper.Delete(obj);
            return Json(new
            {
                result = result.result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Accept(object obj)
        {
            var accept = new TicketGALeaveHelperForm35();
            _sess = Session["user"] as Form_User;
            var result = accept.Accept(obj, _sess);
            return Json(new { result = result.result, message = result.message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Reject(object obj)
        {
            _sess = Session["user"] as Form_User;
            var result = new TicketGALeaveHelperForm35().Reject(obj, _sess);
            return Json(new { result = result.result, message = result.message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string ticket)
        {
            using (var db = new DataContext())
            {
                var ticketDb = db.GA_LEAVE_FORM.Where(m => m.TICKET == ticket).FirstOrDefault();

                if (ticketDb == null) return HttpNotFound();
                else if (ticketDb.FORM_NAME == Constant.GA_PAID_LEAVE_ID)
                {
                    return RedirectToAction("DetailFormPaidLeave", new { ticket = ticket });
                }
                else if (ticketDb.FORM_NAME == Constant.GA_UNPAID_LEAVE_ID)
                {
                    return RedirectToAction("DetailFormUnPaidLeave", new { ticket = ticket });
                }
                return HttpNotFound();
            }

        }

        public ActionResult DetailFormPaidLeave(string ticket)
        {
            _sess = Session["user"] as Form_User;
            var ticketDb = new TicketGALeaveHelperForm35().GetDetailTicket(ticket,_sess);
            if (ticketDb == null) return HttpNotFound();
            SetUpViewBagForCreate();
            return View(ticketDb);
        }

        public ActionResult DetailFormUnPaidLeave(string ticket)
        {
            _sess = Session["user"] as Form_User;
            var ticketDb = new TicketGALeaveHelperForm35().GetDetailTicket(ticket, _sess);
            if (ticketDb == null) return HttpNotFound();
            SetUpViewBagForCreate();
            return View(ticketDb);
        }
        public ActionResult PrintFormUnPaidLeave(string ticket)
        {
            SetUpViewBagForCreate();
            _sess = Session["user"] as Form_User;
            var ticketDb = new TicketGALeaveHelperForm35().GetDetailTicket(ticket, _sess);
            if (ticketDb == null) return HttpNotFound();
            return View(ticketDb);
        }

        public ActionResult PrintFormPaidLeave(string ticket)
        {
            SetUpViewBagForCreate();
            _sess = Session["user"] as Form_User;
            var ticketDb = new TicketGALeaveHelperForm35().GetDetailTicket(ticket, _sess);
            if (ticketDb == null) return HttpNotFound();
            return View(ticketDb);
        }
        public ActionResult Export(string ticket)
        {
            MemoryStream bufferStream = null;
            using (var db = new DataContext())
            {
                var item = db.GA_LEAVE_FORM.Where(m => m.TICKET == ticket).OrderByDescending(m => m.ORDER_HISTORY).FirstOrDefault();
                if (item == null) return HttpNotFound();
                item.GA_LEAVE_FORM_ITEMs = db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == item.ID).ToList();
                var stream = ExcelHelper.CreateExcelFile(null, item);
                // Tạo buffer memory strean để hứng file excel
                bufferStream = stream as MemoryStream;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
                // File name của Excel này là ExcelDemo
                Response.AddHeader("Content-Disposition", "attachment; filename=" + ticket + ".xlsx");
                // Lưu file excel của chúng ta như 1 mảng byte để trả về response
                Response.BinaryWrite(bufferStream.ToArray());
                Console.WriteLine("done!");
                // Send tất cả ouput bytes về phía clients
                Response.Flush();
                Response.End();
                return View("Index");
            }

        }

    }
}