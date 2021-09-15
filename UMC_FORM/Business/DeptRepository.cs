using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class DeptRepository
    {
        public static List<string> GetDepts()
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Depts.Select(r => r.DEPT).ToList();
            }
        }
        public static List<Form_Dept> GetAll()
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Depts.ToList();
            }
        }
        public static Form_Dept GetDept(string deptCode)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Depts.FirstOrDefault(r => r.DEPT == deptCode);
            }
        }
        public static Form_Dept GetDeptByMng(string code)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Depts.FirstOrDefault(r => r.CODE_MNG == code);
            }
        }

        public static bool IsMng(string code)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Depts.FirstOrDefault(r => r.CODE_MNG == code) != null;
            }
        }

        public static bool Save(Form_Dept entity)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    var exist = context.Form_Depts.Find(entity.DEPT);
                    if (exist is null)
                    {
                        context.Form_Depts.Add(entity);
                        context.SaveChanges();
                    }
                    else
                    {
                        exist.CODE_MNG = entity.CODE_MNG;
                        context.Entry<Form_Dept>(exist).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Remove(string deptCode)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    var dept = context.Form_Depts.Find(deptCode);
                    if (dept != null)
                    {
                        context.Form_Depts.Remove(dept);
                        context.SaveChanges();
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
