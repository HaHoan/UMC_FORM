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

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [CustomAuthorize("Normal", "ReadOnly")]
    [NoCache]
    public class GAFormLeave41Controller : Controller
    {
        // GET: GAFormLeave41
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
        public ActionResult CreateFormPaidLeave41()
        {
            SetUpViewBagForCreate();
            return View();
        }
        private void SetUpFormProceduce(string processName, DataContext db, string ticket, string deptManager, string shift, List<Form_Process> process)
        {
            var formStation = db.Form_Stations.Where(m => m.PROCESS == processName).ToList();
            foreach(var pro in process)
            {
                if (pro.FORM_INDEX == 0)
                {
                    var procedure = new Form_Procedures()
                    {
                        ID = Guid.NewGuid().ToString(),
                        TICKET = ticket,
                        FORM_NAME = processName,
                        STATION_NO= pro.STATION_NO,
                        STATION_NAME=pro.STATION_NAME,
                        FORM_INDEX=pro.FORM_INDEX,
                        RETURN_INDEX=pro.RETURN_INDEX,
                        CREATER_NAME=pro.CREATER_NAME,
                        CREATE_DATE=pro.CREATE_DATE,
                        UPDATER_NAME=pro.UPDATER_NAME,
                        UPDATE_DATE=pro.UPDATE_DATE,
                        DES=pro.DES,
                        RETURN_STATION_NO=pro.RETURN_STATION_NO,
                        APPROVAL_NAME=_sess.CODE,
                    };
                    db.Form_Procedures.Add(procedure);
                }else if(pro.FORM_INDEX == 1)
                {
                    var procedure = new Form_Procedures()
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
                        APPROVAL_NAME = shift,
                    };
                    db.Form_Procedures.Add(procedure);
                }
                else if (pro.FORM_INDEX == 2)
                {
                    var procedure = new Form_Procedures()
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
                        APPROVAL_NAME = deptManager,
                    };
                    db.Form_Procedures.Add(procedure);
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
        private Tuple<string, string, int> AddLeaveItem(string leaveItems, DataContext db, GA_LEAVE_FORM prevTicket, GA_LEAVE_FORM currentTicket)
        {
            try
            {
                List<GA_LEAVE_FORM_ITEM> listLeaveItems = new List<GA_LEAVE_FORM_ITEM>();
               
                List<GA_LEAVE_FORM_ITEM_DETAIL> listLeaveItemsdetail = new List<GA_LEAVE_FORM_ITEM_DETAIL>();

                // không sửa gì
                if (string.IsNullOrEmpty(leaveItems))
                {
                    listLeaveItems = db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == prevTicket.ID).ToList();
                }

                // Khi sửa đổi items
                else
                {
                    var format = "dd/MM/yyyy HH:mm";
                    var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    listLeaveItems = JsonConvert.DeserializeObject<List<GA_LEAVE_FORM_ITEM>>(leaveItems, dateTimeConverter);
                   // listLeaveItemsdetail = JsonConvert.DeserializeObject<List<GA_LEAVE_FORM_ITEM_DETAIL>>(leaveItems, dateTimeConverter);
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
                        TIME_FROM = DateTime.Now,
                        TIME_TO = DateTime.Now,
                        TOTAL = item.TOTAL,
                        REASON = string.IsNullOrEmpty(item.REASON) ? "" : item.REASON,
                       CUSTOMER = item.CUSTOMER
                        
                    };
                    
                    db.GA_LEAVE_FORM_ITEM.Add(itemDB);
                    //var item_GA_DETAIL_TIMELEAVE = new GA_LEAVE_FORM_ITEM_DETAIL
                    //{
                    //    GA_LEAVE_FORM_ITEM_ID = itemDB.ID,
                    //    TIME_LEAVE = 
                    //};
                }
                db.SaveChanges();

                return Tuple.Create<string, string, int>(STATUS.SUCCESS, "", listLeaveItems.Count);
            }
            catch (Exception e)
            {
                return Tuple.Create<string, string, int>(STATUS.ERROR, e.Message.ToString(), 0);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(GA_LEAVE_FORM ticket, string leaveItems, string purpose, string formName)
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
                        var process = db.Form_Process.Where(c => c.FORM_NAME == Constant.GA_LEAVE_FORM41).ToList();
                        ticket.STATION_NAME = process.Where(m => m.FORM_INDEX == ticket.PROCEDURE_INDEX).FirstOrDefault().STATION_NAME;
                        ticket.STATION_NO = process.Where(m => m.FORM_INDEX == ticket.PROCEDURE_INDEX).FirstOrDefault().STATION_NO;
                        //SetUpFormProceduce(Constant.GA_LEAVE_FORM41, db, ticket.TICKET, ticket.DEPT_MANAGER, ticket.SHIFT_MANAGER, process);
                        var saveItems = AddLeaveItem(leaveItems, db, ticket, ticket);
                        //if (saveItems.Item1 == STATUS.ERROR)
                        //{
                        //    transaction.Rollback();
                        //    return Json(new { result = STATUS.ERROR, message = saveItems.Item2 }, JsonRequestBehavior.AllowGet);
                        //}
                        //ticket.NUMBER_REGISTER = saveItems.Item3;
                        //db.GA_LEAVE_FORM.Add(ticket);
                        //db.SaveChanges();
                        //var summary = new Form_Summary()
                        //{
                        //    ID = Guid.NewGuid().ToString(),
                        //    IS_FINISH = false,
                        //    IS_REJECT = false,
                        //    PROCEDURE_INDEX = 0,
                        //    TICKET = ticket.TICKET,
                        //    CREATE_USER = _sess.CODE,
                        //    UPD_DATE = DateTime.Now,
                        //    TITLE = purpose,
                        //    PURPOSE = "",
                        //    PROCESS_ID = Constant.GA_LEAVE_FORM41,
                        //    LAST_INDEX = process.Count() - 1
                        //};
                        //db.Form_Summary.Add(summary);
                        //db.SaveChanges();
                        transaction.Commit();

                        // Nếu người tạo trùng với quản lý ca => thực hiện tự động accept bước tiếp theo
                        var currentProceduce = db.Form_Procedures.Where(m => m.TICKET == ticket.TICKET && m.FORM_INDEX == (ticket.PROCEDURE_INDEX + 1)).FirstOrDefault();

                        if (_sess.CODE == currentProceduce.APPROVAL_NAME)
                        {
                           // return Accept(ticket, leaveItems);
                        }
                        //if (!sendMail(summary, STATUS.ACCEPT))
                        //{
                        //    transaction.Rollback();
                        //    return Json(new { result = STATUS.ERROR, message = "Error when send mail!" }, JsonRequestBehavior.AllowGet);
                        //};
                        return Json(new
                        {
                            result = STATUS.SUCCESS,
                            //ticket = summary.TICKET
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
        public ActionResult DetailFormPaidLeave41()
        {
            return View();
        }
        public ActionResult PrintFormPaidLeave41()
        {
            return View();
        }
    }
}