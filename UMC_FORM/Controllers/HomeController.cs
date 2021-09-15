using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    public class HomeController : Controller
    {
        private DataContext db = new DataContext();

        public ActionResult Index(SendType? type)
        {
            if (type is null)
            {
                type = SendType.SENDTOME;
            }
            var session = Session["user"] as Form_User;
            List<Form_Summary> formSummaries = new List<Form_Summary>();
            switch (type)
            {
                case SendType.SENDTOME:
                    foreach (var item in db.Form_Summary.Where(r => r.IS_FINISH == false).Where(t => t.IS_REJECT == false))
                    {
                        var form = db.PR_ACC_F06.FirstOrDefault(r => r.TICKET == item.TICKET && r.PROCEDURE_INDEX == item.PROCEDURE_INDEX);
                        if (form != null)
                        {
                            if (form.PROCEDURE_INDEX == 0)
                            {
                                // var mngUser = db.Form_MNG.FirstOrDefault(r => r.Code_User == form.CREATE_USER);
                                var userCreate = UserRepository.GetUser(item.CREATE_USER);
                                var dept = DeptRepository.GetDept(userCreate.DEPT);
                                if (dept.CODE_MNG == session.CODE)
                                {
                                    formSummaries.Add(item);
                                }
                            }
                            else if (form.PROCEDURE_INDEX == 2) // Gửi giám đốc nhà máy
                            {
                                if (session.CODE.ToUpper().Equals(Constant.FM))
                                {
                                    formSummaries.Add(item);
                                }
                            }
                            else if (form.PROCEDURE_INDEX == 3) // Gửi tổng giám đốc
                            {
                                if (session.CODE.ToUpper().Equals(Constant.GD))
                                {
                                    formSummaries.Add(item);
                                }
                            }
                            else // Gửi đến user được chỉ định
                            {

                                var index = form.PROCEDURE_INDEX + 1;// Tìm index của trạm tiếp theo
                                var processNext = db.Form_Process.FirstOrDefault(r => r.FORM_INDEX == index);
                                if (processNext != null)
                                {
                                    var stationNoNext = processNext.STATION_NO;
                                    var users = db.Form_Stations.Where(r => r.STATION_NO == stationNoNext);// Tim users approval
                                    foreach (var user in users)
                                    {
                                        if (user.USER_ID == session.CODE)
                                        {
                                            formSummaries.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ViewBag.type = 1;
                    break;
                case SendType.MYREQUEST:
                    foreach (var item in db.Form_Summary.Where(r => r.IS_FINISH == false).Where(t => t.IS_REJECT == false))
                    {
                        var form = db.PR_ACC_F06.FirstOrDefault(r => r.TICKET == item.TICKET && r.CREATE_USER.Contains(session.CODE));
                        if (form != null)
                        {
                            formSummaries.Add(item);
                        }
                    }
                    ViewBag.type = 2;
                    break;
                case SendType.CANCEL:
                    ViewBag.type = 3;
                    formSummaries = db.Form_Summary.Where(r => r.IS_REJECT == true).ToList();
                    break;
                case SendType.FINISH:
                    ViewBag.type = 4;
                    formSummaries = db.Form_Summary.Where(r => r.IS_FINISH == true).ToList();
                    break;

                default:
                    break;
            }
            return View(formSummaries.OrderByDescending(r => r.UPD_DATE));
        }
        public ActionResult Search(string search)
        {

            var session = Session["user"] as Form_User;
            List<Form_Summary> formSummaries = new List<Form_Summary>();
            var summary = FormSummaryRepository.GetSummaryLike(search);
            return View(summary);
        }

        public ActionResult About()
        {
            ViewBag.Message = "IT Design.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Tel: 3143";
            return View();
        }

    }
}
