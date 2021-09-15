using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class ProcessRepository
    {
        public static List<Form_ProcessName> GetProcessName()
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_ProcessNames.ToList();
            }
        }
        public static List<Form_Process> GetProcessName(string processId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Process.Where(r => r.FORM_NAME == processId).ToList();
            }
        }

        public static Form_Process GetProcess(int index)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Process.FirstOrDefault(r => r.FORM_INDEX == index);
            }
        }

        public static bool SaveProcessName(Form_ProcessName entity)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    context.Form_ProcessNames.Add(entity);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public static int MaxIndex(string formName)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Process.Where(r => r.FORM_NAME == formName).Max(h => h.FORM_INDEX);

            }
        }
    }
}
