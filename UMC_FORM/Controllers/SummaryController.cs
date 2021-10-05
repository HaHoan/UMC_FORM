using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Authentication;
using UMC_FORM.Business;
using UMC_FORM.Models;
using UMC_FORM.Models.Summary;

namespace UMC_FORM.Controllers
{
    [CustomAuthFilter]
    public class SummaryController : Controller
    {
        // GET: Summary
        public ActionResult Index()
        {
            var session = Session["user"] as Form_User;
            if (session.CODE != Constant.GD.ToLower()) return RedirectToAction("Index", "Home");
            return View();
        }
        public decimal? GetMoney(List<PR_ACC_F06> lst)
        {
            var total = lst.Sum(r => r.AMOUNT_1) + lst.Sum(r => r.AMOUNT_2) + lst.Sum(r => r.AMOUNT_3) + lst.Sum(r => r.AMOUNT_4) + lst.Sum(r => r.AMOUNT_5)
                + lst.Sum(r => r.AMOUNT_6) + lst.Sum(r => r.AMOUNT_7) + +lst.Sum(r => r.AMOUNT_8) + +lst.Sum(r => r.AMOUNT_9) + lst.Sum(r => r.AMOUNT_10);
            return total;
        }

        public JsonResult LoadData(string startDate, string endDate, string filter)
        {
            using (var db = new DataContext())
            {
                List<Form_Summary> list = new List<Form_Summary>();
                var start = DateTime.Parse(startDate);
                var end = DateTime.Parse(endDate);
                list = db.Form_Summary.Where(m => m.UPD_DATE >= start && m.UPD_DATE < end && m.IS_FINISH == true).ToList();
                var oneDay = TimeSpan.FromDays(1);
                var chartListDaily = new List<QuoteChart>();
               
                for (DateTime currentDay = start; currentDay < end; currentDay += oneDay)
                {
                    var totalMoneyInDay = list.Where(m => m.UPD_DATE == currentDay).Sum(m => m.TOTAL_MONEY);
                    if(totalMoneyInDay is int total && total > 0)
                    {
                        var obj = new QuoteChart()
                        {
                            label = currentDay.ToString("yyyy/MM/dd"),
                            y = total
                        };
                        chartListDaily.Add(obj);
                    }
                    
                }
                var chartListMonthly = new List<QuoteChart>();
                for (DateTime currentMonth = start; currentMonth < end; currentMonth=currentMonth.AddMonths(1))
                {
                    
                    var totalMoneyInMonth = list.Where(m => m.UPD_DATE.Month == currentMonth.Month && m.UPD_DATE.Year == currentMonth.Year).Sum(m => m.TOTAL_MONEY);
                    if (totalMoneyInMonth is int total && total > 0)
                    {
                        var obj = new QuoteChart()
                        {
                            label = currentMonth.ToString("MMM"),
                            y = total
                        };
                        chartListMonthly.Add(obj);
                    }

                }

                var  totalMoney = list.Sum(m => m.TOTAL_MONEY);
                int totalAllPeriod = totalMoney is int total1 ? total1 : 0;
               
                return Json(new
                {
                   daily = chartListDaily,
                   monthly = chartListMonthly,
                   totalMoney = totalAllPeriod
                }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
