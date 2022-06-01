using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class UserRepository
    {
           
        public static List<Form_User> GetManagers(string dept = null)
        {
            using(var db = new DataContext())
            {
                var allUser = new List<Form_User>();
                if (!string.IsNullOrEmpty(dept))
                {
                    allUser = db.Form_User.Where(m => m.DEPT == dept).ToList();
                }
                else
                {
                    allUser = db.Form_User.ToList();
                }
                
                var mng = new List<Form_User>();
                foreach(var user in allUser)
                {
                    var pos = db.Form_Position.Where(m => m.CODE == user.CODE && m.POSITION_CODE == POSITION.MANAGER).FirstOrDefault();
                    if(pos != null)
                    {
                        mng.Add(user);
                    }
                }
                return mng;
            }
        }
        public static List<Form_User> GetGroupLeaders(string dept = null)
        {
            using (var db = new DataContext())
            {
                var allUser = new List<Form_User>();
                if (!string.IsNullOrEmpty(dept))
                {
                    allUser = db.Form_User.Where(m => m.DEPT == dept).ToList();
                }
                else
                {
                    allUser = db.Form_User.ToList();
                }

                var mng = new List<Form_User>();
                foreach (var user in allUser)
                {
                    var pos = db.Form_Position.Where(m => m.CODE == user.CODE && m.POSITION_CODE == POSITION.GROUP_LEADER).FirstOrDefault();
                    if (pos != null)
                    {
                        mng.Add(user);
                    }
                }
                return mng;
            }
        }

        public static int ValidateUser(Form_User user)
        {
            using (DataContext context = new DataContext())
            {
                var userExist = context.Form_User.FirstOrDefault(r => r.CODE == user.CODE && r.PASSWORD == user.PASSWORD);
                if (userExist is null)
                {
                    return -1;
                }
                var flag = context.Form_Logs.Any(r => r.USER_ID == userExist.ID && r.EXECUTE_RESULT == (int)EXECUTE_RESULT.SUCCESS);
                if (userExist.PASSWORD.ToUpper() == "UMCVN" || !flag)
                {
                    return -2;
                }
                var role = context.Form_Roles.Find(userExist.ROLE_ID);
                if (role.NAME.ToUpper() == "ADMIN")// Admin
                {
                    return 1;
                }
            }
            return 0;
        }
        public static List<Form_Position> GetAllPosition()
        {
            using (var db = new DataContext())
            {
                var list = db.Form_Position.GroupBy(m => m.POSITION_CODE).ToList();
                var listPosition = new List<Form_Position>();
                foreach (var position in list)
                {
                    listPosition.Add(new Form_Position()
                    {
                        POSITION_CODE = position.Key,
                        NAME = position.FirstOrDefault().NAME
                    });
                }
                return listPosition;
            }
        }
        public static int ValidateUserAsync(Form_User user)
        {
            using (DataContext context = new DataContext())
            {
                var userExist = context.Form_User.FirstOrDefault(r => r.CODE == user.CODE && r.PASSWORD == user.PASSWORD);
                if (userExist is null)
                {
                    return -1;
                }
                var flag = context.Form_Logs.Any(r => r.USER_ID == userExist.ID && r.EXECUTE_RESULT == (int)EXECUTE_RESULT.SUCCESS);
                if (userExist.PASSWORD.ToUpper() == "UMCVN" || !flag)
                {
                    return -2;
                }
                var role =  context.Form_Roles.Find(userExist.ROLE_ID);
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
                var user = context.Form_User.FirstOrDefault(r => r.CODE == username);
                if(user != null)
                {
                    user.POSTION_LIST = context.Form_Position.Where(m => m.CODE == username).ToList();
                }
                return user;
            }
        }
        public static Form_User GetUserByMail(string email)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_User.FirstOrDefault(r => r.EMAIL == email);
            }
        }
     
        public static void Update(string code, string newPass)
        {
            using (DataContext context = new DataContext())
            {
                var user = context.Form_User.FirstOrDefault(r => r.CODE == code);
                user.PASSWORD = newPass;
                context.Entry<Form_User>(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static bool Update(string code, Form_User user)
        {
            using (DataContext context = new DataContext())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var userId = context.Form_User.FirstOrDefault(r => r.CODE == code);
                        userId.CODE = user.CODE;
                        userId.DEPT = user.DEPT;
                        userId.EMAIL = user.EMAIL;
                        userId.ROLE_ID = user.ROLE_ID;
                        userId.NAME = user.NAME;
                        userId.PASSWORD = user.PASSWORD;
                        userId.SIGNATURE = user.SIGNATURE;
                        userId.SHORT_NAME = user.SHORT_NAME;
                        context.Entry<Form_User>(userId).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        if (!string.IsNullOrEmpty(user.POSITIONs))
                        {
                            var posOld = context.Form_Position.Where(m => m.CODE == userId.CODE).ToList();
                            context.Form_Position.RemoveRange(posOld);
                            var listPosition = JsonConvert.DeserializeObject<List<Form_Position>>(user.POSITIONs);
                            foreach (var pos in listPosition)
                            {
                                pos.CODE = user.CODE;
                                context.Form_Position.Add(pos);
                                context.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return false;
                    }

                }

            }
        }
        public static void Update(int userId, string password)
        {
            using (DataContext context = new DataContext())
            {
                var sql = $"UPDATE [UMC_FORM].[dbo].[Form_User] SET PASSWORD = '{password}'  where ID = {userId}";
                context.Database.ExecuteSqlCommand(sql);
                context.SaveChanges();
            }
        }

        public static List<Form_User> GetUsersByDept(string dept)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_User.Where(m => m.DEPT == dept).ToList();
            }
        }

        public static List<Form_User> GetUsers()
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_User.ToList();
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
                foreach (var item in lstCode)
                {
                    var user = context.Form_User.Where(m => m.CODE == item).FirstOrDefault();
                    if (user != null && !string.IsNullOrEmpty(user.EMAIL))
                    {
                        result.Add(user.EMAIL);
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
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            context.Form_User.Add(user);
                            context.SaveChanges();
                            if (!string.IsNullOrEmpty(user.POSITIONs))
                            {
                                var listPosition = JsonConvert.DeserializeObject<List<Form_Position>>(user.POSITIONs);
                                foreach (var pos in listPosition)
                                {
                                    pos.CODE = user.CODE;
                                    context.Form_Position.Add(pos);
                                    context.SaveChanges();
                                }
                            }
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return false;
                        }

                    }

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
                    var userId = context.Form_User.FirstOrDefault(r => r.CODE == code);
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
