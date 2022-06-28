using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;
using UMC_FORM.Models.GA;

namespace UMC_FORM.Business.GA
{
    public class TicketGALeaveHelperForm41:TicketGALeaveHelper
    {
        public override BaseResult SaveGALeaveItemDetail(DataContext db, GA_LEAVE_FORM_ITEM item) {
            if (item.GA_LEAVE_FORM_ITEM_DETAILs == null)
            {
                return new BaseResult(STATUS.ERROR, "Xem lại thông tin đăng ký!");
            }
            foreach (var item_detail in item.GA_LEAVE_FORM_ITEM_DETAILs)
            {
                var new_itemdetail = new GA_LEAVE_FORM_ITEM_DETAIL
                {
                    GA_LEAVE_FORM_ITEM_ID = item.ID,
                    TIME_LEAVE = item_detail.TIME_LEAVE,
                };
                db.GA_LEAVE_FORM_ITEM_DETAIL.Add(new_itemdetail);
                db.SaveChanges();
            }
            return new BaseResult(STATUS.SUCCESS, "");
        }
    }
}