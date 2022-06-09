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
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ticket = (GA_LEAVE_FORM)obj;
                        _sess = Session["user"] as Form_User;
                        ticket.ID = Guid.NewGuid().ToString();
                        ticket.TICKET = DateTime.Now.ToString("yyyyMMddHHmmss");
                        ticket.CREATOR = _sess.CODE;
                        ticket.ORDER_HISTORY = 1;
                        ticket.PROCEDURE_INDEX = 0;
                        ticket.DEPT = _sess.DEPT;
                        ticket.SUBMIT_USER = _sess.CODE;
                        ticket.IS_SIGNATURE = 1;
                        ticket.UPD_DATE = DateTime.Now;
                        var process = db.Form_Process.Where(m => m.FORM_NAME == Constant.GA_LEAVE_FORM).ToList();
                        var station = process.Where(m => m.FORM_INDEX == ticket.PROCEDURE_INDEX).FirstOrDefault();
                        ticket.STATION_NAME = station.STATION_NAME;
                        ticket.STATION_NO = station.STATION_NO;
                        ticket.GA_LEAVE_FORM_ITEMs = TicketGALeaveHelper.convertStringToListItem(ticket.leaveItems, ticket.ID, ticket.ID);

                        if (ticket.GA_LEAVE_FORM_ITEMs == null)
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = "Xem lại thông tin người đăng kí!" }, JsonRequestBehavior.AllowGet);
                        }

                        foreach (var item in ticket.GA_LEAVE_FORM_ITEMs)
                        {
                            db.GA_LEAVE_FORM_ITEM.Add(item);
                            db.SaveChanges();
                        }

                        ticket.NUMBER_REGISTER = ticket.GA_LEAVE_FORM_ITEMs.Count();
                        db.GA_LEAVE_FORM.Add(ticket);
                        db.SaveChanges();
                        var listProcedures = FormProcedureResponsitory.SetUpFormProceduce(Constant.GA_LEAVE_FORM, ticket, process, _sess.CODE);

                        if (listProcedures == null)
                        {

                            if (ticket.GA_LEAVE_FORM_ITEMs == null)
                            {
                                transaction.Rollback();
                                return Json(new { result = STATUS.ERROR, message = "Không lấy được giá trị của process!" }, JsonRequestBehavior.AllowGet);
                            }

                        }

                        foreach (var pro in listProcedures)
                        {
                            db.Form_Procedures.Add(pro);
                            db.SaveChanges();
                        }

                        var summary = new Form_Summary()
                        {
                            ID = Guid.NewGuid().ToString(),
                            IS_FINISH = false,
                            IS_REJECT = false,
                            PROCEDURE_INDEX = 0,
                            TICKET = ticket.TICKET,
                            CREATE_USER = _sess.CODE,
                            UPD_DATE = DateTime.Now,
                            TITLE = ticket.TITLE,
                            PURPOSE = ticket.TITLE,
                            PROCESS_ID = Constant.GA_LEAVE_FORM,
                            LAST_INDEX = process.Count() - 1
                        };
                        db.Form_Summary.Add(summary);
                        db.SaveChanges();
                        transaction.Commit();
                        // Nếu người tạo trùng với quản lý ca => thực hiện tự động accept bước tiếp theo
                        var currentProceduce = db.Form_Procedures.Where(m => m.TICKET == ticket.TICKET
                        && m.FORM_INDEX == (ticket.PROCEDURE_INDEX + 1)).FirstOrDefault();

                        if (_sess.CODE == currentProceduce.APPROVAL_NAME)
                        {
                            if (string.IsNullOrEmpty(ticket.DEPT_MANAGER))
                            {
                                return Json(new { result = STATUS.ERROR, message = "Bạn cần chọn trưởng phòng!" }, JsonRequestBehavior.AllowGet);
                            }
                            return Accept(ticket);
                        }

                        if (!MailResponsitory.SendMail(summary, STATUS.ACCEPT, "GAFormLeave"))
                        {
                            return Json(new { result = STATUS.ERROR, message = "Error when send mail!" }, JsonRequestBehavior.AllowGet);
                        };

                        return Json(new
                        {
                            result = STATUS.SUCCESS,
                            ticket = summary.TICKET
                        }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { result = STATUS.ERROR, message = ex.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        public JsonResult Delete(object obj)
        {
            var ticket = (GA_LEAVE_FORM)obj;
            var result = FormSummaryRepository.Delete(ticket.TICKET);
            if (result == STATUS.SUCCESS)
            {
                return Json(new
                {
                    result = STATUS.SUCCESS,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = STATUS.ERROR, message = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Accept(object obj)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ticket = (GA_LEAVE_FORM)obj;
                        _sess = Session["user"] as Form_User;
                        var formDB = db.GA_LEAVE_FORM.Where(m => m.ID == ticket.ID).FirstOrDefault();
                        if (formDB == null) return Json(new { result = STATUS.ERROR, message = "Ticket không tồn tại!" }, JsonRequestBehavior.AllowGet);
                        var form = formDB.CloneObject() as GA_LEAVE_FORM;
                        var summary = db.Form_Summary.Where(m => m.TICKET == ticket.TICKET).FirstOrDefault();
                        if (summary.IS_REJECT)
                        {
                            // check xem đã đi hết các bước reject chưa
                            FormRejectResponsitory.CheckFormReject(summary, (procedure_index, is_reject) =>
                            {
                                form.PROCEDURE_INDEX = procedure_index;
                                summary.IS_REJECT = is_reject;
                            });
                        }
                        else
                        {
                            form.IS_SIGNATURE = 1;
                            form.PROCEDURE_INDEX += 1;
                        }
                        summary.PROCEDURE_INDEX = form.PROCEDURE_INDEX;
                        if (summary.PROCEDURE_INDEX == summary.LAST_INDEX)
                        {
                            summary.IS_FINISH = true;
                        }

                        form.ID = Guid.NewGuid().ToString();
                        form.SUBMIT_USER = _sess.CODE;
                        form.UPD_DATE = DateTime.Now;
                        form.ORDER_HISTORY++;
                        if (!summary.IS_FINISH)
                        {
                            if (form.PROCEDURE_INDEX == 1)
                            {
                                if(!string.IsNullOrEmpty(ticket.DEPT_MANAGER) && ticket.DEPT_MANAGER != "0")
                                {
                                    var nextStation = db.Form_Procedures.Where(m => m.TICKET == form.TICKET && m.FORM_INDEX == 2).FirstOrDefault();
                                    nextStation.APPROVAL_NAME = ticket.DEPT_MANAGER;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    return Json(new { result = STATUS.ERROR, message = "Bạn cần chọn trưởng phòng cho bước tiếp theo!" }, JsonRequestBehavior.AllowGet);
                                }
                               
                            }
                            else
                            {
                                var isHaveDeptManager = FormProcedureResponsitory.CheckNextStepHaveApprover(form.TICKET, form.PROCEDURE_INDEX + 1);
                                if (!isHaveDeptManager)
                                {
                                    transaction.Rollback();
                                    return Json(new { result = STATUS.ERROR, message = "Bước tiếp theo chưa có người xác nhận!" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }

                        var process = db.Form_Process.Where(m => m.FORM_NAME == Constant.GA_LEAVE_FORM).ToList();
                        var station = process.Where(m => m.FORM_INDEX == form.PROCEDURE_INDEX).FirstOrDefault();
                        form.STATION_NAME = station.STATION_NAME;
                        form.STATION_NO = station.STATION_NO;
                        form.GA_LEAVE_FORM_ITEMs = TicketGALeaveHelper.convertStringToListItem(ticket.leaveItems, ticket.ID, form.ID);
                        if (form.GA_LEAVE_FORM_ITEMs == null)
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = "Xem lại thông tin người đăng kí!" }, JsonRequestBehavior.AllowGet);
                        }
                        foreach (var item in form.GA_LEAVE_FORM_ITEMs)
                        {
                            db.GA_LEAVE_FORM_ITEM.Add(item);
                            db.SaveChanges();
                        }
                        form.NUMBER_REGISTER = form.GA_LEAVE_FORM_ITEMs.Count();
                        db.GA_LEAVE_FORM.Add(form);
                        db.SaveChanges();

                        transaction.Commit();

                        // Nếu người tạo trùng với quản lý ca => thực hiện tự động accept bước tiếp theo
                        var currentProceduces = db.Form_Procedures.Where(m => m.TICKET == ticket.TICKET && m.FORM_INDEX == (form.PROCEDURE_INDEX + 1)).ToList();
                        foreach (var currentProceduce in currentProceduces)
                        {
                            if (currentProceduce != null && _sess.CODE == currentProceduce.APPROVAL_NAME)
                            {
                                return Accept(form);
                            }
                        }

                        if (!MailResponsitory.SendMail(summary, STATUS.ACCEPT, "GAFormLeave"))
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = "Error when send mail!" }, JsonRequestBehavior.AllowGet);
                        };
                        return Json(new
                        {
                            result = STATUS.SUCCESS,
                            ticket = summary.TICKET
                        }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { result = STATUS.ERROR, message = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        public JsonResult Reject(object obj)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ticket = (GA_LEAVE_FORM)obj;
                        var formDb = db.GA_LEAVE_FORM.Where(m => m.ID == ticket.ID).FirstOrDefault();
                        if (formDb == null) return Json(new { result = STATUS.ERROR, message = "Ticket không tồn tại" }, JsonRequestBehavior.AllowGet);
                        var form = formDb.CloneObject() as GA_LEAVE_FORM;
                        _sess = Session["user"] as Form_User;

                        var summary = db.Form_Summary.Where(m => m.TICKET == formDb.TICKET).FirstOrDefault();
                        var process = db.Form_Process.Where(m => m.FORM_NAME == summary.PROCESS_ID).ToList();
                        var currentProcess = db.Form_Process.Where(m => m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1)
                        && m.FORM_NAME == summary.PROCESS_ID).FirstOrDefault();

                        if (currentProcess == null)
                        {
                            return Json(new { result = STATUS.ERROR, message = "Check reject process again!" }, JsonRequestBehavior.AllowGet);
                        }
                        var station = process.Where(m => m.FORM_INDEX == (form.PROCEDURE_INDEX + 1)).FirstOrDefault();
                        form.STATION_NAME = station.STATION_NAME;
                        form.STATION_NO = station.STATION_NO;
                        form.PROCEDURE_INDEX = currentProcess.RETURN_INDEX is int returnIndex ? returnIndex - 1 : 0;
                        form.ORDER_HISTORY += 1;
                        form.IS_SIGNATURE = 0;
                        form.ID = Guid.NewGuid().ToString();
                        form.SUBMIT_USER = _sess.CODE;
                        form.COMMENT = ticket.COMMENT;
                        form.GA_LEAVE_FORM_ITEMs = TicketGALeaveHelper.convertStringToListItem(ticket.leaveItems, ticket.ID, form.ID);
                        if (form.GA_LEAVE_FORM_ITEMs == null)
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = "Xem lại thông tin người đăng kí!" }, JsonRequestBehavior.AllowGet);
                        }
                        foreach (var item in form.GA_LEAVE_FORM_ITEMs)
                        {
                            db.GA_LEAVE_FORM_ITEM.Add(item);
                            db.SaveChanges();
                        }
                        form.NUMBER_REGISTER = form.GA_LEAVE_FORM_ITEMs.Count();
                        db.GA_LEAVE_FORM.Add(form);

                        // để lưu lại bước sẽ quay lại sau khi luồng reject được thực hiện xong
                        summary.REJECT_INDEX = form.PROCEDURE_INDEX;
                        summary.IS_REJECT = true;

                        // Để lưu lại bước bị reject về
                        summary.RETURN_TO = form.PROCEDURE_INDEX;
                        summary.PROCEDURE_INDEX = form.PROCEDURE_INDEX;

                        db.SaveChanges();
                        transaction.Commit();
                        if (!MailResponsitory.SendMail(summary, STATUS.REJECT, "GAFormLeave"))
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = "Error when send mail!" }, JsonRequestBehavior.AllowGet);
                        };

                        return Json(new { result = STATUS.SUCCESS, message = "" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return Json(new { result = STATUS.ERROR, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);
                    }

                }
            }

        }

        private GA_LEAVE_FORM_DETAIL_MODEL GetDetailTicket(string ticket)
        {
            using (var db = new DataContext())
            {
                var modelDetail = new GA_LEAVE_FORM_DETAIL_MODEL();
                var list = db.GA_LEAVE_FORM.Where(m => m.TICKET == ticket).OrderByDescending(m => m.ORDER_HISTORY).ToList();

                if (list.Count == 0)
                {
                    return null;
                }

                modelDetail.TICKET = list.FirstOrDefault();
                if (modelDetail.TICKET == null)
                {
                    return null;
                }

                modelDetail.TICKET.GA_LEAVE_FORM_ITEMs = db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == modelDetail.TICKET.ID).ToList();
                modelDetail.SUMARY = db.Form_Summary.Where(m => m.TICKET == ticket).FirstOrDefault();

                if (modelDetail.SUMARY == null)
                {
                    return null;
                }

                _sess = Session["user"] as Form_User;
                var deptMng = db.Form_Procedures.Where(m => m.TICKET == ticket && m.STATION_NO == "DEPT_MANAGER").FirstOrDefault();

                if (deptMng != null && !string.IsNullOrEmpty(deptMng.APPROVAL_NAME))
                {
                    modelDetail.TICKET.DEPT_MANAGER_OBJECT = UserRepository.GetUser(deptMng.APPROVAL_NAME);
                }

                var groupLeader = db.Form_Procedures.Where(m => m.TICKET == ticket && m.STATION_NO == "GROUP_LEADER").FirstOrDefault();

                if (groupLeader != null && !string.IsNullOrEmpty(groupLeader.APPROVAL_NAME))
                {
                    modelDetail.TICKET.GROUP_LEADER_OBJECT = UserRepository.GetUser(groupLeader.APPROVAL_NAME);
                }

                modelDetail.PERMISSION = new List<string>();
                modelDetail.SUBMITS = new List<string>();
                if (_sess.ROLE_ID == ROLE.CanEdit || _sess.ROLE_ID == ROLE.Approval)
                {
                    modelDetail.PERMISSION = PermissionResponsitory.GetListPermission(modelDetail.SUMARY.PROCEDURE_INDEX + 1, modelDetail.SUMARY.PROCESS_ID);
                    modelDetail.SUBMITS = FormProcedureResponsitory.GetListApprover(modelDetail.SUMARY, _sess.CODE);
                }

                modelDetail.STATION_APPROVE = new TicketGALeaveHelper().GetListApproved(modelDetail.SUMARY);
                return modelDetail;
            }
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
            var ticketDb = GetDetailTicket(ticket);
            if (ticketDb == null) return HttpNotFound();
            SetUpViewBagForCreate();
            return View(ticketDb);
        }

        public ActionResult DetailFormUnPaidLeave(string ticket)
        {
            var ticketDb = GetDetailTicket(ticket);
            if (ticketDb == null) return HttpNotFound();
            SetUpViewBagForCreate();
            return View(ticketDb);
        }
        public ActionResult PrintFormUnPaidLeave(string ticket)
        {
            SetUpViewBagForCreate();
            var ticketDb = GetDetailTicket(ticket);
            if (ticketDb == null) return HttpNotFound();
            return View(ticketDb);
        }

        public ActionResult PrintFormPaidLeave(string ticket)
        {
            SetUpViewBagForCreate();
            var ticketDb = GetDetailTicket(ticket);
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