using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class UserRepository
    {

        public static int ValidateUser(Form_User user)
        {
            using (DataContext context = new DataContext())
            {
                var userExist = context.Form_User.FirstOrDefault(r => r.CODE == user.CODE && r.PASSWORD == user.PASSWORD);
                if (userExist is null)
                {
                    return -1;
                }
                var role = context.Form_Roles.Find(userExist.ROLE_ID);
                if (userExist.PASSWORD.ToUpper() == "UMCVN")
                {
                    return -2;
                }
                if (role.NAME.ToUpper() == "ADMIN")// Admin
                {
                    return 1;
                }
            }
            return 0;
        }
        public static async Task<int> ValidateUserAsync(Form_User user)
        {
            using (DataContext context = new DataContext())
            {
                var userExist = context.Form_User.FirstOrDefault(r => r.CODE == user.CODE && r.PASSWORD == user.PASSWORD);
                if (userExist is null)
                {
                    return -1;
                }
                if (userExist.PASSWORD.ToUpper() == "UMCVN")
                {
                    return -2;
                }
                var role = await context.Form_Roles.FindAsync(userExist.ROLE_ID);
                if (role.NAME.ToUpper() == "ADMIN")// Admin
                {
                    return 1;
                }
            }
            return 0;
        }
        public static Form_User GetUser(string username)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_User.Find(username);
            }
        }
        public static async Task<Form_User> GetUserAsync(string username)
        {
            using (DataContext context = new DataContext())
            {
                return await context.Form_User.FindAsync(username);
            }
        }
        public static void Update(string code, string newPass)
        {
            using (DataContext context = new DataContext())
            {
                var user = context.Form_User.Find(code);
                user.PASSWORD = newPass;
                context.Entry<Form_User>(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static bool Update(string code, Form_User user)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    var userId = context.Form_User.Find(code);
                    userId.CODE = user.CODE;
                    userId.DEPT = user.DEPT;
                    userId.EMAIL = user.EMAIL;
                    userId.ROLE_ID = user.ROLE_ID;
                    userId.NAME = user.NAME;
                    userId.PASSWORD = user.PASSWORD;
                    userId.SIGNATURE = user.SIGNATURE;
                    userId.POSITION = user.POSITION;
                    userId.SHORT_NAME = user.SHORT_NAME;
                    context.Entry<Form_User>(userId).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public static List<Form_User> GetUsers()
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_User.ToList();
            }
        }

        public static List<Form_User> GetUsersByDept(string dept)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_User.Where(m => m.DEPT == dept).ToList();
            }
        }
        public static List<Form_User> GetUsersEx(string code)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_User.Where(r => r.CODE != code).ToList();
            }
        }

        public static List<string> GetUsers(List<string> lstCode)
        {
            List<string> result = new List<string>();
            using (DataContext context = new DataContext())
            {
                foreach (var item in context.Form_User)
                {
                    if (lstCode.Contains(item.CODE))
                    {
                        result.Add(item.EMAIL);
                    }
                }
            }
            return result;
        }

        public static bool Save(Form_User user)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    context.Form_User.Add(user);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.StackTrace);
                return false;
            }
        }
        public static void Remove(string code)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    var userId = context.Form_User.Find(code);
                    context.Form_User.Remove(userId);
                    context.SaveChanges();

                }

            }
            catch (Exception ex)
            {

            }
        }
    }

}
