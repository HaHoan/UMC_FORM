using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class LogRepository
    {
        public static void SaveLog(Form_Log log)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    context.Form_Logs.Add(log);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
