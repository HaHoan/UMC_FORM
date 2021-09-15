using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public  class Repository
    {
        private DataContext db = new DataContext();
        public List<PR_ACC_F06> GetAccF06()
        {
            var list = db.PR_ACC_F06.ToList().GroupBy(r => new { r.TICKET }).Select(h => new PR_ACC_F06()
            {
                TICKET = h.Key.TICKET,
                PROCEDURE_INDEX = h.Max(d => d.PROCEDURE_INDEX),
                UPD_DATE = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.UPD_DATE).FirstOrDefault(),
                CREATE_USER = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.CREATE_USER).FirstOrDefault(),
                TITLE = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.TITLE).FirstOrDefault(),
                ID = h.Where(i => i.PROCEDURE_INDEX == h.Max(d => d.PROCEDURE_INDEX)).Select(t => t.ID).FirstOrDefault()
            }).ToList();
            return list;
        }
    }

}
