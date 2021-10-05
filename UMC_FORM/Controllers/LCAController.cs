﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;
using UMC_FORM.Models.LCA;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    [CustomAuthorize("Normal")]
    public class LCAController : Controller
    {
        private Form_User _sess = new Form_User();
        // GET: LCA
        public ActionResult Create()
        {
            using (var db = new DataContext())
            {
                var list = db.LCA_FORM_01.ToList();
                return View();
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LCA_FORM_01 ticket)
        {
            if (ticket == null)
            {
                return HttpNotFound();
            }
            try
            {

                using (var db = new DataContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            _sess = Session["user"] as Form_User;
                            ticket.DEPT = _sess.DEPT;
                            ticket.SUBMIT_USER = _sess.CODE;
                            ticket.ORDER_HISTORY = 1;
                            ticket.IS_SIGNATURE = 1;
                            ticket.PROCEDURE_INDEX = 0;
                            ticket.ID = Guid.NewGuid().ToString();
                            ticket.TICKET = DateTime.Now.ToString("yyyyMMddHHmmss");
                            ticket.TITLE = Constant.LCA_FORM_01_TITLE;
                            ticket.UPD_DATE = DateTime.Now;

                            db.LCA_FORM_01.Add(ticket);
                            #region FILES
                            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                            for (int file = 0; file < files.Count; file++)
                            {
                                HttpPostedFile filedata = files[file];
                                string fileName = filedata.FileName;
                                var lcaFile = new LCA_FILE();
                                lcaFile.TICKET = ticket.TICKET;
                                lcaFile.ID_TICKET = ticket.ID;
                                lcaFile.FILE_URL = string.Format("/UploadedFiles/{0}-{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
                                lcaFile.FILE_NAME = fileName;
                                string fullPath = Server.MapPath(lcaFile.FILE_URL);
                                if (fileName != "")
                                {
                                    filedata.SaveAs(fullPath);
                                }
                                db.LCA_FILE.Add(lcaFile);
                            }

                            #endregion
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
                                RETURN_TO = 1,
                                PROCESS_ID = Constant.LCA_FORM_01_NAME,
                                LAST_INDEX = db.Form_Process.Where(m => m.FORM_NAME == Constant.LCA_FORM_01_NAME).Count() - 1
                            };
                            db.Form_Summary.Add(summary);
                            #endregion

                            db.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            ModelState.AddModelError("Error", e.Message.ToString());
                            return View();
                        }

                    }
                }

            }
            catch (Exception e)
            {

                ModelState.AddModelError("Error", e.Message.ToString());
                return View();
            }
            return RedirectToAction("Index", "Home", new { type = SendType.MYREQUEST });
        }

        public ActionResult Details(string ticket)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var modelDetail = new LCADetailModel();
                    var list = db.LCA_FORM_01.Where(m => m.TICKET == ticket).OrderByDescending(m => m.ORDER_HISTORY).ToList();
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
                    var userApprover = db.Form_Stations.Where(m => m.FORM_INDEX == modelDetail.SUMARY.RETURN_TO).ToList();
                    if (userApprover.Where(m => m.USER_ID == _sess.CODE).FirstOrDefault() != null)
                    {
                        modelDetail.IS_APPROVER = true;
                    }
                    modelDetail.PERMISSION = new List<string>();
                    var listPermission = db.LCA_PERMISSION.Where(m => m.ITEM_COLUMN_PERMISSION == modelDetail.SUMARY.RETURN_TO.ToString()).ToList();
                    foreach (var permission in listPermission)
                    {
                        modelDetail.PERMISSION.Add(permission.ITEM_COLUMN);
                    }
                    var process = db.Form_Process.Where(m => m.FORM_NAME == Constant.LCA_FORM_01_NAME).OrderBy(m => m.FORM_INDEX).ToList();
                    modelDetail.STATION_APPROVE = new List<StationApproveModel>();
                    foreach (var pro in process)
                    {
                        var station = new StationApproveModel()
                        {
                            STATION_NAME = pro.STATION_NAME,
                            IS_APPROVED = false

                        };
                        var lca = list.Where(m => m.PROCEDURE_INDEX == pro.FORM_INDEX).FirstOrDefault();
                        if (lca != null && lca.IS_SIGNATURE == 1)
                        {
                            station.IS_APPROVED = true;
                            station.APPROVE_DATE = lca.UPD_DATE;
                            station.APPROVER = lca.SUBMIT_USER;
                            station.COMPANY = "UMCVN";
                        }
                        modelDetail.STATION_APPROVE.Add(station);
                    }

                    return View(modelDetail);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message.ToString());
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string ticket, string ID, string status, string quotes, LCA_FORM_01 infoTicket)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var formDb = db.LCA_FORM_01.Where(m => m.TICKET == ticket).OrderByDescending(m => m.ORDER_HISTORY).FirstOrDefault();
                    if (formDb == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        using (DbContextTransaction transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                var form = formDb.CloneObject() as LCA_FORM_01;
                                _sess = Session["user"] as Form_User;
                                #region FORM
                                form.ORDER_HISTORY += 1;
                                form.IS_SIGNATURE = 1;
                                form.PROCEDURE_INDEX += 1;

                                form.UPD_DATE = DateTime.Now;
                                form.SUBMIT_USER = _sess.CODE;
                                form.ID = Guid.NewGuid().ToString();
                                var listPermission = db.LCA_PERMISSION.Where(m => m.ITEM_COLUMN_PERMISSION == form.PROCEDURE_INDEX.ToString()).ToList();
                                if (listPermission.Where(m => m.ITEM_COLUMN == PERMISSION.QUOTE).FirstOrDefault() != null)
                                {
                                    form.RECEIVE_DATE = infoTicket.RECEIVE_DATE;
                                    form.LEAD_TIME = infoTicket.LEAD_TIME;
                                    form.COMMENT = infoTicket.COMMENT;
                                }

                                #region Quote
                                List<LCA_QUOTE> lcaQuotes = new List<LCA_QUOTE>();


                                // không sửa giá
                                if (string.IsNullOrEmpty(quotes))
                                {

                                    lcaQuotes = db.LCA_QUOTE.Where(m => m.ID_TICKET == ID).ToList();
                                }

                                // Khi sửa đổi quotes
                                else
                                {
                                    lcaQuotes = JsonConvert.DeserializeObject<List<LCA_QUOTE>>(quotes);
                                }
                                foreach (var quote in lcaQuotes)
                                {
                                    var quoteDb = new LCA_QUOTE
                                    {
                                        ID_TICKET = form.ID,
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

                                #endregion
                                #region Files
                                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                                for (int file = 0; file < files.Count; file++)
                                {
                                    HttpPostedFile filedata = files[file];
                                    string fileName = filedata.FileName;
                                    var lcaFile = new LCA_FILE();
                                    lcaFile.TICKET = form.TICKET;
                                    lcaFile.ID_TICKET = form.ID;
                                    lcaFile.FILE_URL = string.Format("/UploadedFiles/{0}-{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
                                    lcaFile.FILE_NAME = fileName;
                                    string fullPath = Server.MapPath(lcaFile.FILE_URL);
                                    if (fileName != "")
                                    {
                                        filedata.SaveAs(fullPath);
                                    }
                                    db.LCA_FILE.Add(lcaFile);
                                }

                                #endregion
                                db.LCA_FORM_01.Add(form);
                                #endregion

                                #region SUMARY
                                var summary = db.Form_Summary.Where(m => m.TICKET == ticket).FirstOrDefault();
                                summary.PROCEDURE_INDEX = form.PROCEDURE_INDEX;
                                summary.CREATE_USER = _sess.CODE;
                                summary.UPD_DATE = DateTime.Now;
                                summary.RETURN_TO = form.PROCEDURE_INDEX + 1;
                                if (form.PROCEDURE_INDEX == summary.LAST_INDEX)
                                {
                                    summary.IS_FINISH = true;
                                }
                                #endregion

                                db.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                ModelState.AddModelError("Error", e.Message.ToString());
                                return View();
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {

                ModelState.AddModelError("Error", e.Message.ToString());
                return View();
            }
            return RedirectToAction("Index", "Home", new { type = SendType.SENDTOME });
        }
        public ActionResult PrintView()
        {
            return View();
        }
    }
}