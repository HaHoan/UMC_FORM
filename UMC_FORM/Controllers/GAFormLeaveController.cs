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

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [CustomAuthorize("Normal", "ReadOnly")]
    [NoCache]
    public class GAFormLeaveController : Controller
    {
        private Form_User _sess = new Form_User();

        private void SetUpViewBagForCreate()
        {
            using (var db = new DataContext())
            {
                _sess = Session["user"] as Form_User;
                ViewBag.listManager = db.Form_User.Where(m => m.POSITION == POSITION.MANAGER && m.DEPT == _sess.DEPT).ToList();
                ViewBag.listShiftManager = db.Form_User.Where(m => m.POSITION == POSITION.SHIFT_MANAGER && m.DEPT == _sess.DEPT).ToList();
            }

        }

        private bool sendMail(Form_Summary summary, string typeMail)
        {
            try
            {
                using (var db = new DataContext())
                {
                    List<string> userMails = new List<string>();
                    var dear = "Dear All !";
                    if (summary.PROCEDURE_INDEX == -1)
                    {
                        var userCreate = UserRepository.GetUser(summary.CREATE_USER);
                        userMails.Add(userCreate.EMAIL);
                        var name = string.IsNullOrEmpty(userCreate.SHORT_NAME) ? userCreate.NAME : userCreate.SHORT_NAME;
                        dear = $"Dear {name} san !";
                    }
                    else
                    {
                        var stations = db.Form_Procedures.Where(m => m.TICKET == summary.TICKET &&
                        m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1) &&
                        m.FORM_NAME == summary.PROCESS_ID).Select(m => m.APPROVAL_NAME).ToList();
                        userMails = UserRepository.GetUsers((List<string>)stations);
                        if (userMails.Count == 1)
                        {
                            var userApproval = db.Form_User.Where(m => m.CODE == stations.FirstOrDefault()).FirstOrDefault();
                            var name = string.IsNullOrEmpty(userApproval.SHORT_NAME) ? userApproval.NAME : userApproval.SHORT_NAME;
                            dear = $"Dear {name} san !";
                        }
                    }


                    string body = "";
                    var domain = Bet.Util.Config.GetValue("domain");
                    if (typeMail == STATUS.REJECT)
                    {
                        body = $@"
                                                <h3>{dear}</h3>
                                                <h3 style='color: red' >Request reject. Please click below link view details:</h3>
	                                            <a href='{domain}LCA/Details?ticket={summary.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                    }
                    else if (typeMail == STATUS.ACCEPT)
                    {
                        body = $@"
                                                <h3>{dear}</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='{domain}LCA/Details?ticket={summary.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                    }

                    BackgroundJob.Enqueue(() => MailHelper.SenMailOutlookAsync(userMails, body, null));
                    return true;
                }
            }
            catch (Exception)
            {
                return false;

            }
        }
        private string validateTicket(GA_LEAVE_FORM ticket)
        {
            if (string.IsNullOrEmpty(ticket.DEPT))
            {
                return "Chưa chọn phòng ban!";
            }
            if (ticket.DATE_REGISTER == null)
            {
                return "Cần chọn ngày đăng kí!";
            }
            return "";
        }
        private Tuple<string, string> AddLeaveItem(string leaveItems, DataContext db, GA_LEAVE_FORM prevTicket, GA_LEAVE_FORM currentTicket)
        {
            try
            {
                List<GA_LEAVE_FORM_ITEM> listLeaveItems = new List<GA_LEAVE_FORM_ITEM>();
                // không sửa giá
                if (string.IsNullOrEmpty(leaveItems))
                {
                    listLeaveItems = db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == prevTicket.ID).ToList();
                }

                // Khi sửa đổi quotes
                else
                {
                    listLeaveItems = JsonConvert.DeserializeObject<List<GA_LEAVE_FORM_ITEM>>(leaveItems);
                }

               // test
                var leave = new GA_LEAVE_FORM_ITEM()
                {
                    TICKET = currentTicket.ID,
                    NO = 1,
                    FULLNAME = "Ha Thi Hoan",
                    CODE = "34811",
                    TIME_FROM = DateTime.Now,
                    TIME_TO = DateTime.Now,
                    TOTAL = 2,
                    REASON = "Nghỉ ốm",
                    SPEACIAL_LEAVE = false,
                    REMARK = "alaaa",
                };
                listLeaveItems.Add(leave);
                listLeaveItems.Add(leave);
                //
                if (listLeaveItems == null || listLeaveItems.Count == 0)
                {
                    return Tuple.Create<string, string>(STATUS.ERROR, "Kiểm tra lại thông tin danh sách người đăng ký!");
                }
                foreach (var item in listLeaveItems)
                {
                    if (string.IsNullOrEmpty(item.CODE))
                    {
                        return Tuple.Create<string, string>(STATUS.ERROR, "Thông tin người thứ  " + item.NO + " chưa có mã code!");
                    }

                    //test

                    //
                    var itemDB = new GA_LEAVE_FORM_ITEM
                    {
                        TICKET = currentTicket.ID,
                        NO = item.NO,
                        FULLNAME = item.FULLNAME,
                        CODE = item.CODE,
                        TIME_FROM = item.TIME_FROM,
                        TIME_TO = item.TIME_TO,
                        TOTAL = item.TOTAL,
                        REASON = string.IsNullOrEmpty(item.REASON) ? "" : item.REASON,
                        SPEACIAL_LEAVE = item.SPEACIAL_LEAVE,
                        REMARK = string.IsNullOrEmpty(item.REMARK) ? "" : item.REMARK,
                    };


                    db.GA_LEAVE_FORM_ITEM.Add(itemDB);
                }
                db.SaveChanges();

                return Tuple.Create<string, string>(STATUS.SUCCESS, "");
            }
            catch (Exception e)
            {
                return Tuple.Create<string, string>(STATUS.ERROR, e.Message.ToString());
            }


        }

        public ActionResult CreateFormPaidLeave()
        {
            SetUpViewBagForCreate();
            return View();
        }
        private void SetUpFormProceduce(string processName, DataContext db, string ticket, string deptManager, string shift, List<Form_Process> process)
        {
            var formStation = db.Form_Stations.Where(m => m.PROCESS == processName).ToList();

            foreach (var pro in process)
            {
                if (pro.FORM_INDEX == 0)
                {
                    var proceduce = new Form_Procedures()
                    {
                        ID = Guid.NewGuid().ToString(),
                        TICKET = ticket,
                        FORM_NAME = processName,
                        STATION_NO = pro.STATION_NO,
                        STATION_NAME = pro.STATION_NAME,
                        FORM_INDEX = pro.FORM_INDEX,
                        RETURN_INDEX = pro.RETURN_INDEX,
                        CREATER_NAME = pro.CREATER_NAME,
                        CREATE_DATE = pro.CREATE_DATE,
                        UPDATER_NAME = pro.UPDATER_NAME,
                        UPDATE_DATE = pro.UPDATE_DATE,
                        DES = pro.DES,
                        RETURN_STATION_NO = pro.RETURN_STATION_NO,
                        APPROVAL_NAME = _sess.CODE
                    };
                    db.Form_Procedures.Add(proceduce);

                }
                else
                if (pro.FORM_INDEX == 1)
                {
                    var proceduce = new Form_Procedures()
                    {
                        ID = Guid.NewGuid().ToString(),
                        TICKET = ticket,
                        FORM_NAME = processName,
                        STATION_NO = pro.STATION_NO,
                        STATION_NAME = pro.STATION_NAME,
                        FORM_INDEX = pro.FORM_INDEX,
                        RETURN_INDEX = pro.RETURN_INDEX,
                        CREATER_NAME = pro.CREATER_NAME,
                        CREATE_DATE = pro.CREATE_DATE,
                        UPDATER_NAME = pro.UPDATER_NAME,
                        UPDATE_DATE = pro.UPDATE_DATE,
                        DES = pro.DES,
                        RETURN_STATION_NO = pro.RETURN_STATION_NO,
                        APPROVAL_NAME = shift
                    };
                    db.Form_Procedures.Add(proceduce);
                }
                else if (pro.FORM_INDEX == 2)
                {
                    var proceduce = new Form_Procedures()
                    {
                        ID = Guid.NewGuid().ToString(),
                        TICKET = ticket,
                        FORM_NAME = processName,
                        STATION_NO = pro.STATION_NO,
                        STATION_NAME = pro.STATION_NAME,
                        FORM_INDEX = pro.FORM_INDEX,
                        RETURN_INDEX = pro.RETURN_INDEX,
                        CREATER_NAME = pro.CREATER_NAME,
                        CREATE_DATE = pro.CREATE_DATE,
                        UPDATER_NAME = pro.UPDATER_NAME,
                        UPDATE_DATE = pro.UPDATE_DATE,
                        DES = pro.DES,
                        RETURN_STATION_NO = pro.RETURN_STATION_NO,
                        APPROVAL_NAME = deptManager
                    };
                    db.Form_Procedures.Add(proceduce);
                }
                else
                {
                    var listUserApprover = formStation.Where(m => m.FORM_INDEX == pro.FORM_INDEX).ToList();
                    foreach (var userApprover in listUserApprover)
                    {
                        var proceduce = new Form_Procedures()
                        {
                            ID = Guid.NewGuid().ToString(),
                            TICKET = ticket,
                            FORM_NAME = processName,
                            STATION_NO = pro.STATION_NO,
                            STATION_NAME = pro.STATION_NAME,
                            FORM_INDEX = pro.FORM_INDEX,
                            RETURN_INDEX = pro.RETURN_INDEX,
                            CREATER_NAME = pro.CREATER_NAME,
                            CREATE_DATE = pro.CREATE_DATE,
                            UPDATER_NAME = pro.UPDATER_NAME,
                            UPDATE_DATE = pro.UPDATE_DATE,
                            DES = pro.DES,
                            RETURN_STATION_NO = pro.RETURN_STATION_NO,
                            APPROVAL_NAME = userApprover.USER_ID
                        };
                        db.Form_Procedures.Add(proceduce);

                    }

                }


            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateNew(GA_LEAVE_FORM ticket, string leaveItems, string purpose, string formName)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        _sess = Session["user"] as Form_User;

                        //test
                        ticket = new GA_LEAVE_FORM()
                        {
                            DEPT = _sess.DEPT,
                            CREATOR = _sess.CODE,
                            DATE_REGISTER = DateTime.Now,
                            NUMBER_REGISTER = 1,
                            DEPT_MANAGER = "hungnd",
                            SHIFT_MANAGER = "thent"

                        };
                        purpose = Constant.GA_LEAVE_FORM_GA_35;
                        formName = Constant.GA_PAID_LEAVE_ID;

                        //

                        string validate = validateTicket(ticket);
                        if (!string.IsNullOrEmpty(validate))
                        {
                            return Json(new { result = STATUS.ERROR, message = validate }, JsonRequestBehavior.AllowGet);
                        }

                        ticket.ID = Guid.NewGuid().ToString();
                        ticket.TICKET = DateTime.Now.ToString("yyyyMMddHHmmss");
                        ticket.CREATOR = _sess.CODE;
                        ticket.ORDER_HISTORY = 1;
                        ticket.PROCEDURE_INDEX = 0;
                        ticket.TITLE = purpose;
                        ticket.FORM_NAME = formName;
                        ticket.DEPT = _sess.DEPT;
                        ticket.SUBMIT_USER = _sess.CODE;
                        ticket.IS_SIGNATURE = 1;
                        ticket.UPD_DATE = DateTime.Now;
                        var process = db.Form_Process.Where(m => m.FORM_NAME == Constant.GA_LEAVE_FORM).ToList();
                        ticket.STATION_NAME = process.Where(m => m.FORM_INDEX == ticket.PROCEDURE_INDEX).FirstOrDefault().STATION_NAME;
                        ticket.STATION_NO = process.Where(m => m.FORM_INDEX == ticket.PROCEDURE_INDEX).FirstOrDefault().STATION_NO;
                        db.GA_LEAVE_FORM.Add(ticket);
                        db.SaveChanges();

                        SetUpFormProceduce(Constant.GA_LEAVE_FORM, db, ticket.TICKET, ticket.DEPT_MANAGER, ticket.SHIFT_MANAGER, process);

                        var saveItems = AddLeaveItem(leaveItems, db, ticket, ticket);
                        if (saveItems.Item1 == STATUS.ERROR)
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = saveItems.Item2 }, JsonRequestBehavior.AllowGet);
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
                            TITLE = purpose,
                            PROCESS_ID = Constant.GA_LEAVE_FORM,
                            PURPOSE = purpose,
                            LAST_INDEX = process.Count() - 1
                        };
                        db.Form_Summary.Add(summary);
                        db.SaveChanges();
                        transaction.Commit();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Accept(GA_LEAVE_FORM ticket, string leaveItems)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //test
                        ticket.ID = "4d0a427a-ca31-4187-ad52-3e0905a9a43c";
                        ticket.TICKET = "20220409154612";
                        //
                        _sess = Session["user"] as Form_User;
                        //string validate = validateTicket(ticket);
                        //if (!string.IsNullOrEmpty(validate))
                        //{
                        //    return Json(new { result = STATUS.ERROR, message = validate }, JsonRequestBehavior.AllowGet);
                        //}
                        var formDB = db.GA_LEAVE_FORM.Where(m => m.ID == ticket.ID).FirstOrDefault();
                        if (formDB == null) return Json(new { result = STATUS.ERROR, message = "Ticket không tồn tại!" }, JsonRequestBehavior.AllowGet);
                        var form = formDB.CloneObject() as GA_LEAVE_FORM;
                        form.ID = Guid.NewGuid().ToString();
                        form.SUBMIT_USER = _sess.CODE;
                        form.IS_SIGNATURE = 1;
                        form.UPD_DATE = DateTime.Now;

                        form.ORDER_HISTORY++;
                        form.PROCEDURE_INDEX++;
                        var process = db.Form_Process.Where(m => m.FORM_NAME == Constant.GA_LEAVE_FORM).ToList();
                        form.STATION_NAME = process.Where(m => m.FORM_INDEX == form.PROCEDURE_INDEX).FirstOrDefault().STATION_NAME;
                        form.STATION_NO = process.Where(m => m.FORM_INDEX == form.PROCEDURE_INDEX).FirstOrDefault().STATION_NO;
                        db.GA_LEAVE_FORM.Add(form);
                        db.SaveChanges();

                        var saveItems = AddLeaveItem(leaveItems, db, formDB, form);
                        if (saveItems.Item1 == STATUS.ERROR)
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = saveItems.Item2 }, JsonRequestBehavior.AllowGet);
                        }

                        var summary = db.Form_Summary.Where(m => m.TICKET == ticket.TICKET).FirstOrDefault();
                        summary.PROCEDURE_INDEX++;
                        if (summary.PROCEDURE_INDEX == summary.LAST_INDEX)
                        {
                            summary.IS_FINISH = true;
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        if (!sendMail(summary, STATUS.ACCEPT))
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

        [HttpPost]
        public JsonResult Reject(GA_LEAVE_FORM ticket, string leaveItems, string purpose, string formName)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var formDb = db.GA_LEAVE_FORM.Where(m => m.ID == ticket.ID).FirstOrDefault();
                        if (formDb == null) return Json(new { result = STATUS.ERROR, message = "Ticket không tồn tại" }, JsonRequestBehavior.AllowGet);
                        var form = formDb.CloneObject() as GA_LEAVE_FORM;
                        _sess = Session["user"] as Form_User;
                        var summary = db.Form_Summary.Where(m => m.TICKET == formDb.TICKET).FirstOrDefault();

                        // để lưu lại bước sẽ quay lại sau khi luồng reject được thực hiện xong
                        summary.REJECT_INDEX = form.PROCEDURE_INDEX;

                        var process = db.Form_Process.Where(m => m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1) && m.FORM_NAME == summary.PROCESS_ID).FirstOrDefault();
                        if (process == null)
                        {
                            return Json(new { result = STATUS.ERROR, message = "Check reject process again!" }, JsonRequestBehavior.AllowGet);
                        }

                        form.PROCEDURE_INDEX = process.RETURN_INDEX is int returnIndex ? returnIndex - 1 : 0;
                        form.ORDER_HISTORY += 1;
                        form.IS_SIGNATURE = 0;
                        form.ID = Guid.NewGuid().ToString();
                        form.SUBMIT_USER = _sess.CODE;
                        form.COMMENT = ticket.COMMENT;
                        var saveItems = AddLeaveItem(leaveItems, db, formDb, form);
                        if (saveItems.Item1 == STATUS.ERROR)
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = saveItems.Item2 }, JsonRequestBehavior.AllowGet);
                        }
                        db.GA_LEAVE_FORM.Add(form);

                        summary.IS_REJECT = true;

                        // Để lưu lại bước bị reject về
                        summary.RETURN_TO = form.PROCEDURE_INDEX;
                        summary.PROCEDURE_INDEX = form.PROCEDURE_INDEX;

                        db.SaveChanges();
                        transaction.Commit();
                        if (!sendMail(summary, STATUS.REJECT))
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = "Error when send mail!" }, JsonRequestBehavior.AllowGet);
                        };
                        return Json(new { result = STATUS.ERROR, message = saveItems.Item2 }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return Json(new { result = STATUS.ERROR, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);
                    }

                }
            }

        }
        private GA_LEAVE_FORM GetDetailTicket(string ticket)
        {
            using (var db = new DataContext())
            {
                if (string.IsNullOrEmpty(ticket))
                {
                    return null;
                }
                var ticketDb = db.GA_LEAVE_FORM.Where(m => m.TICKET == ticket).FirstOrDefault();
                if (ticketDb == null)
                {
                    return null;
                }
                ticketDb.GA_LEAVE_FORM_ITEMs = db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == ticketDb.ID).ToList();
                return ticketDb;
            }
        }
        public ActionResult DetailFormPaidLeave(string ticket)
        {
            //test
            ticket = "20220409154612";
            //
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

        public ActionResult CreateFormUnPaidLeave()
        {
            SetUpViewBagForCreate();
            return View();
        }
        public ActionResult DetailFormUnPaidLeave(string ticket)
        {
            SetUpViewBagForCreate();
            var ticketDb = GetDetailTicket(ticket);
            if (ticketDb == null) return HttpNotFound();
            return View(ticketDb);
        }
        public ActionResult PrintFormUnPaidLeave(string ticket)
        {
            SetUpViewBagForCreate();
            var ticketDb = GetDetailTicket(ticket);
            if (ticketDb == null) return HttpNotFound();
            return View(ticketDb);
        }
    }
}