using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public abstract class TicketHelper
    {
        public List<StationApproveModel> GetListApproved(Form_Summary summary)
        {
            using (var db = new DataContext())
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
                    var listForm = CheckIsSignature(pro.STATION_NAME.Trim(), summary);
                    if (listForm != null)
                    {
                        station.IS_APPROVED = true;
                        station.APPROVE_DATE = listForm.UPDATE_DATE is DateTime date ? date : DateTime.Now;
                        station.APPROVER = listForm.APPROVAL_NAME;
                        station.COMPANY = "UMCVN";
                        var user = db.Form_User.Where(m => m.CODE == station.APPROVER).FirstOrDefault();
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

        }
        public abstract Form_Procedures CheckIsSignature(string stationName, Form_Summary summary);

    }
}
