using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [CustomAuthorize("Admin")]
    public class ProcessController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Process
        public ActionResult Index()
        {
            var process = ProcessRepository.GetProcessName();
            return View(process);
        }

        // GET: Process/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var process = ProcessRepository.GetProcessName(id);
            if (process == null || process.Count == 0)
            {
                return HttpNotFound();
            }
            var users = UserRepository.GetUsers();
            ViewBag.process = id;
            return View(users);
        }

        // GET: Process/Create
        public ActionResult Create(string id = null)
        {
            if (id != null)
            {
                var process = ProcessRepository.GetProcessName(id);
                if (process == null || process.Count == 0)
                {
                    return HttpNotFound();
                }
                ViewBag.process = id;
            }
            var users = UserRepository.GetUsers();
            ViewBag.listStations = ProcessRepository.GetAllStation();
            ViewBag.listPermission = ProcessRepository.GetAllPermission();
            ViewBag.Depts = DeptRepository.GetAll();
            return View(users);

        }


        [HttpPost]
        public ActionResult Create(Form_ProcessName entity)
        {
            var sess = Session["user"] as Form_User;
            entity.UPD_USER = sess.NAME;
            entity.UPD_TIME = DateTime.Now;
            if (ProcessRepository.SaveProcessName(entity))
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        // GET: Process/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form_Process form_Process = db.Form_Process.Find(id);
            if (form_Process == null)
            {
                return HttpNotFound();
            }
            return View(form_Process);
        }

        // POST: Process/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FORM_NAME,STATION_NO,FORM_INDEX,RETURN_STATION_NO,CREATER_NAME,UPDATER_NAME,CREATE_DATE,UPDATE_DATE,DES")] Form_Process form_Process)
        {
            if (ModelState.IsValid)
            {
                db.Entry(form_Process).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(form_Process);
        }

        // GET: Process/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Form_Process form_Process = db.Form_Process.Find(id);
            if (form_Process == null)
            {
                return HttpNotFound();
            }
            return View(form_Process);
        }

        // POST: Process/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Form_Process form_Process = db.Form_Process.Find(id);
            db.Form_Process.Remove(form_Process);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetProcess(string process)
        {
            return Json(new { body = "OK" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetProcess(string process, string selectedForm, string user, string state)
        {
            try
            {
                using (var db = new DataContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (state == "new")
                            {
                                var exist = db.Form_Process.Where(m => m.FORM_NAME == selectedForm).FirstOrDefault();
                                if (exist != null)
                                    return Json(new { body = STATUS.ERROR, message = "Đã tồn tại " + selectedForm + " này rồi! Hãy điền tên khác!" }, JsonRequestBehavior.AllowGet);

                            }
                            var session = Session["user"] as Form_User;
                            List<Form_Process> formprocess = new List<Form_Process>();
                            List<Form_Stations> stations = new List<Form_Stations>();
                            List<ApprovalEntity> members = (List<ApprovalEntity>)Newtonsoft.Json.JsonConvert.DeserializeObject(user, typeof(List<ApprovalEntity>));
                            List<FormProcessJsonEntity> myDeserializedObjList = (List<FormProcessJsonEntity>)Newtonsoft.Json.JsonConvert.DeserializeObject(process, typeof(List<FormProcessJsonEntity>));
                            var dept = DeptRepository.GetDept(session.DEPT);
                            var mng = UserRepository.GetUser(dept.CODE_MNG);

                            foreach (var item in myDeserializedObjList)
                            {
                                formprocess.Add(new Form_Process()
                                {
                                    FORM_NAME = selectedForm,
                                    ID = Guid.NewGuid().ToString(),
                                    FORM_INDEX = item.index,
                                    CREATE_DATE = DateTime.Now,
                                    STATION_NO = item.no,
                                    STATION_NAME = item.name,
                                    RETURN_STATION_NO = item.returnTo is null ? "" : myDeserializedObjList.FirstOrDefault(r => r.index == item.returnTo).name,
                                    RETURN_INDEX = item.returnTo,
                                    UPDATE_DATE = DateTime.Now,
                                    CREATER_NAME = session.NAME,
                                    UPDATER_NAME = session.NAME
                                });

                                var stationSlect = members.FirstOrDefault(r => r.index == item.index + 1);

                                if (stationSlect != null && stationSlect.member.Count > 0)
                                {
                                    foreach (var u in stationSlect.member)
                                    {
                                       
                                        stations.Add(new Form_Stations()
                                        {
                                            ID = Guid.NewGuid().ToString(),
                                            STATION_NO = item.no,
                                            STATION_NAME = item.name,
                                            USER_ID = u.code,
                                            USER_NAME = u.name,
                                            FORM_INDEX = item.index,
                                            PROCESS = selectedForm

                                        });
                                    }

                                }
                                else
                                {
                                    stations.Add(new Form_Stations()
                                    {
                                        ID = Guid.NewGuid().ToString(),
                                        STATION_NO = item.no,
                                        STATION_NAME = item.name,
                                        USER_ID = "",
                                        USER_NAME = "",
                                        FORM_INDEX = item.index,
                                        PROCESS = selectedForm

                                    });
                                }

                                //REJECT LIST

                                var listReject = db.Form_Reject.Where(m => m.PROCESS_NAME == selectedForm && m.START_INDEX == (item.index - 1)).ToList();
                                if (listReject != null)
                                {
                                    db.Form_Reject.RemoveRange(listReject);
                                    db.SaveChanges();
                                }

                                if (item.rejectList != null)
                                {
                                    int i = 0;
                                    foreach (var reject in item.rejectList)
                                    {
                                        var formReject = new Form_Reject()
                                        {
                                            PROCESS_NAME = selectedForm,
                                            FORM_INDEX = reject.FORM_INDEX - 1,
                                            START_INDEX = reject.START_INDEX - 1,
                                            TOTAL_STEP = item.rejectList.Count(),
                                            STEP_ORDER = i
                                        };
                                        db.Form_Reject.Add(formReject);
                                        i++;
                                    }
                                    db.SaveChanges();
                                }

                                //PERMISSION
                                var listPermission = db.LCA_PERMISSION.Where(m => m.PROCESS == selectedForm && m.ITEM_COLUMN_PERMISSION == item.index.ToString()).ToList();
                                if (listPermission != null)
                                {
                                    db.LCA_PERMISSION.RemoveRange(listPermission);
                                    db.SaveChanges();
                                }
                                if (item.permission != null)
                                {
                                    foreach(var permission in item.permission)
                                    {
                                        var per = new LCA_PERMISSION()
                                        {
                                            ITEM_COLUMN = permission.ITEM_COLUMN,
                                            ITEM_COLUMN_PERMISSION = permission.ITEM_COLUMN_PERMISSION,
                                            DEPT = permission.DEPT,
                                            PROCESS = selectedForm
                                        };
                                        db.LCA_PERMISSION.Add(per);
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {

                                }
                              

                            }
                            #region Save Process
                            var formProcessOld = db.Form_Process.Where(r => r.FORM_NAME == selectedForm);
                            db.Form_Process.RemoveRange(formProcessOld);
                            db.SaveChanges();
                            db.Form_Process.AddRange(formprocess);
                            db.SaveChanges();
                            #endregion

                            #region Save Station
                            var stationNoOld = db.Form_Stations.Where(r => r.PROCESS == selectedForm);
                            db.Form_Stations.RemoveRange(stationNoOld);
                            db.SaveChanges();
                            db.Form_Stations.AddRange(stations);
                            db.SaveChanges();

                            var formProcessName = db.Form_ProcessNames.Where(m => m.PROCESS_ID == selectedForm).FirstOrDefault();
                            if (formProcessName != null)
                            {
                                formProcessName.PROCESS_NAME = selectedForm;
                                formProcessName.UPD_TIME = DateTime.Now;
                                formProcessName.UPD_USER = session.NAME;

                            }
                            else
                            {
                                formProcessName = new Form_ProcessName()
                                {
                                    UPD_USER = session.NAME,
                                    UPD_TIME = DateTime.Now,
                                    PROCESS_NAME = selectedForm,
                                    PROCESS_ID = selectedForm
                                };
                                db.Form_ProcessNames.Add(formProcessName);
                            }
                            db.SaveChanges();
                            transaction.Commit();
                            #endregion

                          
                            return Json(new { body = "OK" }, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            return Json(new { body = "NG" }, JsonRequestBehavior.AllowGet);

                        }

                    }
                }

            }
            catch (Exception)
            {

                return Json(new { body = "NG" }, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult CheckExist(string processId)
        {
            using (var db = new DataContext())
            {
                try
                {
                    var process = db.Form_Process.Where(m => m.FORM_NAME == processId.Trim()).FirstOrDefault();

                    if (process != null)
                    {
                        return Json(new
                        {
                            result = STATUS.ERROR,
                            message = "Đã tồn tại " + processId + " này rồi! Hãy điền tên khác!"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            result = STATUS.SUCCESS
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = STATUS.ERROR,
                        message = "Có lỗi xảy ra"
                    }, JsonRequestBehavior.AllowGet);
                }


            }


        }

        public ActionResult DeleteProcess(string id)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var processName = db.Form_ProcessNames.Where(m => m.PROCESS_NAME == id.Trim()).ToList();
                        if (processName != null)
                            db.Form_ProcessNames.RemoveRange(processName);
                        var process = db.Form_Process.Where(m => m.FORM_NAME == id.Trim()).ToList();
                        if (process != null)
                            db.Form_Process.RemoveRange(process);
                        var station = db.Form_Stations.Where(m => m.PROCESS == id.Trim()).ToList();
                        if (station != null)
                            db.Form_Stations.RemoveRange(station);
                        db.SaveChanges();
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {

                        transaction.Rollback();
                        ViewBag.Error = e.Message.ToString();
                        var process = ProcessRepository.GetProcessName();
                        return View("Index", process);
                    }

                }
            }

        }
        public JsonResult LoadProcess(string processId)
        {
            var process = ProcessRepository.GetProcessName(processId);
            var sort = process.OrderBy(r => r.FORM_INDEX);
            var list = StationRepository.GetStationsWithProcessName(processId);
            var listStation = new List<StationMemberModel>();
            var rejectList = ProcessRepository.GetRejectList(processId);
            var permission = ProcessRepository.GetAllPermission(processId);
            foreach (var item in sort)
            {
                var listtemp = new List<Member>();
                var stations = list.Where(r => r.STATION_NO == item.STATION_NO).ToList();
                foreach (var temp in stations)
                {
                    listtemp.Add(new Member()
                    {
                        code = temp.USER_ID,
                        name = temp.USER_NAME
                    });
                }
                listStation.Add(new StationMemberModel()
                {

                    index = item.FORM_INDEX + 1,
                    member = listtemp
                });
            }

            return Json(new
            {
                data = sort,
                reject = rejectList,
                permission = permission,
                stations = listStation
            }, JsonRequestBehavior.AllowGet);
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
