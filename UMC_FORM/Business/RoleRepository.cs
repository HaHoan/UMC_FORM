using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class RoleRepository
    {

        public static List<Form_Role> GetRoles()
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Roles.ToList();
            }
        }
        public static string GetRole(int roleId)
        {
            string result = "";
            using (DataContext context = new DataContext())
            {
                var role = context.Form_Roles.Find(roleId);
                if (role != null)
                {
                    result = role.NAME;
                }
            }
            return result;
        }
    }
}
