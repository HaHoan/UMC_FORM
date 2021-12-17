using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{

    public static class ProcessRepository
    {

        public static List<Form_Process> GetAllStation()
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    var listStation = context.Form_Process.GroupBy(m => m.STATION_NO).ToList();
                    var list = new List<Form_Process>();
                    foreach (var station in listStation)
                    {
                        var stationName = context.Form_Process.Where(m => m.STATION_NO == station.Key).FirstOrDefault();

                        list.Add(new Form_Process()
                        {
                            STATION_NO = station.Key,
                            STATION_NAME = stationName.STATION_NAME
                        });
                    }
                    return list;
                }
            }
            catch (Exception)
            {

                return null;
            }

        }
        public static List<string> GetAllPermission()
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                  return context.LCA_PERMISSION.GroupBy(m => m.ITEM_COLUMN).Select(m => m.Key).ToList();
                 
                }
            }
            catch (Exception)
            {

                return null;
            }

        }
        public static List<LCA_PERMISSION> GetAllPermission(string processId)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    return context.LCA_PERMISSION.Where(m => m.PROCESS == processId).ToList();

                }
            }
            catch (Exception)
            {

                return null;
            }

        }
        public static List<Form_ProcessName> GetProcessName()
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_ProcessNames.ToList();
            }
        }
        public static List<Form_Reject> GetRejectList(string process)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Reject.Where(m => m.PROCESS_NAME == process).ToList();
            }
        }
        public static bool IsExistProcessName(string processName)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_ProcessNames.Where(m => m.PROCESS_NAME == processName).FirstOrDefault() != null;
            }

        }
        public static List<Form_Process> GetProcessName(string processId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Process.Where(r => r.FORM_NAME == processId).ToList();
            }
        }

        public static Form_Process GetProcess(string processId, int index)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Process.FirstOrDefault(r => r.FORM_NAME == processId && r.FORM_INDEX == index);
            }
        }
        public static List<Form_Process> GetProcess(string processId)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Process.Where(r => r.FORM_NAME == processId).ToList();
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
