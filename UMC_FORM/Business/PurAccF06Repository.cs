using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class PurAccF06Repository
    {
        /// <summary>
        /// Kiểm tra tính hợp lệ
        /// -1: Không chọn loại giấy tờ
        /// -2: Không nhập nội dung
        /// -3: Không nhập ngày yêu cầu giao hàng
        /// 0: OK
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Validate(PR_ACC_F06 entity)
        {
            if (!entity.IS_S1 && !entity.IS_S2 && !entity.IS_S3 && !entity.IS_S4 && !entity.IS_S5 && !entity.IS_S6 && !entity.IS_S7 && !entity.IS_S8 && !entity.IS_S9 && !entity.IS_S10)
            {
                return -1;// Không chọn loại giấy từ
            }
            if (entity.ITEM_NAME_1 == null && entity.ITEM_NAME_2 == null &&
                entity.ITEM_NAME_3 == null && entity.ITEM_NAME_4 == null &&
                entity.ITEM_NAME_5 == null && entity.ITEM_NAME_6 == null &&
                entity.ITEM_NAME_7 == null && entity.ITEM_NAME_8 == null &&
                entity.ITEM_NAME_9 == null && entity.ITEM_NAME_10 == null)
            {
                return -2;// Không chọn loại giấy từ
            }
            if (entity.REQUEST_DATE == null)
            {
                return -3; //Không nhập ngày yêu cầu giao hàng
            }
            //if (DateTime.Compare(entity.REQUEST_DATE, DateTime.Now) < 0)
            //{
            //    return -4; //Ngày yêu cầu giao hàng trước ngày hiện tại
            //}
            return 0;
        }

        public static void SetAmounts(PR_ACC_F06 entity)
        {
            if (entity.QTY_1 != null)
            {
                entity.AMOUNT_1 = entity.UNIT_PRICE_1 * entity.QTY_1;
            }
            if (entity.QTY_2 != null)
            {
                entity.AMOUNT_2 = entity.UNIT_PRICE_2 * entity.QTY_2;
            }
            if (entity.QTY_3 != null)
            {
                entity.AMOUNT_3 = entity.UNIT_PRICE_3 * entity.QTY_3;
            }
            if (entity.QTY_4 != null)
            {
                entity.AMOUNT_4 = entity.UNIT_PRICE_4 * entity.QTY_4;
            }
            if (entity.QTY_5 != null)
            {
                entity.AMOUNT_5 = entity.UNIT_PRICE_5 * entity.QTY_5;
            }
            if (entity.QTY_6 != null)
            {
                entity.AMOUNT_6 = entity.UNIT_PRICE_6 * entity.QTY_6;
            }
            if (entity.QTY_7 != null)
            {
                entity.AMOUNT_7 = entity.UNIT_PRICE_7 * entity.QTY_7;
            }
            if (entity.QTY_8 != null)
            {
                entity.AMOUNT_8 = entity.UNIT_PRICE_8 * entity.QTY_8;
            }
            if (entity.QTY_9 != null)
            {
                entity.AMOUNT_9 = entity.UNIT_PRICE_9 * entity.QTY_9;
            }
            if (entity.QTY_10 != null)
            {
                entity.AMOUNT_10 = entity.UNIT_PRICE_10 * entity.QTY_10;
            }
        }

        public static bool Update(PR_ACC_F06 entity)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        context.Database.Log = Console.Write;
                        #region Save Summary
                        var summary = context.Form_Summary.FirstOrDefault(r => r.TICKET == entity.TICKET);
                        summary.PROCEDURE_INDEX = entity.PROCEDURE_INDEX;
                        summary.UPD_DATE = entity.UPD_DATE;
                        summary.IS_FINISH = entity.PROCEDURE_INDEX == 7;
                        context.Entry<Form_Summary>(summary).State = EntityState.Modified;
                        context.SaveChanges();
                        #endregion
                        #region Save Form
                        context.PR_ACC_F06.Add(entity);
                        context.SaveChanges();
                        #endregion
                        transaction.Commit();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        public static bool Update(PR_ACC_F06 entity, int procedureIndex, bool reject, bool finish)
        {
            try
            {
                using (DataContext context = new DataContext())
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        context.Database.Log = Console.Write;
                        #region Save Summary
                        var summary = context.Form_Summary.FirstOrDefault(r => r.TICKET == entity.TICKET);
                        summary.PROCEDURE_INDEX = procedureIndex;
                        summary.IS_REJECT = reject;
                        summary.IS_FINISH = finish;
                        summary.UPD_DATE = entity.UPD_DATE;
                        context.Entry<Form_Summary>(summary).State = EntityState.Modified;
                        context.SaveChanges();
                        #endregion
                        #region Save Form
                        context.PR_ACC_F06.Add(entity);
                        context.SaveChanges();
                        #endregion
                        transaction.Commit();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static List<PR_ACC_F06> GetForms(string ticket)
        {
            using (DataContext context = new DataContext())
            {
                return context.PR_ACC_F06.Where(r => r.TICKET == ticket).ToList();
            }
        }
        public static List<PR_ACC_F06> GetForms(string ticket, string createUser, bool signature)
        {
            using (DataContext context = new DataContext())
            {
                return context.PR_ACC_F06.Where(r => r.TICKET == ticket && r.CREATE_USER.ToUpper() == createUser && r.IS_SIGNATURE == signature).ToList();
            }
        }
        public static List<PR_ACC_F06> GetFormsByUser(string createUser, bool signature)
        {
            using (DataContext context = new DataContext())
            {
                return context.PR_ACC_F06.Where(r => r.CREATE_USER.ToUpper() == createUser && r.IS_SIGNATURE == signature).ToList();
            }
        }
        public static bool IsSignature(string ticket, int index)
        {
            using (DataContext context = new DataContext())
            {
                return context.PR_ACC_F06.Any(r => r.TICKET == ticket && r.PROCEDURE_INDEX == index);
            }
        }

        /// <summary>
        /// Lấy dữ liệu form cuối cùng
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static PR_ACC_F06 GetForm(string ticket)
        {
            using (DataContext context = new DataContext())
            {
                return context.PR_ACC_F06.Where(r => r.TICKET == ticket).OrderByDescending(o => o.ORDER_HISTORY).FirstOrDefault();
            }
        }

        public static string ChooseFormProcess(PR_ACC_F06 entity)
        {
            string cus = "C";
            if (entity.OWNER_OF_ITEM_1.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_2.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_3.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_4.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_5.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_6.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_7.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_8.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_9.ToUpper().Equals(cus) ||
                entity.OWNER_OF_ITEM_10.ToUpper().Equals(cus))
            {
                return Constant.PR_ACC_F06_BC_NAME;
            }
            return Constant.PR_ACC_F06_NAME;
        }

    }
}
