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

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
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
                ViewBag.listGroupLeader = db.Form_User.Where(m => m.POSITION == POSITION.GROUP_LEADER && m.DEPT == _sess.DEPT).ToList();
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
                            var userApproval = db.Form_User.Where(m => stations.Contains(m.CODE) && m.EMAIL == userMails.FirstOrDefault()).FirstOrDefault();
                            if (userApproval != null)
                            {
                                var name = string.IsNullOrEmpty(userApproval.SHORT_NAME) ? userApproval.NAME : userApproval.SHORT_NAME;
                                dear = $"Dear {name} san !";
                            }

                        }
                    }


                    string body = "";
                    var domain = Bet.Util.Config.GetValue("domain");
                    if (typeMail == STATUS.REJECT)
                    {
                        body = $@"
                                                <h3>{dear}</h3>
                                                <h3 style='color: red' >Request reject. Please click below link view details:</h3>
	                                            <a href='{domain}GAFormLeave/DetailFormPaidLeave?ticket={summary.TICKET}'>Click to approval</a>
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
	                                            <a href='{domain}GAFormLeave/DetailFormPaidLeave?ticket={summary.TICKET}'>Click to approval</a>
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

        private Tuple<string, string, int> AddLeaveItem(string leaveItems, DataContext db, GA_LEAVE_FORM prevTicket, GA_LEAVE_FORM currentTicket)
        {
            try
            {
                List<GA_LEAVE_FORM_ITEM> listLeaveItems = new List<GA_LEAVE_FORM_ITEM>();
                // không sửa gì
                if (string.IsNullOrEmpty(leaveItems))
                {
                    listLeaveItems = db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == prevTicket.ID).ToList();
                }

                // Khi sửa đổi items
                else
                {
                    var format = "dd/MM/yyyy";
                    var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    listLeaveItems = JsonConvert.DeserializeObject<List<GA_LEAVE_FORM_ITEM>>(leaveItems, dateTimeConverter);
                }

                if (listLeaveItems == null || listLeaveItems.Count == 0)
                {
                    return Tuple.Create<string, string, int>(STATUS.ERROR, "Kiểm tra lại thông tin danh sách người đăng ký!", 0);
                }
                foreach (var item in listLeaveItems)
                {
                    if (string.IsNullOrEmpty(item.CODE))
                    {
                        return Tuple.Create<string, string, int>(STATUS.ERROR, "Thông tin người thứ  " + item.NO + " chưa có mã code!", 0);
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

                return Tuple.Create<string, string, int>(STATUS.SUCCESS, "", listLeaveItems.Count);
            }
            catch (Exception e)
            {
                return Tuple.Create<string, string, int>(STATUS.ERROR, e.ToString(), 0);
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
        private void SetUpFormProceduce(string processName, DataContext db, string ticket, string deptManager, string groupLeader, List<Form_Process> process)
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
                        APPROVAL_NAME = groupLeader
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
        private string UpdateFormProceduce(string processName, DataContext db, string ticket, string deptManager, int nextApprove)
        {
            var deptStation = db.Form_Procedures.Where(m => m.TICKET == ticket && m.STATION_NO == "DEPT_MANAGER").FirstOrDefault();
            if (deptManager == "0" || string.IsNullOrEmpty(deptManager))
            {
                if(deptStation.FORM_INDEX == nextApprove)
                {
                    return "Bạn chưa chọn trưởng bộ phận để approve cho bước tiếp theo!";
                }
                else
                {
                    return "";
                }
            }
            if (deptStation.FORM_INDEX == nextApprove && string.IsNullOrEmpty(deptManager))
            {
                
            }
            deptStation.APPROVAL_NAME = deptManager;
            db.SaveChanges();
            return "";
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

                        SetUpFormProceduce(Constant.GA_LEAVE_FORM, db, ticket.TICKET, ticket.DEPT_MANAGER, ticket.GROUP_LEADER, process);

                        var saveItems = AddLeaveItem(leaveItems, db, ticket, ticket);
                        if (saveItems.Item1 == STATUS.ERROR)
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = saveItems.Item2 }, JsonRequestBehavior.AllowGet);
                        }
                        ticket.NUMBER_REGISTER = saveItems.Item3;
                        db.GA_LEAVE_FORM.Add(ticket);
                        db.SaveChanges();
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
                            PURPOSE = "",
                            PROCESS_ID = Constant.GA_LEAVE_FORM,
                            LAST_INDEX = process.Count() - 1
                        };
                        db.Form_Summary.Add(summary);
                        db.SaveChanges();
                        transaction.Commit();

                        // Nếu người tạo trùng với quản lý ca => thực hiện tự động accept bước tiếp theo
                        var currentProceduce = db.Form_Procedures.Where(m => m.TICKET == ticket.TICKET && m.FORM_INDEX == (ticket.PROCEDURE_INDEX + 1)).FirstOrDefault();

                        if (_sess.CODE == currentProceduce.APPROVAL_NAME)
                        {
                            return Accept(ticket, leaveItems);
                        }
                        if (!sendMail(summary, STATUS.ACCEPT))
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Details(GA_LEAVE_FORM ticket, string leaveItems, string status)
        {
            if (status == STATUS.ACCEPT)
            {
                return Accept(ticket, leaveItems);
            }
            else if (status == STATUS.REJECT)
            {
                return Reject(ticket, leaveItems);
            }
            return Json(new { result = STATUS.ERROR, message = "Chưa có status này" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Accept(GA_LEAVE_FORM ticket, string leaveItems)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        _sess = Session["user"] as Form_User;
                        var formDB = db.GA_LEAVE_FORM.Where(m => m.ID == ticket.ID).FirstOrDefault();
                        if (formDB == null) return Json(new { result = STATUS.ERROR, message = "Ticket không tồn tại!" }, JsonRequestBehavior.AllowGet);
                        var form = formDB.CloneObject() as GA_LEAVE_FORM;
                        var summary = db.Form_Summary.Where(m => m.TICKET == ticket.TICKET).FirstOrDefault();
                        if (summary.IS_REJECT)
                        {
                            var stationByIndex = db.Form_Process.Where(m => m.FORM_INDEX == (form.PROCEDURE_INDEX + 1) && m.FORM_NAME == summary.PROCESS_ID).FirstOrDefault();
                            if (stationByIndex != null)
                            {
                                var stationIsSinging = db.GA_LEAVE_FORM.Where(m => m.TICKET == form.TICKET
                                && m.STATION_NO.Trim() == stationByIndex.STATION_NO.Trim()
                                && m.IS_SIGNATURE == 1).FirstOrDefault();
                                if (stationIsSinging == null)
                                {
                                    form.IS_SIGNATURE = 1;
                                }
                                else form.IS_SIGNATURE = 0;
                            }
                            else
                            {
                                return Json(new { result = STATUS.ERROR, message = "Kiểm tra lại form reject process" }, JsonRequestBehavior.AllowGet);
                            }


                            var processReject = db.Form_Reject.Where(m => m.PROCESS_NAME == summary.PROCESS_ID && m.START_INDEX == summary.RETURN_TO).ToList();
                            var currentStep = processReject.Where(m => m.FORM_INDEX == summary.PROCEDURE_INDEX).FirstOrDefault();
                            if (currentStep != null)
                            {

                                if (currentStep.STEP_ORDER == currentStep.TOTAL_STEP)
                                {
                                    form.PROCEDURE_INDEX = summary.REJECT_INDEX;
                                    summary.IS_REJECT = false;
                                }
                                else
                                {
                                    var nextStep = processReject.Where(m => m.STEP_ORDER == currentStep.STEP_ORDER + 1).FirstOrDefault();
                                    if (nextStep != null && nextStep.FORM_INDEX < summary.REJECT_INDEX)
                                    {
                                        form.PROCEDURE_INDEX = nextStep.FORM_INDEX;
                                    }
                                    else
                                    {
                                        form.PROCEDURE_INDEX = summary.REJECT_INDEX;
                                        summary.IS_REJECT = false;
                                    }

                                }
                            }
                            else
                            {
                                form.PROCEDURE_INDEX = summary.REJECT_INDEX;
                                summary.IS_REJECT = false;
                            }
                        }
                        else
                        {
                            form.IS_SIGNATURE = 1;
                            form.PROCEDURE_INDEX += 1;
                        }
                        summary.PROCEDURE_INDEX++;
                        if (summary.PROCEDURE_INDEX == summary.LAST_INDEX)
                        {
                            summary.IS_FINISH = true;
                        }
                        form.ID = Guid.NewGuid().ToString();
                        form.SUBMIT_USER = _sess.CODE;
                        form.UPD_DATE = DateTime.Now;
                        form.ORDER_HISTORY++;
                        var result = UpdateFormProceduce(Constant.GA_LEAVE_FORM, db, ticket.TICKET, ticket.DEPT_MANAGER, form.PROCEDURE_INDEX + 1);
                        if (!string.IsNullOrEmpty(result))
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = result}, JsonRequestBehavior.AllowGet);
                        }
                        var process = db.Form_Process.Where(m => m.FORM_NAME == Constant.GA_LEAVE_FORM).ToList();
                        form.STATION_NAME = process.Where(m => m.FORM_INDEX == form.PROCEDURE_INDEX).FirstOrDefault().STATION_NAME;
                        form.STATION_NO = process.Where(m => m.FORM_INDEX == form.PROCEDURE_INDEX).FirstOrDefault().STATION_NO;
                        var saveItems = AddLeaveItem(leaveItems, db, formDB, form);
                        if (saveItems.Item1 == STATUS.ERROR)
                        {
                            transaction.Rollback();
                            return Json(new { result = STATUS.ERROR, message = saveItems.Item2 }, JsonRequestBehavior.AllowGet);
                        }
                        form.NUMBER_REGISTER = saveItems.Item3;
                        db.GA_LEAVE_FORM.Add(form);
                        db.SaveChanges();


                        transaction.Commit();

                        // Nếu người tạo trùng với quản lý ca => thực hiện tự động accept bước tiếp theo
                        var currentProceduces = db.Form_Procedures.Where(m => m.TICKET == ticket.TICKET && m.FORM_INDEX == (form.PROCEDURE_INDEX + 1)).ToList();
                        foreach (var currentProceduce in currentProceduces)
                        {
                            if (currentProceduce != null && _sess.CODE == currentProceduce.APPROVAL_NAME)
                            {
                                return Accept(form, leaveItems);
                            }
                        }

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

        public JsonResult Reject(GA_LEAVE_FORM ticket, string leaveItems)
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

                        var process = db.Form_Process.Where(m => m.FORM_NAME == summary.PROCESS_ID).ToList();
                        var currentProcess = db.Form_Process.Where(m => m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1) && m.FORM_NAME == summary.PROCESS_ID).FirstOrDefault();

                        if (currentProcess == null)
                        {
                            return Json(new { result = STATUS.ERROR, message = "Check reject process again!" }, JsonRequestBehavior.AllowGet);
                        }
                        form.STATION_NAME = process.Where(m => m.FORM_INDEX == (form.PROCEDURE_INDEX + 1)).FirstOrDefault().STATION_NAME;
                        form.STATION_NO = process.Where(m => m.FORM_INDEX == (form.PROCEDURE_INDEX + 1)).FirstOrDefault().STATION_NO;
                        form.PROCEDURE_INDEX = currentProcess.RETURN_INDEX is int returnIndex ? returnIndex - 1 : 0;
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
                        form.NUMBER_REGISTER = saveItems.Item3;
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

                modelDetail.PERMISSION = new List<string>();

                modelDetail.SUBMITS = new List<string>();
                if (_sess.ROLE_ID == ROLE.CanEdit || _sess.ROLE_ID == ROLE.Approval)
                {
                    var listPermission = db.LCA_PERMISSION.Where(m => m.ITEM_COLUMN_PERMISSION == (modelDetail.SUMARY.PROCEDURE_INDEX + 1).ToString()
                    && m.PROCESS == modelDetail.SUMARY.PROCESS_ID).ToList();
                    foreach (var permission in listPermission)
                    {
                        modelDetail.PERMISSION.Add(permission.ITEM_COLUMN);
                    }

                    var userApprover = db.Form_Procedures.Where(m => m.FORM_INDEX == (modelDetail.SUMARY.PROCEDURE_INDEX + 1)
                                       && m.FORM_NAME == modelDetail.SUMARY.PROCESS_ID && m.TICKET == modelDetail.TICKET.TICKET).ToList();
                    if (userApprover.Where(m => m.APPROVAL_NAME == _sess.CODE).FirstOrDefault() != null)
                    {
                        if (modelDetail.SUMARY.IS_REJECT)
                        {
                            modelDetail.SUBMITS.Add(SUBMIT.RE_APPROVE);
                        }
                        else
                        {
                            modelDetail.SUBMITS.Add(SUBMIT.APPROVE);
                        }

                    }

                }

                modelDetail.STATION_APPROVE = getListApproved(modelDetail.SUMARY, db, list);
                return modelDetail;
            }
        }
        private List<StationApproveModel> getListApproved(Form_Summary summary, DataContext db, List<GA_LEAVE_FORM> list)
        {
            var listApproved = new List<StationApproveModel>();
            var process = db.Form_Procedures.Where(m => m.TICKET == summary.TICKET && m.FORM_NAME == summary.PROCESS_ID).OrderBy(m => m.FORM_INDEX).ToList();
            foreach (var pro in process)
            {
                if (listApproved.Where(m => m.STATION_NAME.Trim() == pro.STATION_NAME.Trim()).FirstOrDefault() != null) continue;
                var station = new StationApproveModel()
                {
                    STATION_NAME = pro.STATION_NAME,
                    IS_APPROVED = false

                };
                var lca = list.Where(m => m.STATION_NAME.Trim() == pro.STATION_NAME.Trim() && m.IS_SIGNATURE == 1).FirstOrDefault();
                if (lca != null)
                {
                    station.IS_APPROVED = true;
                    station.APPROVE_DATE = lca.UPD_DATE;
                    station.APPROVER = lca.SUBMIT_USER;
                    station.COMPANY = "UMCVN";
                    var user = db.Form_User.Where(m => m.NAME == station.APPROVER).FirstOrDefault();
                    if (user != null)
                    {
                        station.SIGNATURE = user.SIGNATURE;
                    }
                    else
                    {
                        station.SIGNATURE = station.APPROVER;
                    }
                }
                listApproved.Add(station);
            }
            return listApproved;
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