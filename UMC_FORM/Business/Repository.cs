using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class Repository
    {
        //public List<PR_ACC_F06> GetAccF06()
        //{
        //    using (DataContext db = new DataContext())
        //    {
        //        var list = db.PR_ACC_F06.ToList().GroupBy(r => new { r.TICKET }).Select(h => new PR_ACC_F06()
        //        {
        //            TICKET = h.Key.TICKET,
        //            PROCEDURE_INDEX = h.Max(d => d.PROCEDURE_INDEX),
        //            UPD_DATE = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.UPD_DATE).FirstOrDefault(),
        //            CREATE_USER = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.CREATE_USER).FirstOrDefault(),
        //            TITLE = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.TITLE).FirstOrDefault(),
        //            ID = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.ID).FirstOrDefault()
        //        }).ToList();
        //        return list;
        //    }
        //}
        public static List<Form_Procedures> Process2Procedure(List<Form_Process> process, string ticket, string approvalName)
        {
            List<Form_Procedures> procedures = new List<Form_Procedures>();
            foreach (var item in process)
            {
                procedures.Add(new Form_Procedures()
                {
                    ID = Guid.NewGuid().ToString(),
                    FORM_INDEX = item.FORM_INDEX,
                    CREATER_NAME = item.CREATER_NAME,
                    CREATE_DATE = item.CREATE_DATE,
                    APPROVAL_NAME = approvalName,
                    FORM_NAME = item.FORM_NAME,
                    RETURN_INDEX = item.RETURN_INDEX,
                    RETURN_STATION_NO = item.RETURN_STATION_NO,
                    STATION_NAME = item.STATION_NAME,
                    STATION_NO = item.STATION_NO,
                    TICKET = ticket,
                    UPDATER_NAME = item.UPDATER_NAME,
                    DES = item.DES,
                    UPDATE_DATE = item.UPDATE_DATE
                });
            }
            return procedures;
        }
    }

}
