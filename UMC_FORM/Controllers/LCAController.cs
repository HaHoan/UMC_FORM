using Hangfire;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;
using UMC_FORM.Models.LCA;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [NoCache]
    public class LCAController : Controller
    {
        private Form_User _sess = new Form_User();
        // GET: LCA
        public ActionResult Create()
        {
           
            using (var db = new DataContext())
            {
                _sess = Session["user"] as Form_User;
                ViewBag.listManager = UserRepository.GetManagers(_sess.DEPT);
                return View();
            }

        }
        private List<StationApproveModel> getListApproved(Form_Summary summary, DataContext db, List<LCA_FORM_01> list)
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
        private void setUpFormProceduce(string processName, DataContext db, string ticket, string deptManager, List<Form_Process> process)
        {
            var formStation = db.Form_Stations.Where(m => m.PROCESS == processName).ToList();
            var oldProcess = db.Form_Procedures.Where(m => m.TICKET == ticket).ToList();
            if (oldProcess == null || oldProcess.Count == 0)
            {
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
                    if (pro.FORM_INDEX == 1 || pro.FORM_INDEX == 3)
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

            else
            {
                foreach (var pro in process)
                {
                    if (pro.FORM_INDEX == 0)
                    {
                        var proceduceOld = oldProcess.Where(m => m.TICKET == ticket && m.FORM_INDEX == 0).FirstOrDefault();

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
                            RETURN_STATION_NO = pro.RETURN_STATION_NO
                        };
                        if (proceduceOld != null)
                        {
                            proceduce.APPROVAL_NAME = proceduceOld.APPROVAL_NAME;
                        }
                        db.Form_Procedures.Add(proceduce);

                    }
                    else
                    if (pro.FORM_INDEX == 1 || pro.FORM_INDEX == 3)
                    {
                        var proceduceOld = oldProcess.Where(m => m.TICKET == ticket && m.STATION_NAME.Trim().Replace("\n", "").Replace("\r", "") == pro.STATION_NAME.Trim().Replace("\n", "").Replace("\r", "")).FirstOrDefault();

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
                            RETURN_STATION_NO = pro.RETURN_STATION_NO

                        };
                        if (proceduceOld != null)
                        {
                            proceduce.APPROVAL_NAME = proceduceOld.APPROVAL_NAME;
                        }
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
            // Trường hợp thay đổi proceduce thì phải xóa các trạm không dùng
            var processOld = db.Form_Procedures.Where(m => m.TICKET == ticket && m.FORM_NAME != processName).ToList();
            db.Form_Procedures.RemoveRange(processOld);

        }
        public JsonResult DeleteFiles(string deleteFile)
        {
            try
            {
                string fullPath = Request.MapPath("~" + deleteFile);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                return Json(new { result = STATUS.SUCCESS, }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { result = STATUS.ERROR, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult UploadFiles()
        {
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                var listFiles = new List<LCA_FILE>();
                for (int file = 0; file < files.Count; file++)
                {
                    HttpPostedFile filedata = files[file];
                    string fileName = filedata.FileName;
                    var lcaFile = new LCA_FILE();
                    lcaFile.FILE_URL = string.Format("/UploadedFiles/{0}-{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
                    lcaFile.FILE_NAME = fileName;
                    string fullPath = Server.MapPath(lcaFile.FILE_URL);
                    if (fileName != "")
                    {
                        filedata.SaveAs(fullPath);
                        listFiles.Add(lcaFile);
                    }
                }
                return Json(new { result = STATUS.SUCCESS, message = listFiles }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { result = STATUS.ERROR, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(LCA_FORM_01 ticket, string quotes, string deptManager, string listFiles)
        {
            if (ticket == null)
            {
                return Json(new { result = STATUS.ERROR, message = "Ticket not have data!" }, JsonRequestBehavior.AllowGet);
            }
            try
            {

                using (var db = new DataContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {

                            string validate = validateTicket(ticket);
                            if (!string.IsNullOrEmpty(validate))
                            {
                                return Json(new { result = STATUS.ERROR, message = validate }, JsonRequestBehavior.AllowGet);
                            }
                            _sess = Session["user"] as Form_User;
                            ticket.DEPT = _sess.DEPT;
                            ticket.SUBMIT_USER = _sess.CODE;
                            ticket.CREATE_USER = _sess.CODE;
                            ticket.ORDER_HISTORY = 1;
                            ticket.IS_SIGNATURE = 1;
                            ticket.PROCEDURE_INDEX = 0;
                            ticket.ID = Guid.NewGuid().ToString();
                            ticket.TICKET = DateTime.Now.ToString("yyyyMMddHHmmss");
                            ticket.TITLE = Constant.LCA_FORM_01_TITLE;
                            ticket.UPD_DATE = DateTime.Now;
                            var processName = (ticket.PAYER == PAYER.UMCVN) ? Constant.LCA_01 : Constant.LCA_Process;
                            var process = db.Form_Process.Where(m => m.FORM_NAME == processName).ToList();
                            var deptDb = db.Form_User.Where(m => m.CODE == deptManager).FirstOrDefault();
                            if (deptDb == null) return Json(new { result = STATUS.ERROR, message = "Không tồn tại user có code là " + deptManager }, JsonRequestBehavior.AllowGet);
                            setUpFormProceduce(processName, db, ticket.TICKET, deptManager, process);

                            ticket.STATION_NAME = process.Where(m => m.FORM_INDEX == ticket.PROCEDURE_INDEX).FirstOrDefault().STATION_NAME;
                            ticket.STATION_NO = process.Where(m => m.FORM_INDEX == ticket.PROCEDURE_INDEX).FirstOrDefault().STATION_NO;
                            db.LCA_FORM_01.Add(ticket);
                            #region FILES

                            if (!Directory.Exists(Server.MapPath("/UploadedFiles/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("/UploadedFiles/"));
                            }

                            var saveFile = AddFilesToForm(listFiles, ticket, db);
                            if (saveFile.Item1 == STATUS.ERROR)
                            {
                                transaction.Rollback();
                                return Json(new { result = STATUS.ERROR, message = saveFile.Item2 }, JsonRequestBehavior.AllowGet); ;
                            }
                            #endregion
                            var saveQuote = AddQuotes(quotes, db, ticket, ticket);
                            if (saveQuote.Item1 == STATUS.ERROR)
                            {
                                transaction.Rollback();

                                return Json(new { result = STATUS.ERROR, message = saveQuote.Item2 }, JsonRequestBehavior.AllowGet);
                            }
                            #region SUMARY
                            Form_Summary summary = new Form_Summary()
                            {
                                ID = Guid.NewGuid().ToString(),
                                IS_FINISH = false,
                                IS_REJECT = false,
                                PROCEDURE_INDEX = 0,
                                TICKET = ticket.TICKET,
                                CREATE_USER = _sess.CODE,
                                UPD_DATE = DateTime.Now,
                                TITLE = Constant.LCA_FORM_01_TITLE,
                                PROCESS_ID = processName,
                                PURPOSE = ticket.PURPOSE
                            };
                            summary.LAST_INDEX = process.Count() - 1;

                            db.Form_Summary.Add(summary);
                            #endregion

                            db.SaveChanges();
                            transaction.Commit();
                            if (!MailResponsitory.SendMail(summary, STATUS.ACCEPT, "LCA"))
                            {
                                transaction.Rollback();
                                ModelState.AddModelError("Error", "Error when send mail!");
                                return Json(new { result = STATUS.ERROR }, JsonRequestBehavior.AllowGet);
                            }
                            return Json(new
                            {
                                result = STATUS.SUCCESS,
                                ticket = summary.TICKET,
                                typeMail = STATUS.ACCEPT
                            }, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            var error = ModelState.Values.Where(m => m.Errors.Count > 0).ToList();
                            ModelState.AddModelError("Error", e.Message.ToString());
                            return Json(new { result = STATUS.ERROR, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }

            }
            catch (Exception e)
            {

                ModelState.AddModelError("Error", e.Message.ToString());
                return Json(new { result = STATUS.ERROR, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Details(string ticket)
        {
            try
            {
                if (string.IsNullOrEmpty(ticket))
                {
                    return HttpNotFound();
                }
                double number = 1.00;
                var str = number.FormatPrice();
                using (var db = new DataContext())
                {
                    var modelDetail = new LCADetailModel();
                    var list = db.LCA_FORM_01.Where(m => m.TICKET == ticket).OrderByDescending(m => m.ORDER_HISTORY).ToList();
                    if (list.Count == 0)
                    {
                        return HttpNotFound();
                    }
                    modelDetail.TICKET = list.FirstOrDefault();
                    modelDetail.TICKET.FILES = db.LCA_FILE.Where(m => m.TICKET == modelDetail.TICKET.TICKET).ToList();
                    if (modelDetail.TICKET == null)
                    {
                        return HttpNotFound();
                    }
                    modelDetail.TICKET.LCA_QUOTEs = db.LCA_QUOTE.Where(m => m.ID_TICKET == modelDetail.TICKET.ID).ToList();
                    modelDetail.SUMARY = db.Form_Summary.Where(m => m.TICKET == ticket).FirstOrDefault();
                    if (modelDetail.SUMARY == null)
                    {
                        return HttpNotFound();
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
                        var isEditQuote = listPermission.Where(m => m.DEPT == _sess.DEPT && m.ITEM_COLUMN == PERMISSION.QUOTE).FirstOrDefault();
                        if (isEditQuote != null)
                        {
                            modelDetail.SUBMITS.Add(SUBMIT.EDIT_QUOTE);

                        }

                    }

                    modelDetail.STATION_APPROVE = getListApproved(modelDetail.SUMARY, db, list);

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    modelDetail.USERS = js.Serialize(db.Form_User.Select(r => new { name = r.CODE, username = r.NAME }).ToList());

                    return View(modelDetail);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message.ToString());
                return View();
            }

        }

        private bool checkUserHavePermissionToChangeData(DataContext db, string ticket)
        {
            _sess = Session["user"] as Form_User;
            if (_sess.ROLE_ID == ROLE.ReadOnly) return false;
            var modelDetail = new LCADetailModel();
            modelDetail.SUMARY = db.Form_Summary.Where(m => m.TICKET == ticket).FirstOrDefault();
            modelDetail.PERMISSION = new List<string>();
            modelDetail.SUBMITS = new List<string>();
            var listPermission = db.LCA_PERMISSION.Where(m => m.ITEM_COLUMN_PERMISSION == (modelDetail.SUMARY.PROCEDURE_INDEX + 1).ToString()
                   && m.PROCESS == modelDetail.SUMARY.PROCESS_ID).ToList();
            foreach (var permission in listPermission)
            {
                modelDetail.PERMISSION.Add(permission.ITEM_COLUMN);
            }

            var userApprover = db.Form_Procedures.Where(m => m.FORM_INDEX == (modelDetail.SUMARY.PROCEDURE_INDEX + 1)
                               && m.FORM_NAME == modelDetail.SUMARY.PROCESS_ID && m.TICKET == modelDetail.SUMARY.TICKET).ToList();
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
            var isEditQuote = listPermission.Where(m => m.DEPT == _sess.DEPT && m.ITEM_COLUMN == PERMISSION.QUOTE).FirstOrDefault();
            if (isEditQuote != null)
            {
                modelDetail.SUBMITS.Add(SUBMIT.EDIT_QUOTE);

            }
            if (modelDetail.SUBMITS.Count <= 0) return false;
            else return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Details(string status, string quotes, LCA_FORM_01 infoTicket, string listFiles,string listFilesQuote)
        {
            try
            {

                using (var db = new DataContext())
                {
                    var formDb = db.LCA_FORM_01.Where(m => m.TICKET == infoTicket.TICKET).OrderByDescending(m => m.ORDER_HISTORY).FirstOrDefault();

                    if (formDb == null)
                    {
                        return Json(new { result = STATUS.ERROR }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        if (formDb.ID != infoTicket.ID)
                        {
                            return Json(new { result = STATUS.WAIT }, JsonRequestBehavior.AllowGet);
                        }

                        if (checkUserHavePermissionToChangeData(db, formDb.TICKET) == false)
                        {
                            return Json(new { result = STATUS.ERROR, message = "You not have permission to edit this ticket!" }, JsonRequestBehavior.AllowGet);
                        }

                        if (status == STATUS.ACCEPT)
                        {
                            var result = Accept(formDb, db, infoTicket, quotes, listFiles,listFilesQuote);
                            if (result.Item1 == STATUS.ERROR)
                            {
                                return Json(new { result = STATUS.ERROR, message = result.Item2 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    result = STATUS.SUCCESS,
                                    ticket = formDb.TICKET,
                                    typeMail = STATUS.ACCEPT
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (status == STATUS.REJECT)
                        {
                            var result = Reject(formDb, db, infoTicket);
                            if (result.Item1 == STATUS.ERROR)
                            {
                                return Json(new { result = STATUS.ERROR, message = result.Item2 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    result = STATUS.SUCCESS,
                                    ticket = formDb.TICKET,
                                    typeMail = STATUS.REJECT
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (status == STATUS.EDIT_QUOTE)
                        {
                            var result = EditQuote(formDb, db, infoTicket, quotes, listFiles,listFilesQuote);
                            if (result.Item1 == STATUS.ERROR)
                            {
                                return Json(new { result = STATUS.ERROR, message = result.Item2 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    result = STATUS.SUCCESS,
                                    ticket = formDb.TICKET,
                                    typeMail = STATUS.EDIT_QUOTE
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { result = STATUS.ERROR, message = "Not have this status!" }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
            }
            catch (Exception e)
            {

                ModelState.AddModelError("Error", e.Message.ToString());
                return Json(new { result = STATUS.ERROR, message = e.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        private Tuple<string, string> Reject(LCA_FORM_01 formDb, DataContext db, LCA_FORM_01 infoTicket)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {

                    var form = formDb.CloneObject() as LCA_FORM_01;
                    _sess = Session["user"] as Form_User;
                    var summary = db.Form_Summary.Where(m => m.TICKET == formDb.TICKET).FirstOrDefault();

                    // để lưu lại bước sẽ quay lại sau khi luồng reject được thực hiện xong
                    summary.REJECT_INDEX = form.PROCEDURE_INDEX;

                    var process = db.Form_Process.Where(m => m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1) && m.FORM_NAME == summary.PROCESS_ID).FirstOrDefault();
                    if (process == null)
                    {
                        ModelState.AddModelError("Error", "Error System!!!");
                        return Tuple.Create<string, string>(STATUS.ERROR, "Check reject process again!");
                    }

                    form.PROCEDURE_INDEX = process.RETURN_INDEX is int returnIndex ? returnIndex - 1 : 0;
                    form.ORDER_HISTORY += 1;
                    form.IS_SIGNATURE = 0;
                    form.ID = Guid.NewGuid().ToString();
                    form.SUBMIT_USER = _sess.CODE;
                    form.COMMENT = infoTicket.COMMENT;
                    var saveQuote = AddQuotes("", db, formDb, form);
                    if (saveQuote.Item1 == STATUS.ERROR)
                    {
                        transaction.Rollback();
                        return saveQuote;
                    }
                    db.LCA_FORM_01.Add(form);

                    summary.IS_REJECT = true;

                    // Để lưu lại bước bị reject về
                    summary.RETURN_TO = form.PROCEDURE_INDEX;
                    summary.PROCEDURE_INDEX = form.PROCEDURE_INDEX;

                    db.SaveChanges();
                    transaction.Commit();
                    if (!MailResponsitory.SendMail(summary, STATUS.REJECT, "LCA"))
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("Error", "Error when send mail!");
                        return Tuple.Create<string, string>(STATUS.ERROR, "Error when send mail!");
                    };
                    return Tuple.Create<string, string>(STATUS.SUCCESS, "");
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("Error", e.Message.ToString());
                    return Tuple.Create<string, string>(STATUS.ERROR, e.Message.ToString());
                }

            }
        }
        private Tuple<string, string> AddQuotes(string quotes, DataContext db, LCA_FORM_01 prevTicket, LCA_FORM_01 currentTicket, string status = null)
        {
            try
            {
                List<LCA_QUOTE> lcaQuotes = new List<LCA_QUOTE>();
                // không sửa giá
                if (string.IsNullOrEmpty(quotes))
                {

                    lcaQuotes = db.LCA_QUOTE.Where(m => m.ID_TICKET == prevTicket.ID).ToList();
                }

                // Khi sửa đổi quotes
                else
                {
                    lcaQuotes = JsonConvert.DeserializeObject<List<LCA_QUOTE>>(quotes);
                }
                if (lcaQuotes == null || lcaQuotes.Count == 0)
                {
                    return Tuple.Create<string, string>(STATUS.ERROR, "Check quote info again!");
                }
                foreach (var quote in lcaQuotes)
                {
                    if(quote.QUANTITY == 0)
                    {
                        return Tuple.Create<string, string>(STATUS.ERROR, "Item  " + quote.NO + " not have quantity!");
                    }
                    if (status == STATUS.EDIT_QUOTE && quote.LCA_UNIT_PRICE == 0)
                        return Tuple.Create<string, string>(STATUS.ERROR, "Item  " + quote.NO + " not have price!");
                    var quoteDb = new LCA_QUOTE
                    {
                        ID_TICKET = currentTicket.ID,
                        NO = quote.NO,
                        REQUEST_ITEM = quote.REQUEST_ITEM,
                        LCA_UNIT_PRICE = quote.LCA_UNIT_PRICE,
                        LCA_TOTAL_COST = quote.LCA_TOTAL_COST,
                        CUSTOMER_UNIT_PRICE = quote.CUSTOMER_UNIT_PRICE,
                        CUSTOMER_TOTAL_COST = quote.CUSTOMER_TOTAL_COST,
                        QUANTITY = quote.QUANTITY
                    };
                    db.LCA_QUOTE.Add(quoteDb);
                }
                db.SaveChanges();

                return Tuple.Create<string, string>(STATUS.SUCCESS, "");
            }
            catch (Exception e)
            {
                return Tuple.Create<string, string>(STATUS.ERROR, e.Message.ToString());
            }


        }
        private Tuple<string, string> AddFilesToForm(string listFiles, LCA_FORM_01 form, DataContext db, string type = null)
        {
            if (string.IsNullOrEmpty(listFiles))
            {
                return Tuple.Create<string, string>(STATUS.SUCCESS, "");
            }
            var list = JsonConvert.DeserializeObject<List<LCA_FILE>>(listFiles);
            if (list != null)
            {
                foreach (var file in list)
                {
                    var lcaFile = new LCA_FILE();
                    lcaFile.TICKET = form.TICKET;
                    lcaFile.ID_TICKET = form.ID;
                    lcaFile.FILE_URL = file.FILE_URL;
                    lcaFile.FILE_NAME = file.FILE_NAME;
                    lcaFile.FILE_TYPE = type;
                    string fullPath = Request.MapPath("~" + file.FILE_URL);
                    if (System.IO.File.Exists(fullPath))
                    {
                        db.LCA_FILE.Add(lcaFile);
                    }
                    else
                    {
                        return Tuple.Create<string, string>(STATUS.ERROR, "File " + file.FILE_URL + " upload not success!");
                    }

                }
                return Tuple.Create<string, string>(STATUS.SUCCESS, "");
            }
            else
            {
                return Tuple.Create<string, string>(STATUS.ERROR, "File upload not success!");
            }
        }

        private Tuple<string, string> Accept(LCA_FORM_01 formDb, DataContext db, LCA_FORM_01 infoTicket, string quotes, string listFiles, string listFileQuote)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var form = formDb.CloneObject() as LCA_FORM_01;

                    _sess = Session["user"] as Form_User;
                    var summary = db.Form_Summary.Where(m => m.TICKET == formDb.TICKET).FirstOrDefault();
                    var listPermission = db.LCA_PERMISSION.Where(m => m.ITEM_COLUMN_PERMISSION == (form.PROCEDURE_INDEX + 1).ToString()
                                                                 && m.PROCESS == summary.PROCESS_ID).ToList();
                    #region FORM
                    if (summary.IS_REJECT)
                    {
                        var stationByIndex = db.Form_Process.Where(m => m.FORM_INDEX == (form.PROCEDURE_INDEX + 1) && m.FORM_NAME == summary.PROCESS_ID).FirstOrDefault();
                        if (stationByIndex != null)
                        {
                            var stationIsSinging = db.LCA_FORM_01.Where(m => m.TICKET == form.TICKET
                            && m.STATION_NAME.Trim().Replace("\n", "").Replace("\r", "") == stationByIndex.STATION_NAME.Trim().Replace("\n", "").Replace("\r", "")
                            && m.IS_SIGNATURE == 1).FirstOrDefault();
                            if (stationIsSinging == null)
                            {
                                form.IS_SIGNATURE = 1;
                            }
                            else form.IS_SIGNATURE = 0;
                        }
                        else
                        {
                            return Tuple.Create<string, string>(STATUS.ERROR, "Kiểm tra lại form reject process");
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
                    form.ORDER_HISTORY += 1;
                    form.UPD_DATE = DateTime.Now;
                    form.SUBMIT_USER = _sess.CODE;
                    form.ID = Guid.NewGuid().ToString();

                    if (listPermission.Where(m => m.ITEM_COLUMN == PERMISSION.QUOTE).FirstOrDefault() != null)
                    {
                        form.RECEIVE_DATE = infoTicket.RECEIVE_DATE;
                        if(infoTicket.LEAD_TIME_NUMBER == 0)
                        {
                            return Tuple.Create<string, string>(STATUS.ERROR, "Lead time không được bằng 0");
                        }
                           
                        form.LEAD_TIME_NUMBER = infoTicket.LEAD_TIME_NUMBER;

                    }
                    if (listPermission.Where(m => m.ITEM_COLUMN == PERMISSION.ADD_ID).FirstOrDefault() != null)
                    {
                        form.LCA_ID = infoTicket.LCA_ID;
                    }
                    if (listPermission.Where(m => m.ITEM_COLUMN == PERMISSION.COMMENT).FirstOrDefault() != null)
                    {
                        form.COMMENT = infoTicket.COMMENT;
                    }
                    if (listPermission.Where(m => m.ITEM_COLUMN == PERMISSION.EDIT_INFO).FirstOrDefault() != null)
                    {
                        form.REQUEST_DATE = infoTicket.REQUEST_DATE;
                        form.TARGET_DATE = infoTicket.TARGET_DATE;
                        form.REQUEST_TARGET = infoTicket.REQUEST_TARGET;
                        form.CONTENT_ERROR = infoTicket.CONTENT_ERROR;
                        form.ERROR_RATE_CURRENT = infoTicket.ERROR_RATE_CURRENT;
                        form.IMPROVED_EFICIENCY = infoTicket.IMPROVED_EFICIENCY;
                        form.COST_SAVING = infoTicket.COST_SAVING;
                        form.DECREASE_PERSON = infoTicket.DECREASE_PERSON;
                        form.OTHER = infoTicket.OTHER;
                        form.PAYER = infoTicket.PAYER;
                        form.PCB = infoTicket.PCB;
                        form.CUSTOMER = infoTicket.CUSTOMER;
                        form.MODEL = infoTicket.MODEL;
                        form.REQUEST_CONTENT = infoTicket.REQUEST_CONTENT;

                    }
                    #region Quote
                    var saveQuote = AddQuotes(quotes, db, infoTicket, form);
                    if (saveQuote.Item1 == STATUS.ERROR)
                    {
                        transaction.Rollback();
                        return saveQuote;
                    }
                    #endregion
                    #region Files
                    var saveFile = AddFilesToForm(listFiles, form, db);
                    if (saveFile.Item1 == STATUS.ERROR)
                    {
                        transaction.Rollback();
                        return saveFile;
                    }
                    var saveFileQuote = AddFilesToForm(listFileQuote, form, db,FILE_TYPE.QUOTED);
                    if (saveFileQuote.Item1 == STATUS.ERROR)
                    {
                        transaction.Rollback();
                        return saveFileQuote;
                    }
                    #endregion


                    var process = db.Form_Procedures.Where(m => m.TICKET == form.TICKET).ToList();
                    form.STATION_NAME = process.Where(m => m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1)).FirstOrDefault().STATION_NAME;
                    form.STATION_NO = process.Where(m => m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1)).FirstOrDefault().STATION_NO;
                   
                    #endregion

                    #region SUMARY

                    summary.PROCEDURE_INDEX = form.PROCEDURE_INDEX;
                    summary.UPD_DATE = DateTime.Now;
                    if (form.PROCEDURE_INDEX == summary.LAST_INDEX)
                    {
                        summary.IS_FINISH = true;
                        form.LEAD_TIME = form.UPD_DATE.AddDays(form.LEAD_TIME_NUMBER);
                    }

                    db.LCA_FORM_01.Add(form);
                    #endregion

                    // Thay đổi payer => change process
                    if (formDb.PAYER != form.PAYER)
                    {
                        summary = changeProcessWhenChangePayer(formDb.PAYER, form.PAYER, db, summary);
                        if (summary == null)
                        {
                            transaction.Rollback();
                            ModelState.AddModelError("Error", "Bạn không thể thay đổi thành chi trả theo " + form.PAYER + " được!");
                            return Tuple.Create<string, string>(STATUS.ERROR, "Bạn không thể thay đổi thành chi trả theo " + form.PAYER + " được!");

                        }
                        if (!summary.IS_REJECT)
                        {
                            form.PROCEDURE_INDEX = summary.REJECT_INDEX;
                            summary.PROCEDURE_INDEX = summary.REJECT_INDEX;
                        }

                    }
                    db.SaveChanges();
                    transaction.Commit();

                    if (!MailResponsitory.SendMail(summary, STATUS.ACCEPT, "LCA"))
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("Error", "Gửi mail bị lỗi");
                        return Tuple.Create<string, string>(STATUS.ERROR, "Gửi mail bị lỗi");
                    };
                    return Tuple.Create<string, string>(STATUS.SUCCESS, "");
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("Error", e.Message.ToString());
                    return Tuple.Create<string, string>(STATUS.ERROR, e.Message.ToString());
                }

            }
        }
        private Form_Summary changeProcessWhenChangePayer(string payerOld, string payerNew, DataContext db, Form_Summary summary)
        {
            try
            {
                var processOld = db.Form_Process.Where(m => m.FORM_NAME == (payerOld == PAYER.CUSTOMER ? Constant.LCA_Process : Constant.LCA_01)).ToList();
                var processNew = db.Form_Process.Where(m => m.FORM_NAME == (payerNew == PAYER.CUSTOMER ? Constant.LCA_Process : Constant.LCA_01)).ToList();
                var stationReturnToOld = processOld.Where(m => m.FORM_INDEX == (summary.RETURN_TO + 1)).FirstOrDefault();
                var stationReTurnToNew = processNew.Where(m => m.STATION_NAME == stationReturnToOld.STATION_NAME).FirstOrDefault();
                var stationRejectOld = processOld.Where(m => m.FORM_INDEX == (summary.REJECT_INDEX + 1)).FirstOrDefault();
                var stationRejectNew = processNew.Where(m => m.STATION_NAME.Trim().Replace("\n", "").Replace("\r", "") == stationRejectOld.STATION_NAME.Trim().Replace("\n", "").Replace("\r", "")).FirstOrDefault();

                summary.RETURN_TO = stationReTurnToNew.FORM_INDEX - 1;
                summary.REJECT_INDEX = stationRejectNew.FORM_INDEX - 1;
                summary.PROCESS_ID = stationRejectNew.FORM_NAME;
                summary.LAST_INDEX = processNew.Count() - 1;

                var proceduce = db.Form_Procedures.Where(m => m.TICKET == summary.TICKET).ToList();
                setUpFormProceduce(stationRejectNew.FORM_NAME, db, summary.TICKET, "", processNew);
                return summary;
            }
            catch (Exception e)
            {
                return null;
            }


        }
        private Tuple<string, string> EditQuote(LCA_FORM_01 formDb, DataContext db, LCA_FORM_01 infoTicket, string quotes, string listFiles,string listFileQuote)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var form = formDb.CloneObject() as LCA_FORM_01;

                    _sess = Session["user"] as Form_User;
                    var summary = db.Form_Summary.Where(m => m.TICKET == formDb.TICKET).FirstOrDefault();

                    #region FORM

                    form.ORDER_HISTORY += 1;
                    form.UPD_DATE = DateTime.Now;
                    form.SUBMIT_USER = _sess.CODE;
                    form.IS_SIGNATURE = 0;
                    form.LCA_ID = infoTicket.LCA_ID;
                    form.ID = Guid.NewGuid().ToString();
                    if (infoTicket.LEAD_TIME_NUMBER == 0)
                    {
                        return Tuple.Create<string, string>(STATUS.ERROR, "Lead time must > 0");
                    }
                    form.LEAD_TIME_NUMBER = infoTicket.LEAD_TIME_NUMBER;
                    form.RECEIVE_DATE = infoTicket.RECEIVE_DATE;
                    #region Quote
                    var saveQuote = AddQuotes(quotes, db, infoTicket, form, STATUS.EDIT_QUOTE);
                    if (saveQuote.Item1 == STATUS.ERROR)
                    {
                        return saveQuote;
                    }

                    #endregion
                    #region Files
                    var saveFile = AddFilesToForm(listFiles, form, db);
                    if (saveFile.Item1 == STATUS.ERROR)
                    {
                        transaction.Rollback();
                        return saveFile;
                    }
                    var saveFileQuote = AddFilesToForm(listFileQuote, form, db, FILE_TYPE.QUOTED);
                    if (saveFileQuote.Item1 == STATUS.ERROR)
                    {
                        transaction.Rollback();
                        return saveFileQuote;
                    }
                    #endregion
                    var process = db.Form_Procedures.Where(m => m.TICKET == form.TICKET).ToList();
                    form.STATION_NAME = process.Where(m => m.FORM_INDEX == form.PROCEDURE_INDEX).FirstOrDefault().STATION_NAME;
                    form.STATION_NO = process.Where(m => m.FORM_INDEX == form.PROCEDURE_INDEX).FirstOrDefault().STATION_NO;
                    db.LCA_FORM_01.Add(form);
                    #endregion

                    #region SUMARY

                    summary.UPD_DATE = DateTime.Now;
                    summary.STATUS = STATUS.QUOTED;
                    #endregion
                    db.SaveChanges();
                    transaction.Commit();
                    if (!MailResponsitory.SendMail(summary, STATUS.EDIT_QUOTE, "LCA"))
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("Error", "Error when send mail!");
                        return Tuple.Create<string, string>(STATUS.ERROR, "Error when send mail!");
                    };
                    return Tuple.Create<string, string>(STATUS.SUCCESS, "");
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("Error", e.Message.ToString());
                    return Tuple.Create<string, string>(STATUS.ERROR, e.Message.ToString());
                }

            }
        }

        public ActionResult PrintView(string ticket)
        {
            try
            {
                if (string.IsNullOrEmpty(ticket))
                {
                    return HttpNotFound();
                }
                using (var db = new DataContext())
                {
                    _sess = Session["user"] as Form_User;
                    var modelDetail = new LCADetailModel();
                    var list = db.LCA_FORM_01.Where(m => m.TICKET == ticket).OrderByDescending(m => m.ORDER_HISTORY).ToList();
                    modelDetail.TICKET = list.FirstOrDefault();

                    if (modelDetail.TICKET == null)
                    {
                        return HttpNotFound();
                    }

                    modelDetail.SUMARY = db.Form_Summary.Where(m => m.TICKET == ticket).FirstOrDefault();
                    if (modelDetail.SUMARY == null)
                    {
                        return HttpNotFound();
                    }

                    modelDetail.TICKET.FILES = db.LCA_FILE.Where(m => m.TICKET == modelDetail.TICKET.TICKET).ToList();
                    modelDetail.TICKET.LCA_QUOTEs = db.LCA_QUOTE.Where(m => m.ID_TICKET == modelDetail.TICKET.ID).ToList();

                    modelDetail.STATION_APPROVE = getListApproved(modelDetail.SUMARY, db, list);
                    modelDetail.LIST_COMMENT = db.Form_Comment.Where(m => m.TICKET == modelDetail.TICKET.TICKET).OrderBy(m => m.UPD_DATE).ToList();
                    return View(modelDetail);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message.ToString());
                return View();
            }
        }

        private string validateTicket(LCA_FORM_01 ticket)
        {
            if (string.IsNullOrEmpty(ticket.PAYER))
            {
                return "Cần chọn một hình thức thanh toán";
            }
            if (string.IsNullOrEmpty(ticket.PURPOSE))
            {
                return "Cần điền vào ô mục đích";
            }
            if (string.IsNullOrEmpty(ticket.REQUEST_TARGET))
            {
                return "Cần chọn ít nhất một mục đích yêu cầu";
            }
            if (string.IsNullOrEmpty(ticket.REQUEST_CONTENT))
            {
                return "Cần điền nội dung yêu cầu";
            }
            return "";
        }
    }
}