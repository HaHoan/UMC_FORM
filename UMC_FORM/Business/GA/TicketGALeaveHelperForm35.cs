using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;
using UMC_FORM.Models.GA;

namespace UMC_FORM.Business.GA
{
    public class TicketGALeaveHelperForm35 : TicketGALeaveHelper
    {
        public override BaseResult SaveGALeaveItemDetail(DataContext db, GA_LEAVE_FORM_ITEM item)
        {
            return new BaseResult(STATUS.SUCCESS, "");
        }
    }
}