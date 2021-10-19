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
        public ActionResult Create()
        {
            // var users = UserRepository.GetUsers();
            return View();
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
        public JsonResult GetProcess(string process, string selectedForm, string user)
        {
            var session = Session["user"] as Form_User;
            List<Form_Process> formprocess = new List<Form_Process>();
            List<Form_Stations> stations = new List<Form_Stations>();
            List<ApprovalEntity> members = (List<ApprovalEntity>)Newtonsoft.Json.JsonConvert.DeserializeObject(user, typeof(List<ApprovalEntity>));
            List<FormProcessJsonEntity> myDeserializedObjList = (List<FormProcessJsonEntity>)Newtonsoft.Json.JsonConvert.DeserializeObject(process, typeof(List<FormProcessJsonEntity>));
            var dept = DeptRepository.GetDept(session.DEPT);
            var mng = UserRepository.GetUser(dept.CODE_MNG);
            if (!members.Exists(r => r.index == 2)) members.Add(new ApprovalEntity() { index = 2, member = new List<MemberEntity>() { new MemberEntity() { code = mng.CODE, name = mng.NAME } } });
            if (!members.Exists(r => r.index == 4)) members.Add(new ApprovalEntity() { index = 4, member = new List<MemberEntity>() { new MemberEntity() { code = "iwasaki", name = "Iwasaki" } } });
            if (!members.Exists(r => r.index == 5)) members.Add(new ApprovalEntity() { index = 5, member = new List<MemberEntity>() { new MemberEntity() { code = "yokouchi", name = "Yokouchi" } } });
            foreach (var item in myDeserializedObjList)
            {
                var stationNo = $"{selectedForm}_{item.index}";
                formprocess.Add(new Form_Process()
                {
                    FORM_NAME = selectedForm,
                    ID = Guid.NewGuid().ToString(),
                    FORM_INDEX = item.index,
                    CREATE_DATE = DateTime.Now,
                    STATION_NO = stationNo,
                    STATION_NAME = item.name,
                    RETURN_STATION_NO = item.returnTo is null ? "" : myDeserializedObjList.FirstOrDefault(r => r.index == item.returnTo).name,
                    RETURN_INDEX = item.returnTo,
                    UPDATE_DATE = DateTime.Now,
                    CREATER_NAME = session.NAME,
                    UPDATER_NAME = session.NAME
                });

                var stationSlect = members.FirstOrDefault(r => r.index == item.index + 1);

                if (stationSlect != null)
                {
                    foreach (var u in stationSlect.member)
                    {
                        stations.Add(new Form_Stations()
                        {
                            ID = Guid.NewGuid().ToString(),
                            STATION_NO = stationNo,
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
                        STATION_NO = stationNo,
                        STATION_NAME = item.name,
                        USER_ID = "",
                        USER_NAME = "",
                        FORM_INDEX = item.index,
                        PROCESS = selectedForm

                    });
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
            foreach (var station in stations)
            {
                var stationNoOld = db.Form_Stations.Where(r => r.STATION_NO == station.STATION_NO);
                db.Form_Stations.RemoveRange(stationNoOld);
                db.SaveChanges();
            }

            db.Form_Stations.AddRange(stations);
            db.SaveChanges();
            #endregion
            return Json(new { body = "OK" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult LoadProcess(string processId)
        {
            var process = ProcessRepository.GetProcessName(processId);
            var sort = process.OrderBy(r => r.FORM_INDEX);
            var list = StationRepository.GetStationsWithProcessName(processId);
            var listStation = new List<StationMemberModel>();
            foreach (var item in sort)
            {
                var listtemp = new List<Member>();
                foreach (var temp in list.Where(r => r.STATION_NO == item.STATION_NO))
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
