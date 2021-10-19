using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;
using Bet.Util;
using System.Data.Entity.Validation;
using System.Threading.Tasks;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [CustomAuthorize("Normal")]
    [NoCache]
    public class PurAccF06Controller : Controller
    {
        private DataContext db = new DataContext();
        private Form_User _sess = new Form_User();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Details(string ticket)
        {
            try
            {
                if (ticket == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var entity = PurAccF06Repository.GetForm(ticket);
                if (entity == null)
                {
                    return HttpNotFound();
                }
                var summary = FormSummaryRepository.GetSummary(ticket);
                if (summary == null)
                {
                    return HttpNotFound();
                }

                var flag = IsEdit(summary);
                if (flag)
                {
                    return RedirectToAction("AssetEdit", new { ticket = ticket });
                }
                _sess = Session["user"] as Form_User;
                var allForm = PurAccF06Repository.GetForms(entity.TICKET);
                JavaScriptSerializer js = new JavaScriptSerializer();
                ViewBag.signature = Common.GetSignatures(allForm);
                ViewBag.users = js.Serialize(db.Form_User.Select(r => new { name = r.CODE, username = r.NAME }).ToList());
                ViewBag.accept = IsAccept(summary);
                ViewBag.summary = summary;
                var author = UserRepository.GetUser(summary.CREATE_USER);
                entity.author = new AuthorEntity() { code = author.CODE, fullname = author.NAME, dept = author.DEPT };
                //Kiểm tra xem có phải trưởng phòng và đã ký rồi hay chưa
                var mngSignature = PurAccF06Repository.IsSignature(ticket, 1);
                ViewBag.isMng = DeptRepository.IsMng(_sess.CODE) && !mngSignature;
                List<CostChangeEntity> lst = new List<CostChangeEntity>();
                var temp = allForm.Where(r => r.PROCEDURE_INDEX == 0 && r.CREATE_USER == summary.CREATE_USER).OrderBy(o => o.ORDER_HISTORY).ToList();
                temp.RemoveAt(temp.Count - 1);
                #region Chèn giá cũ (Cần cải tiến code)
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_1",
                    //prices = temp.GroupBy(r => r.UNIT_PRICE_1).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_1)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_1)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_1)
                    .Where(r => r != entity.UNIT_PRICE_1)
                    .Select(h => String.Format("{0:n0}", h)).ToList()

                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_2",
                    //prices = temp.GroupBy(r => r.UNIT_PRICE_2).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_2)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_2)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_2)
                    .Where(r => r != entity.UNIT_PRICE_2)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_3",
                    //prices = temp.GroupBy(r => r.UNIT_PRICE_3).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_3)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_3)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_3)
                    .Where(r => r != entity.UNIT_PRICE_3)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_4",
                    //prices = temp.GroupBy(r => r.UNIT_PRICE_4).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_4)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_4)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_4)
                    .Where(r => r != entity.UNIT_PRICE_4)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_5",
                    // prices = temp.GroupBy(r => r.UNIT_PRICE_5).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_5)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_5)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_5)
                    .Where(r => r != entity.UNIT_PRICE_5)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_6",
                    //prices = temp.GroupBy(r => r.UNIT_PRICE_6).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_6)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_6)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_6)
                    .Where(r => r != entity.UNIT_PRICE_6)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_7",
                    //prices = temp.GroupBy(r => r.UNIT_PRICE_7).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_7)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_7)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_7)
                    .Where(r => r != entity.UNIT_PRICE_7)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_8",
                    //prices = temp.GroupBy(r => r.UNIT_PRICE_8).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_8)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_8)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_8)
                    .Where(r => r != entity.UNIT_PRICE_8)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_9",
                    // prices = temp.GroupBy(r => r.UNIT_PRICE_9).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_9)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_9)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_9)
                    .Where(r => r != entity.UNIT_PRICE_9)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                lst.Add(new CostChangeEntity()
                {

                    unitPrice = "UNIT_PRICE_10",
                    //prices = temp.GroupBy(r => r.UNIT_PRICE_10).Select(h => String.Format("{0:n0}", h.FirstOrDefault().UNIT_PRICE_10)).ToList()
                    prices = temp.GroupBy(r => r.UNIT_PRICE_10)
                    .Select(h => h.FirstOrDefault().UNIT_PRICE_10)
                    .Where(r => r != entity.UNIT_PRICE_10)
                    .Select(h => String.Format("{0:n0}", h)).ToList()
                });
                #endregion
                entity.histories = lst;
                return View(entity);
            }
            catch (Exception ex)
            {
                log.Fatal("Fatal", ex);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        public ActionResult PrintView(string ticket)
        {
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PR_ACC_F06 entity = PurAccF06Repository.GetForm(ticket);
            if (entity == null)
            {
                return HttpNotFound();
            }
            var summary = FormSummaryRepository.GetSummary(ticket);
            if (summary == null)
            {
                return HttpNotFound();
            }

            var allForm = PurAccF06Repository.GetForms(ticket);
            var users = UserRepository.GetUsers();
            ViewBag.summary = summary;
            JavaScriptSerializer js = new JavaScriptSerializer();
            ViewBag.signature = Common.GetSignatures(allForm);
            ViewBag.users = js.Serialize(users.Select(r => new { name = r.CODE, username = r.NAME }).ToList());
            var author = UserRepository.GetUser(summary.CREATE_USER);
            entity.author = new AuthorEntity() { code = author.CODE, fullname = author.NAME, dept = author.DEPT };
            return View(entity);
        }
        public bool IsAccept(Form_Summary summary)
        {
            if (summary.IS_FINISH || summary.IS_REJECT)
            {
                return false;
            }

            var index = summary.PROCEDURE_INDEX + 1;// Tìm index của trạm tiếp theo

            var processNext = ProcessRepository.GetProcess(summary.PROCESS_ID, index);

            if (processNext != null)
            {
                var stationNoNext = processNext.STATION_NO;
                var users = StationRepository.GetStations(stationNoNext);// Tim users approval
                foreach (var user in users)
                {
                    if (user.USER_ID == _sess.CODE)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool IsEdit(Form_Summary summary)
        {
            if (summary.IS_FINISH || summary.IS_REJECT)
            {
                return false;
            }
            var sess = Session["user"] as Form_User;
            var index = summary.PROCEDURE_INDEX + 1;// Tìm index của trạm tiếp theo
            var dept = DeptRepository.GetDept(sess.DEPT);
            var processNext = ProcessRepository.GetProcess(summary.PROCESS_ID, index);
            if (processNext != null)
            {
                var stationNoNext = processNext.STATION_NO;
                var users = StationRepository.GetStations(stationNoNext);// Tim users approval

                foreach (var user in users)
                {
                    if (user.USER_ID == sess.CODE && dept.DEPT.ToUpper().Equals("ASSET"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [HttpPost]
        public async Task<JsonResult> Accept(string ticket, string list, bool usePur)
        {
            string msg = "";
            try
            {
                List<AssetEntity> assetEntity = (List<AssetEntity>)Newtonsoft.Json.JsonConvert.DeserializeObject(list, typeof(List<AssetEntity>));
                //return Json(new { msg = "abc" }, JsonRequestBehavior.AllowGet);
                _sess = Session["user"] as Form_User;
                var formSummary = db.Form_Summary.FirstOrDefault(r => r.TICKET == ticket);
                var form = db.PR_ACC_F06.Where(r => r.TICKET == ticket).OrderByDescending(h => h.ORDER_HISTORY).FirstOrDefault();
                var f = form.CloneObject() as PR_ACC_F06;

                f.ORDER_HISTORY = f.ORDER_HISTORY + 1;
                if (!formSummary.IS_REJECT)
                {
                    if (formSummary.PROCEDURE_INDEX == 0 && formSummary.RETURN_TO > 0)
                    {
                        f.PROCEDURE_INDEX = formSummary.RETURN_TO;
                    }
                    else
                    {

                        f.PROCEDURE_INDEX = form.PROCEDURE_INDEX + 1;
                    }
                }
                else
                {
                    var lastForm = db.PR_ACC_F06.Where(r => r.TICKET == form.TICKET).OrderByDescending(h => h.ORDER_HISTORY).FirstOrDefault();
                    f.PROCEDURE_INDEX = lastForm.PROCEDURE_INDEX;
                }
                var totalStep = db.Form_Process.Where(r => r.FORM_NAME == form.FORM_NAME).Max(h => h.FORM_INDEX);

                f.CREATE_USER = _sess.CODE;
                f.UPD_DATE = DateTime.Now;
                f.ID = Guid.NewGuid().ToString();
                //xem lai
                f.IS_SIGNATURE = true;
                foreach (var item in assetEntity)
                {
                    switch (item.assetIndex)
                    {
                        case 1:
                            f.AK_1 = item.assetType;
                            f.ACOUNT_CODE_1 = item.accountCode;
                            f.ASSET_NO_1 = item.assetNo;
                            f.COST_CENTER_1 = item.costCenter;
                            break;
                        case 2:
                            f.AK_2 = item.assetType;
                            f.ACOUNT_CODE_2 = item.accountCode;
                            f.ASSET_NO_2 = item.assetNo;
                            f.COST_CENTER_2 = item.costCenter;
                            break;
                        case 3:
                            f.AK_3 = item.assetType;
                            f.ACOUNT_CODE_3 = item.accountCode;
                            f.ASSET_NO_3 = item.assetNo;
                            f.COST_CENTER_3 = item.costCenter;
                            break;
                        case 4:
                            f.AK_4 = item.assetType;
                            f.ACOUNT_CODE_4 = item.accountCode;
                            f.ASSET_NO_4 = item.assetNo;
                            f.COST_CENTER_4 = item.costCenter;
                            break;
                        case 5:
                            f.AK_5 = item.assetType;
                            f.ACOUNT_CODE_5 = item.accountCode;
                            f.ASSET_NO_5 = item.assetNo;
                            f.COST_CENTER_5 = item.costCenter;
                            break;
                        case 6:
                            f.AK_6 = item.assetType;
                            f.ACOUNT_CODE_6 = item.accountCode;
                            f.ASSET_NO_6 = item.assetNo;
                            f.COST_CENTER_6 = item.costCenter;
                            break;
                        case 7:
                            f.AK_7 = item.assetType;
                            f.ACOUNT_CODE_7 = item.accountCode;
                            f.ASSET_NO_7 = item.assetNo;
                            f.COST_CENTER_7 = item.costCenter;
                            break;
                        case 8:
                            f.AK_8 = item.assetType;
                            f.ACOUNT_CODE_8 = item.accountCode;
                            f.ASSET_NO_8 = item.assetNo;
                            f.COST_CENTER_8 = item.costCenter;
                            break;
                        case 9:
                            f.AK_9 = item.assetType;
                            f.ACOUNT_CODE_9 = item.accountCode;
                            f.ASSET_NO_9 = item.assetNo;
                            f.COST_CENTER_9 = item.costCenter;
                            break;
                        case 10:
                            f.AK_10 = item.assetType;
                            f.ACOUNT_CODE_10 = item.accountCode;
                            f.ASSET_NO_10 = item.assetNo;
                            f.COST_CENTER_10 = item.costCenter;
                            break;
                        default:
                            break;
                    }
                }
                using (DataContext context = new DataContext())
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var modelState = ModelState.Values.Where(r => r.Errors.Count > 0).ToList();
                            if (modelState.Count == 0)
                            {
                                if(usePur == true)
                                {
                                    formSummary.USE_PUR = true;
                                }
                                if (!formSummary.USE_PUR && DeptRepository.IsMng(_sess.CODE))
                                {
                                    formSummary.LAST_INDEX = 4;
                                }
                                
                                if ((formSummary.PROCEDURE_INDEX == formSummary.LAST_INDEX - 1) && formSummary.RETURN_TO == 0)
                                {
                                    formSummary.IS_FINISH = true;
                                }
                                // Trưởng bộ phận gửi thẳng đến người Reject
                                if (formSummary.PROCEDURE_INDEX == 0 && formSummary.RETURN_TO > 0)
                                {
                                    formSummary.PROCEDURE_INDEX = formSummary.RETURN_TO;
                                    formSummary.RETURN_TO = 0;
                                }
                                else
                                {
                                    formSummary.PROCEDURE_INDEX = f.PROCEDURE_INDEX;
                                }
                                db.Entry<Form_Summary>(formSummary).State = EntityState.Modified;
                                db.SaveChanges();
                                db.PR_ACC_F06.Add(f);
                                db.SaveChanges();
                                transaction.Commit();
                                msg = "OK";
                            }
                            var process = ProcessRepository.GetProcess(formSummary.PROCESS_ID, f.PROCEDURE_INDEX + 1);
                            var stations = StationRepository.GetStations(process.STATION_NO);
                            var userID = stations.Select(r => r.USER_ID).ToList();
                            var userMails = UserRepository.GetUsers(userID);

                            var dear = "Dear All !";
                            if (userMails.Count == 1)
                            {
                                var userApproval = UserRepository.GetUser(userID.FirstOrDefault());
                                dear = $"Dear {userApproval.SHORT_NAME} san !";
                            }

                            string body = $@"
                                                <h3>{dear}</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='http://172.28.10.17:90/PurAccF06/Details?ticket={f.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                            await Business.MailHelper.SenMailOutlookAsync(userMails, body);
                        }
                        catch (Exception ex)
                        {
                            msg = "Err";
                            Debug.WriteLine(ex.Message);
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                log.Fatal("Fatal", ex);
            }

            return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public async Task<JsonResult> Reject(string ticket)
        {
            string msg = "";
            try
            {
                var sess = Session["user"] as Form_User;
                var form = PurAccF06Repository.GetForm(ticket);
                int index = form.PROCEDURE_INDEX + 1;


                using (DataContext context = new DataContext())
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {

                        try
                        {
                            #region Save entity
                            var f = form.CloneObject() as PR_ACC_F06;
                            f.ORDER_HISTORY = form.ORDER_HISTORY + 1;
                            f.CREATE_USER = sess.CODE;
                            f.UPD_DATE = DateTime.Now;
                            f.ID = Guid.NewGuid().ToString();
                            f.IS_SIGNATURE = false;
                            f.PROCEDURE_INDEX = index;
                            db.PR_ACC_F06.Add(f);
                            db.SaveChanges();
                            #endregion
                            #region Save Summary
                            var summary = db.Form_Summary.FirstOrDefault(r => r.TICKET == form.TICKET);
                            summary.IS_REJECT = true;
                            summary.PROCEDURE_INDEX = index;
                            summary.RETURN_TO = form.PROCEDURE_INDEX;// Nếu xác nhận OK thì chuyển lại bước này

                            db.Entry<Form_Summary>(summary).State = EntityState.Modified;
                            db.SaveChanges();
                            #endregion
                            msg = "OK";
                            transaction.Commit();
                            #region SendMail
                            var userCreate = UserRepository.GetUser(summary.CREATE_USER);
                            string body = $@"
                                                <h3>Dear {userCreate.SHORT_NAME} san !</h3>
                                                <h3 style='color: red' >Request reject. Please click below link view details:</h3>
	                                            <a href='http://172.28.10.17:90/PurAccF06/Details?ticket={f.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                            Task t = Business.MailHelper.SenMailOutlookAsync(userCreate.EMAIL, body);
                            await t;
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            msg = "Err";
                            Debug.WriteLine(ex.Message);
                            transaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                log.Fatal("Fatal", ex);
            }
            return Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", "Home");
        }

        public ActionResult SendToMe()
        {
            var session = Session["user"] as Form_User;
            var list = db.PR_ACC_F06.ToList().GroupBy(r => new { r.TICKET }).Select(h => new PR_ACC_F06()
            {
                TICKET = h.Key.TICKET,
                PROCEDURE_INDEX = h.Max(d => d.PROCEDURE_INDEX),
                UPD_DATE = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.UPD_DATE).FirstOrDefault(),
                CREATE_USER = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.CREATE_USER).FirstOrDefault(),
                TITLE = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.TITLE).FirstOrDefault(),
                ID = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.ID).FirstOrDefault()
            }).ToList();
            List<PR_ACC_F06> accept = new List<PR_ACC_F06>();
            foreach (var form in list)
            {
                var index = form.PROCEDURE_INDEX + 1;
                var processNext = db.Form_Process.FirstOrDefault(r => r.FORM_INDEX == index);
                if (processNext != null)
                {
                    var stationNoNext = processNext.STATION_NO;
                    var user = db.Form_Stations.First(r => r.STATION_NO == stationNoNext);
                    if (user.USER_ID == session.CODE)
                    {
                        accept.Add(form);
                    }
                }
            }
            ViewBag.accepts = accept.Select(r => r.ID);
            return View(accept);
        }

     
        // GET: PurAccF06/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PR_ACC_F06 entity)
        {
            int validate = PurAccF06Repository.Validate(entity);
            string message = "";

            switch (validate)
            {
                case -1:
                    message = "Bạn chưa chọn loại Form";
                    break;
                case -2:
                    message = "Bạn chưa nhập thông tin";
                    break;
                case -3:
                    message = "Bạn chưa nhập ngày yêu cầu giao hàng";
                    break;
                default:
                    _sess = Session["user"] as Form_User;
                    var dateTimeNow = DateTime.Now;
                    var ticket = dateTimeNow.ToString("yyyyMMddHHmmss");
                    PurAccF06Repository.SetAmounts(entity);
                    string formName = PurAccF06Repository.ChooseFormProcess(entity);
                    var a = ModelState.Values.Where(r => r.Errors.Count > 0).ToList();
                    string virtualPath = "";
                    string[] virtualPaths = new string[5];
                    HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                    for (int file = 0; file < files.Count; file++)
                    {
                        HttpPostedFile filedata = files[file];
                        string fileName = filedata.FileName;
                        virtualPath = string.Format("/UploadedFiles/{0}", fileName);
                        virtualPaths[file] = virtualPath;
                        string fullPath = Server.MapPath(virtualPath);
                        if (fileName != "")
                        {
                            filedata.SaveAs(fullPath);
                        }

                    }
                    using (DataContext context = new DataContext())
                    {
                        using (DbContextTransaction transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                context.Database.Log = Console.Write;
                                if (entity.TICKET == null)
                                {
                                    #region Save Summary
                                    Form_Summary summary = new Form_Summary()
                                    {
                                        ID = Guid.NewGuid().ToString(),
                                        IS_FINISH = false,
                                        IS_REJECT = false,
                                        PROCEDURE_INDEX = 0,
                                        TICKET = ticket,
                                        CREATE_USER = _sess.CODE,
                                        UPD_DATE = dateTimeNow,
                                        TITLE = Constant.PR_ACC_F06_TITLE,
                                        PROCESS_ID = formName
                                    };
                                    db.Form_Summary.Add(summary);
                                    db.SaveChanges();
                                    #endregion
                                    #region Save Form
                                    entity.ID = Guid.NewGuid().ToString();
                                    entity.UPD_DATE = dateTimeNow;
                                    entity.ISSUE_DATE = dateTimeNow;
                                    entity.CREATE_USER = _sess.CODE;
                                    entity.FORM_NAME = formName;
                                    entity.STATION_NO = $"{formName}_0";
                                    for (int file = 0; file < files.Count; file++)
                                    {
                                        HttpPostedFile filedata = files[file];
                                        string fileName = filedata.FileName;
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_1) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_1 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_1 = fileName;
                                            }
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_2) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_2 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_2 = fileName;
                                            }
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_3) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_3 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_3 = fileName;
                                            }
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_4) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_4 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_4 = fileName;
                                            }
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_5) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_5 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_5 = fileName;
                                            }
                                        }
                                    }
                                    entity.TICKET = ticket;
                                    entity.IS_SIGNATURE = true;
                                    db.PR_ACC_F06.Add(entity);
                                    db.SaveChanges();
                                    #endregion

                                }
                                else
                                {
                                    #region Updata Summary
                                    var summaryExist = db.Form_Summary.FirstOrDefault(r => r.TICKET == entity.TICKET);
                                    summaryExist.UPD_DATE = DateTime.Now;
                                    summaryExist.PROCEDURE_INDEX = 3;
                                    summaryExist.IS_REJECT = false;
                                    summaryExist.IS_RETURN = false;
                                    db.Entry<Form_Summary>(summaryExist).State = EntityState.Modified;
                                    db.SaveChanges();
                                    #endregion
                                    #region Save Form
                                    var formExist = db.PR_ACC_F06.Where(r => r.TICKET == entity.TICKET).OrderByDescending(o => o.ORDER_HISTORY).FirstOrDefault();
                                    var e = formExist.CloneObject() as PR_ACC_F06;
                                    e.ID = Guid.NewGuid().ToString();
                                    e.UNIT_PRICE_1 = entity.UNIT_PRICE_1;
                                    e.UNIT_PRICE_2 = entity.UNIT_PRICE_2;
                                    e.UNIT_PRICE_3 = entity.UNIT_PRICE_3;
                                    e.UNIT_PRICE_4 = entity.UNIT_PRICE_4;
                                    e.UNIT_PRICE_5 = entity.UNIT_PRICE_5;
                                    e.UNIT_PRICE_6 = entity.UNIT_PRICE_6;
                                    e.UNIT_PRICE_7 = entity.UNIT_PRICE_7;
                                    e.UNIT_PRICE_8 = entity.UNIT_PRICE_8;
                                    e.UNIT_PRICE_9 = entity.UNIT_PRICE_9;
                                    e.UNIT_PRICE_10 = entity.UNIT_PRICE_10;
                                    if (e.QTY_1 != null)
                                    {
                                        e.AMOUNT_1 = e.UNIT_PRICE_1 * e.QTY_1;
                                    }
                                    if (e.QTY_2 != null)
                                    {
                                        e.AMOUNT_2 = e.UNIT_PRICE_2 * e.QTY_2;
                                    }
                                    if (e.QTY_3 != null)
                                    {
                                        e.AMOUNT_3 = e.UNIT_PRICE_3 * e.QTY_3;
                                    }
                                    if (e.QTY_4 != null)
                                    {
                                        e.AMOUNT_4 = e.UNIT_PRICE_4 * e.QTY_4;
                                    }
                                    if (e.QTY_5 != null)
                                    {
                                        e.AMOUNT_5 = e.UNIT_PRICE_5 * e.QTY_5;
                                    }
                                    if (e.QTY_6 != null)
                                    {
                                        e.AMOUNT_6 = e.UNIT_PRICE_6 * e.QTY_6;
                                    }
                                    if (e.QTY_7 != null)
                                    {
                                        e.AMOUNT_7 = e.UNIT_PRICE_7 * e.QTY_7;
                                    }
                                    if (e.QTY_8 != null)
                                    {
                                        e.AMOUNT_8 = e.UNIT_PRICE_8 * e.QTY_8;
                                    }
                                    if (e.QTY_9 != null)
                                    {
                                        e.AMOUNT_9 = e.UNIT_PRICE_9 * e.QTY_9;
                                    }
                                    if (e.QTY_10 != null)
                                    {
                                        e.AMOUNT_10 = e.UNIT_PRICE_10 * e.QTY_10;
                                    }
                                    e.ORDER_HISTORY = e.ORDER_HISTORY + 1;
                                    e.PROCEDURE_INDEX = 3;
                                    db.PR_ACC_F06.Add(e);
                                    db.SaveChanges();
                                    #endregion
                                }

                                transaction.Commit();
                                var dept = DeptRepository.GetDept(_sess.DEPT);
                                var userApproval = UserRepository.GetUser(dept.CODE_MNG);
                                string body = $@"
                                                <h3>Dear {userApproval.SHORT_NAME} san!</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='http://172.28.10.17:90/PurAccF06/Details?ticket={entity.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                                Task sendMail = Business.MailHelper.SenMailOutlookAsync(userApproval.EMAIL, body);
                                await sendMail;
                                return RedirectToAction("Index", "Home", new { type = SendType.SENDTOME });
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error occurred " + ex.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                    break;
            }
            ViewBag.message = message;
            return View(entity);
        }



        public ActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateAsync(PR_ACC_F06 entity)
        {

            int validate = PurAccF06Repository.Validate(entity);
            string message = "";

            switch (validate)
            {
                case -1:
                    message = "Bạn chưa chọn loại Form";
                    break;
                case -2:
                    message = "Bạn chưa nhập thông tin";
                    break;
                case -3:
                    message = "Bạn chưa nhập ngày yêu cầu giao hàng";
                    break;
                default:
                    _sess = Session["user"] as Form_User;
                    var dateTimeNow = DateTime.Now;
                    var ticket = dateTimeNow.ToString("yyyyMMddHHmmss");
                    PurAccF06Repository.SetAmounts(entity);
                    string formName = PurAccF06Repository.ChooseFormProcess(entity);
                    //  var tmp = ModelState.Values.Where(r => r.Errors.Count > 0).ToList();
                    string virtualPath = "";
                    string[] virtualPaths = new string[5];
                    HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                    for (int file = 0; file < files.Count; file++)
                    {
                        HttpPostedFile filedata = files[file];
                        string fileName = filedata.FileName;
                        virtualPath = string.Format("/UploadedFiles/{0}", fileName);
                        virtualPaths[file] = virtualPath;
                        string fullPath = Server.MapPath(virtualPath);
                        if (fileName != "")
                        {
                            filedata.SaveAs(fullPath);
                        }

                    }
                    using (DataContext context = new DataContext())
                    {
                        using (DbContextTransaction transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                context.Database.Log = Console.Write;
                                if (entity.TICKET == null)
                                {
                                    #region Save Summary
                                    Form_Summary summary = new Form_Summary()
                                    {
                                        ID = Guid.NewGuid().ToString(),
                                        IS_FINISH = false,
                                        IS_REJECT = false,
                                        PROCEDURE_INDEX = 0,
                                        TICKET = ticket,
                                        CREATE_USER = _sess.CODE,
                                        UPD_DATE = dateTimeNow,
                                        TITLE = Constant.PR_ACC_F06_TITLE,
                                        PROCESS_ID = formName
                                    };
                                    db.Form_Summary.Add(summary);
                                    db.SaveChanges();
                                    #endregion
                                    #region Save Form
                                    entity.ID = Guid.NewGuid().ToString();
                                    entity.UPD_DATE = dateTimeNow;
                                    entity.ISSUE_DATE = dateTimeNow;
                                    entity.CREATE_USER = _sess.CODE;
                                    entity.FORM_NAME = formName;
                                    entity.STATION_NO = $"{formName}_0";
                                    for (int file = 0; file < files.Count; file++)
                                    {
                                        HttpPostedFile filedata = files[file];
                                        string fileName = filedata.FileName;
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_1) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_1 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_1 = fileName;
                                            }
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_2) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_2 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_2 = fileName;
                                            }
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_3) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_3 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_3 = fileName;
                                            }
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_4) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_4 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_4 = fileName;
                                            }
                                            if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_5) == $"FILE_PATH_{file + 1}")
                                            {
                                                entity.FILE_PATH_5 = WebHelper.BaseUrl + virtualPaths[file];
                                                entity.FILE_NAME_5 = fileName;
                                            }
                                        }
                                    }
                                    entity.TICKET = ticket;
                                    entity.IS_SIGNATURE = true;
                                    db.PR_ACC_F06.Add(entity);
                                    db.SaveChanges();
                                    #endregion

                                }
                                else
                                {
                                    #region Updata Summary
                                    var summaryExist = db.Form_Summary.FirstOrDefault(r => r.TICKET == entity.TICKET);
                                    summaryExist.UPD_DATE = DateTime.Now;
                                    summaryExist.PROCEDURE_INDEX = 3;
                                    summaryExist.IS_REJECT = false;
                                    summaryExist.IS_RETURN = false;
                                    db.Entry<Form_Summary>(summaryExist).State = EntityState.Modified;
                                    db.SaveChanges();
                                    #endregion
                                    #region Save Form
                                    var formExist = db.PR_ACC_F06.Where(r => r.TICKET == entity.TICKET).OrderByDescending(o => o.ORDER_HISTORY).FirstOrDefault();
                                    var e = formExist.CloneObject() as PR_ACC_F06;
                                    e.ID = Guid.NewGuid().ToString();
                                    e.UNIT_PRICE_1 = entity.UNIT_PRICE_1;
                                    e.UNIT_PRICE_2 = entity.UNIT_PRICE_2;
                                    e.UNIT_PRICE_3 = entity.UNIT_PRICE_3;
                                    e.UNIT_PRICE_4 = entity.UNIT_PRICE_4;
                                    e.UNIT_PRICE_5 = entity.UNIT_PRICE_5;
                                    e.UNIT_PRICE_6 = entity.UNIT_PRICE_6;
                                    e.UNIT_PRICE_7 = entity.UNIT_PRICE_7;
                                    e.UNIT_PRICE_8 = entity.UNIT_PRICE_8;
                                    e.UNIT_PRICE_9 = entity.UNIT_PRICE_9;
                                    e.UNIT_PRICE_10 = entity.UNIT_PRICE_10;
                                    if (e.QTY_1 != null)
                                    {
                                        e.AMOUNT_1 = e.UNIT_PRICE_1 * e.QTY_1;
                                    }
                                    if (e.QTY_2 != null)
                                    {
                                        e.AMOUNT_2 = e.UNIT_PRICE_2 * e.QTY_2;
                                    }
                                    if (e.QTY_3 != null)
                                    {
                                        e.AMOUNT_3 = e.UNIT_PRICE_3 * e.QTY_3;
                                    }
                                    if (e.QTY_4 != null)
                                    {
                                        e.AMOUNT_4 = e.UNIT_PRICE_4 * e.QTY_4;
                                    }
                                    if (e.QTY_5 != null)
                                    {
                                        e.AMOUNT_5 = e.UNIT_PRICE_5 * e.QTY_5;
                                    }
                                    if (e.QTY_6 != null)
                                    {
                                        e.AMOUNT_6 = e.UNIT_PRICE_6 * e.QTY_6;
                                    }
                                    if (e.QTY_7 != null)
                                    {
                                        e.AMOUNT_7 = e.UNIT_PRICE_7 * e.QTY_7;
                                    }
                                    if (e.QTY_8 != null)
                                    {
                                        e.AMOUNT_8 = e.UNIT_PRICE_8 * e.QTY_8;
                                    }
                                    if (e.QTY_9 != null)
                                    {
                                        e.AMOUNT_9 = e.UNIT_PRICE_9 * e.QTY_9;
                                    }
                                    if (e.QTY_10 != null)
                                    {
                                        e.AMOUNT_10 = e.UNIT_PRICE_10 * e.QTY_10;
                                    }
                                    e.ORDER_HISTORY = e.ORDER_HISTORY + 1;
                                    e.PROCEDURE_INDEX = 3;
                                    db.PR_ACC_F06.Add(e);
                                    db.SaveChanges();
                                    #endregion
                                }

                                transaction.Commit();
                                var dept = DeptRepository.GetDept(_sess.DEPT);
                                var userApproval = UserRepository.GetUser(dept.CODE_MNG);
                                string body = $@"
                                                <h3>Dear {userApproval.SHORT_NAME} san!</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='http://172.28.10.17:90/PurAccF06/Details?ticket={entity.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                                Task sendMail = Business.MailHelper.SenMailOutlookAsync(userApproval.EMAIL, body);
                                await sendMail;
                                //return RedirectToAction("Index", "Home", new { type = SendType.SENDTOME });
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error occurred " + ex.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                    message = $"Create Ticket: {ticket} sucess!";
                    break;
            }
            ViewBag.message = message;

            //System.Threading.Thread.Sleep(1000);
            return Json(message);
        }



        public ActionResult AssetEdit(string ticket)
        {
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var summary = FormSummaryRepository.GetSummary(ticket);
            if (summary == null)
            {
                return HttpNotFound();
            }
            var sess = Session["user"] as Form_User;
            var dept = DeptRepository.GetDept(sess.DEPT);
            var flag = IsEdit(summary);
            if (!flag)
            {
                return HttpNotFound();
            }
            //PR_ACC_F06 entity = db.PR_ACC_F06.Where(r => r.TICKET == ticket).OrderByDescending(h => h.ORDER_HISTORY).FirstOrDefault();
            var entity = PurAccF06Repository.GetForm(ticket);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssetEdit(PR_ACC_F06 pR_ACC_F06)
        {
            var sess = Session["user"] as Form_User;
            var entities = PurAccF06Repository.GetForms(pR_ACC_F06.TICKET);
            var first = entities.OrderBy(o => o.ORDER_HISTORY).FirstOrDefault();
            var last = entities.OrderByDescending(o => o.ORDER_HISTORY).FirstOrDefault();
            if (last.CREATE_USER != sess.CODE)
            {
                var entity = last.CloneObject() as PR_ACC_F06;
                entity.COST_CENTER_1 = pR_ACC_F06.COST_CENTER_1;
                entity.COST_CENTER_2 = pR_ACC_F06.COST_CENTER_2;
                entity.COST_CENTER_3 = pR_ACC_F06.COST_CENTER_3;
                entity.COST_CENTER_4 = pR_ACC_F06.COST_CENTER_4;
                entity.COST_CENTER_5 = pR_ACC_F06.COST_CENTER_5;
                entity.COST_CENTER_6 = pR_ACC_F06.COST_CENTER_6;
                entity.COST_CENTER_7 = pR_ACC_F06.COST_CENTER_7;
                entity.COST_CENTER_8 = pR_ACC_F06.COST_CENTER_8;
                entity.COST_CENTER_9 = pR_ACC_F06.COST_CENTER_9;
                entity.COST_CENTER_10 = pR_ACC_F06.COST_CENTER_10;
                entity.AK_1 = pR_ACC_F06.AK_1;
                entity.AK_2 = pR_ACC_F06.AK_2;
                entity.AK_3 = pR_ACC_F06.AK_3;
                entity.AK_4 = pR_ACC_F06.AK_4;
                entity.AK_5 = pR_ACC_F06.AK_5;
                entity.AK_6 = pR_ACC_F06.AK_6;
                entity.AK_7 = pR_ACC_F06.AK_7;
                entity.AK_8 = pR_ACC_F06.AK_8;
                entity.AK_9 = pR_ACC_F06.AK_9;
                entity.AK_10 = pR_ACC_F06.AK_10;

                entity.ACOUNT_CODE_1 = pR_ACC_F06.ACOUNT_CODE_1;
                entity.ACOUNT_CODE_2 = pR_ACC_F06.ACOUNT_CODE_2;
                entity.ACOUNT_CODE_3 = pR_ACC_F06.ACOUNT_CODE_3;
                entity.ACOUNT_CODE_4 = pR_ACC_F06.ACOUNT_CODE_4;
                entity.ACOUNT_CODE_5 = pR_ACC_F06.ACOUNT_CODE_5;
                entity.ACOUNT_CODE_6 = pR_ACC_F06.ACOUNT_CODE_6;
                entity.ACOUNT_CODE_7 = pR_ACC_F06.ACOUNT_CODE_7;
                entity.ACOUNT_CODE_8 = pR_ACC_F06.ACOUNT_CODE_8;
                entity.ACOUNT_CODE_9 = pR_ACC_F06.ACOUNT_CODE_9;
                entity.ACOUNT_CODE_10 = pR_ACC_F06.ACOUNT_CODE_10;

                entity.ASSET_NO_1 = pR_ACC_F06.ASSET_NO_1;
                entity.ASSET_NO_2 = pR_ACC_F06.ASSET_NO_2;
                entity.ASSET_NO_3 = pR_ACC_F06.ASSET_NO_3;
                entity.ASSET_NO_4 = pR_ACC_F06.ASSET_NO_4;
                entity.ASSET_NO_5 = pR_ACC_F06.ASSET_NO_5;
                entity.ASSET_NO_6 = pR_ACC_F06.ASSET_NO_6;
                entity.ASSET_NO_7 = pR_ACC_F06.ASSET_NO_7;
                entity.ASSET_NO_8 = pR_ACC_F06.ASSET_NO_8;
                entity.ASSET_NO_9 = pR_ACC_F06.ASSET_NO_9;
                entity.ASSET_NO_10 = pR_ACC_F06.ASSET_NO_10;
                entity.ID = Guid.NewGuid().ToString();
                entity.ORDER_HISTORY = last.ORDER_HISTORY + 1;
                entity.PROCEDURE_INDEX = entity.PROCEDURE_INDEX + 1;
                entity.CREATE_USER = sess.CODE;
                entity.UPD_DATE = DateTime.Now;
                var a = ModelState.Values.Where(r => r.Errors.Count > 0).ToList();
                var summary = FormSummaryRepository.GetSummary(pR_ACC_F06.TICKET);
                var flag = PurAccF06Repository.Update(entity);
                if (flag)
                {
                    var isFn = ProcessRepository.MaxIndex(summary.PROCESS_ID) == entity.PROCEDURE_INDEX;
                    #region Send Email
                    if (!isFn) //Nếu là người cuối cùng xác nhận thì k gửi mail
                    {
                        var process = ProcessRepository.GetProcess(summary.PROCESS_ID, entity.PROCEDURE_INDEX + 1);
                        var stations = StationRepository.GetStations(process.STATION_NO);
                        var userID = stations.Select(r => r.USER_ID).ToList();
                        var userMails = UserRepository.GetUsers(userID);
                        var dear = "Dear All !";
                        if (userMails.Count == 1)
                        {
                            var userApproval = UserRepository.GetUser(userID.FirstOrDefault());
                            dear = $"Dear {userApproval.SHORT_NAME} san !";
                        }
                        string body = $@"
                                                <h3>{dear}</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='http://172.28.10.17:90/PurAccF06/Details?ticket={entity.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                        await Business.MailHelper.SenMailOutlookAsync(userMails, body);
                    }
                    #endregion
                    return RedirectToAction("Index", "Home");
                }
            }



            return View(pR_ACC_F06);
        }
        public async Task<JsonResult> AssetEditAsync(PR_ACC_F06 pR_ACC_F06)
        {
            string message = "";
            var sess = Session["user"] as Form_User;
            var entities = PurAccF06Repository.GetForms(pR_ACC_F06.TICKET);
            var first = entities.OrderBy(o => o.ORDER_HISTORY).FirstOrDefault();
            var last = entities.OrderByDescending(o => o.ORDER_HISTORY).FirstOrDefault();
            //if (last.CREATE_USER != sess.CODE)
            //{
                var entity = last.CloneObject() as PR_ACC_F06;
                entity.COST_CENTER_1 = pR_ACC_F06.COST_CENTER_1;
                entity.COST_CENTER_2 = pR_ACC_F06.COST_CENTER_2;
                entity.COST_CENTER_3 = pR_ACC_F06.COST_CENTER_3;
                entity.COST_CENTER_4 = pR_ACC_F06.COST_CENTER_4;
                entity.COST_CENTER_5 = pR_ACC_F06.COST_CENTER_5;
                entity.COST_CENTER_6 = pR_ACC_F06.COST_CENTER_6;
                entity.COST_CENTER_7 = pR_ACC_F06.COST_CENTER_7;
                entity.COST_CENTER_8 = pR_ACC_F06.COST_CENTER_8;
                entity.COST_CENTER_9 = pR_ACC_F06.COST_CENTER_9;
                entity.COST_CENTER_10 = pR_ACC_F06.COST_CENTER_10;
                entity.AK_1 = pR_ACC_F06.AK_1;
                entity.AK_2 = pR_ACC_F06.AK_2;
                entity.AK_3 = pR_ACC_F06.AK_3;
                entity.AK_4 = pR_ACC_F06.AK_4;
                entity.AK_5 = pR_ACC_F06.AK_5;
                entity.AK_6 = pR_ACC_F06.AK_6;
                entity.AK_7 = pR_ACC_F06.AK_7;
                entity.AK_8 = pR_ACC_F06.AK_8;
                entity.AK_9 = pR_ACC_F06.AK_9;
                entity.AK_10 = pR_ACC_F06.AK_10;

                entity.ACOUNT_CODE_1 = pR_ACC_F06.ACOUNT_CODE_1;
                entity.ACOUNT_CODE_2 = pR_ACC_F06.ACOUNT_CODE_2;
                entity.ACOUNT_CODE_3 = pR_ACC_F06.ACOUNT_CODE_3;
                entity.ACOUNT_CODE_4 = pR_ACC_F06.ACOUNT_CODE_4;
                entity.ACOUNT_CODE_5 = pR_ACC_F06.ACOUNT_CODE_5;
                entity.ACOUNT_CODE_6 = pR_ACC_F06.ACOUNT_CODE_6;
                entity.ACOUNT_CODE_7 = pR_ACC_F06.ACOUNT_CODE_7;
                entity.ACOUNT_CODE_8 = pR_ACC_F06.ACOUNT_CODE_8;
                entity.ACOUNT_CODE_9 = pR_ACC_F06.ACOUNT_CODE_9;
                entity.ACOUNT_CODE_10 = pR_ACC_F06.ACOUNT_CODE_10;

                entity.ASSET_NO_1 = pR_ACC_F06.ASSET_NO_1;
                entity.ASSET_NO_2 = pR_ACC_F06.ASSET_NO_2;
                entity.ASSET_NO_3 = pR_ACC_F06.ASSET_NO_3;
                entity.ASSET_NO_4 = pR_ACC_F06.ASSET_NO_4;
                entity.ASSET_NO_5 = pR_ACC_F06.ASSET_NO_5;
                entity.ASSET_NO_6 = pR_ACC_F06.ASSET_NO_6;
                entity.ASSET_NO_7 = pR_ACC_F06.ASSET_NO_7;
                entity.ASSET_NO_8 = pR_ACC_F06.ASSET_NO_8;
                entity.ASSET_NO_9 = pR_ACC_F06.ASSET_NO_9;
                entity.ASSET_NO_10 = pR_ACC_F06.ASSET_NO_10;
                entity.ID = Guid.NewGuid().ToString();
                entity.ORDER_HISTORY = last.ORDER_HISTORY + 1;
                entity.PROCEDURE_INDEX = entity.PROCEDURE_INDEX + 1;
                entity.CREATE_USER = sess.CODE;
                entity.UPD_DATE = DateTime.Now;
                var a = ModelState.Values.Where(r => r.Errors.Count > 0).ToList();
                var summary = FormSummaryRepository.GetSummary(pR_ACC_F06.TICKET);
                var flag = PurAccF06Repository.Update(entity);
                if (flag)
                {
                    var isFn = ProcessRepository.MaxIndex(summary.PROCESS_ID) == entity.PROCEDURE_INDEX;
                    #region Send Email
                    if (!isFn) //Nếu là người cuối cùng xác nhận thì k gửi mail
                    {
                        var process = ProcessRepository.GetProcess(summary.PROCESS_ID, entity.PROCEDURE_INDEX + 1);
                        var stations = StationRepository.GetStations(process.STATION_NO);
                        var userID = stations.Select(r => r.USER_ID).ToList();
                        var userMails = UserRepository.GetUsers(userID);
                        var dear = "Dear All !";
                        if (userMails.Count == 1)
                        {
                            var userApproval = UserRepository.GetUser(userID.FirstOrDefault());
                            dear = $"Dear {userApproval.SHORT_NAME} san !";
                        }
                        string body = $@"
                                                <h3>{dear}</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='http://172.28.10.17:90/PurAccF06/Details?ticket={entity.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                        await Business.MailHelper.SenMailOutlookAsync(userMails, body);
                        message = "OK";
                    }
                    #endregion
                    //  return RedirectToAction("Index", "Home");
                //}
            }
            return Json(message);
        }
        // GET: PurAccF06/Edit/5
        public ActionResult Edit(string ticket)
        {
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var summary = FormSummaryRepository.GetSummary(ticket);
            if (summary == null)
            {
                return HttpNotFound();
            }
            if (!summary.IS_REJECT)
            {
                return HttpNotFound();
            }
            var entity = PurAccF06Repository.GetForm(ticket);
            return View(entity);
        }

        // POST: PurAccF06/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PR_ACC_F06 pR_ACC_F06)
        {

            var entities = PurAccF06Repository.GetForms(pR_ACC_F06.TICKET);
            var first = entities.OrderBy(o => o.ORDER_HISTORY).FirstOrDefault();
            var last = entities.OrderByDescending(o => o.ORDER_HISTORY).FirstOrDefault();
            var entity = last.CloneObject() as PR_ACC_F06;
            entity.PROCEDURE_INDEX = 0;
            entity.UNIT_PRICE_1 = pR_ACC_F06.UNIT_PRICE_1;
            entity.UNIT_PRICE_2 = pR_ACC_F06.UNIT_PRICE_2;
            entity.UNIT_PRICE_3 = pR_ACC_F06.UNIT_PRICE_3;
            entity.UNIT_PRICE_4 = pR_ACC_F06.UNIT_PRICE_4;
            entity.UNIT_PRICE_5 = pR_ACC_F06.UNIT_PRICE_5;
            entity.UNIT_PRICE_6 = pR_ACC_F06.UNIT_PRICE_6;
            entity.UNIT_PRICE_7 = pR_ACC_F06.UNIT_PRICE_7;
            entity.UNIT_PRICE_8 = pR_ACC_F06.UNIT_PRICE_8;
            entity.UNIT_PRICE_9 = pR_ACC_F06.UNIT_PRICE_9;
            entity.UNIT_PRICE_10 = pR_ACC_F06.UNIT_PRICE_10;
            entity.ID = Guid.NewGuid().ToString();
            entity.ORDER_HISTORY = last.ORDER_HISTORY + 1;
            if (entity.QTY_1 != null)
            {
                entity.AMOUNT_1 = entity.UNIT_PRICE_1 * entity.QTY_1;
            }
            if (entity.QTY_2 != null)
            {
                entity.AMOUNT_2 = entity.UNIT_PRICE_2 * entity.QTY_2;
            }
            if (entity.QTY_3 != null)
            {
                entity.AMOUNT_3 = entity.UNIT_PRICE_3 * entity.QTY_3;
            }
            if (entity.QTY_4 != null)
            {
                entity.AMOUNT_4 = entity.UNIT_PRICE_4 * entity.QTY_4;
            }
            if (entity.QTY_5 != null)
            {
                entity.AMOUNT_5 = entity.UNIT_PRICE_5 * entity.QTY_5;
            }
            if (entity.QTY_6 != null)
            {
                entity.AMOUNT_6 = entity.UNIT_PRICE_6 * entity.QTY_6;
            }
            if (entity.QTY_7 != null)
            {
                entity.AMOUNT_7 = entity.UNIT_PRICE_7 * entity.QTY_7;
            }
            if (entity.QTY_8 != null)
            {
                entity.AMOUNT_8 = entity.UNIT_PRICE_8 * entity.QTY_8;
            }
            if (entity.QTY_9 != null)
            {
                entity.AMOUNT_9 = entity.UNIT_PRICE_9 * entity.QTY_9;
            }
            if (entity.QTY_10 != null)
            {
                entity.AMOUNT_10 = entity.UNIT_PRICE_10 * entity.QTY_10;
            }
            PurAccF06Repository.SetAmounts(entity);
            var sess = Session["user"] as Form_User;
            entity.CREATE_USER = sess.CODE;
            entity.UPD_DATE = DateTime.Now;
            var a = ModelState.Values.Where(r => r.Errors.Count > 0).ToList();
            string virtualPath = "";
            string[] virtualPaths = new string[5];
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            for (int file = 0; file < files.Count; file++)
            {
                HttpPostedFile filedata = files[file];
                string fileName = filedata.FileName;
                virtualPath = string.Format("/UploadedFiles/{0}", fileName);
                virtualPaths[file] = virtualPath;
                string fullPath = Server.MapPath(virtualPath);
                if (fileName != "")
                {
                    filedata.SaveAs(fullPath);
                }

            }
            int f = 0;
            if (entity.FILE_NAME_1 != null)
            {
                f = 1;
            }
            if (entity.FILE_NAME_2 != null)
            {
                f = 2;
            }
            if (entity.FILE_NAME_3 != null)
            {
                f = 3;
            }
            if (entity.FILE_NAME_4 != null)
            {
                f = 4;
            }
            for (int file = 0; file < files.Count; file++)
            {
                HttpPostedFile filedata = files[file];
                string fileName = filedata.FileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_1) == $"FILE_PATH_{file + f + 1}")
                    {
                        entity.FILE_PATH_1 = WebHelper.BaseUrl + virtualPaths[file];
                        entity.FILE_NAME_1 = fileName;
                    }
                    if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_2) == $"FILE_PATH_{file + f + 1}")
                    {
                        entity.FILE_PATH_2 = WebHelper.BaseUrl + virtualPaths[file];
                        entity.FILE_NAME_2 = fileName;
                    }
                    if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_3) == $"FILE_PATH_{file + f + 1}")
                    {
                        entity.FILE_PATH_3 = WebHelper.BaseUrl + virtualPaths[file];
                        entity.FILE_NAME_3 = fileName;
                    }
                    if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_4) == $"FILE_PATH_{file + f + 1}")
                    {
                        entity.FILE_PATH_4 = WebHelper.BaseUrl + virtualPaths[file];
                        entity.FILE_NAME_4 = fileName;
                    }
                    if (RemoteMgr.GetPropertyName(() => entity.FILE_PATH_5) == $"FILE_PATH_{file + f + 1}")
                    {
                        entity.FILE_PATH_5 = WebHelper.BaseUrl + virtualPaths[file];
                        entity.FILE_NAME_5 = fileName;
                    }
                }
            }
            var summary = FormSummaryRepository.GetSummary(entity.TICKET);
            var flag = PurAccF06Repository.Update(entity, entity.PROCEDURE_INDEX, false, false);
            if (flag)
            {
                //var process = ProcessRepository.GetProcess(summary.RETURN_TO);
                var process = ProcessRepository.GetProcess(summary.PROCESS_ID, entity.PROCEDURE_INDEX + 1);
                var station = StationRepository.GetStations(process.STATION_NO);
                var userApproval = UserRepository.GetUser(station.FirstOrDefault().USER_ID);
                string body = $@"
                                                <h3>Dear {userApproval.SHORT_NAME} san!</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='http://172.28.10.17:90/PurAccF06/Details?ticket={entity.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                Task t = Business.MailHelper.SenMailOutlookAsync(userApproval.EMAIL, body);
                await t;
                return RedirectToAction("Index", "Home");
            }
            return View(entity);
        }

        public void SaveFiles(HttpFileCollection files)
        {
            string virtualPath = "";
            string[] virtualPaths = new string[5];
            for (int file = 0; file < files.Count; file++)
            {
                HttpPostedFile filedata = files[file];
                string fileName = filedata.FileName;
                virtualPath = string.Format("/UploadedFiles/{0}", fileName);
                virtualPaths[file] = virtualPath;
                string fullPath = Server.MapPath(virtualPath);
                if (fileName != "")
                {
                    filedata.SaveAs(fullPath);
                }

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
    }
}
