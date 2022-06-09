using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class PermissionResponsitory
    {
        public static List<string> GetListPermission(int ProcedureIndex, string processId)
        {
            using(var db = new DataContext())
            {
                var permissions = new List<string>();
                var listPermission = db.LCA_PERMISSION.Where(m => m.ITEM_COLUMN_PERMISSION == ProcedureIndex.ToString()
                                    && m.PROCESS == processId).ToList();
                foreach (var permission in listPermission)
                {
                    permissions.Add(permission.ITEM_COLUMN);
                }
                return permissions;
            }
        }
    }
}