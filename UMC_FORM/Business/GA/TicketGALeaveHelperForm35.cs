using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        public TicketGALeaveHelperForm35()
        {
            ControllerName = "GAFormLeave";
            ProcessName = Constant.GA_LEAVE_FORM;
        }
        public override BaseResult SaveGALeaveItemDetail(DataContext db, GA_LEAVE_FORM_ITEM item)
        {
            return new BaseResult(STATUS.SUCCESS, "");
        }
        public override List<GA_LEAVE_FORM_ITEM> convertStringToListItem(string leaveItems, string prevID, string currentID)
        {
            try
            {
                using (var db = new DataContext())
                {
                    List<GA_LEAVE_FORM_ITEM> listLeaveItems = new List<GA_LEAVE_FORM_ITEM>();

                    // không sửa gì
                    if (string.IsNullOrEmpty(leaveItems))
                    {

                        listLeaveItems = db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == prevID).ToList();

                    }

                    // Khi sửa đổi items
                    else
                    {
                        var format = "dd/MM/yyyy";
                        var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                        listLeaveItems = JsonConvert.DeserializeObject<List<GA_LEAVE_FORM_ITEM>>(leaveItems, dateTimeConverter);
                    }

                    if (listLeaveItems == null || listLeaveItems.Count == 0)
                    {
                        return null;
                    }
                    var newListItem = new List<GA_LEAVE_FORM_ITEM>();
                    foreach (var item in listLeaveItems)
                    {
                        if (string.IsNullOrEmpty(item.CODE))
                        {
                            return null;
                        }

                        var itemDB = new GA_LEAVE_FORM_ITEM
                        {
                            TICKET = currentID,
                            NO = item.NO,
                            FULLNAME = item.FULLNAME,
                            CODE = item.CODE,
                            TIME_FROM = item.TIME_FROM,
                            TIME_TO = item.TIME_TO,
                            TOTAL = item.TOTAL,
                            REASON = string.IsNullOrEmpty(item.REASON) ? "" : item.REASON,
                            SPEACIAL_LEAVE = false,
                            REMARK = string.IsNullOrEmpty(item.REMARK) ? "" : item.REMARK,
                        };

                        newListItem.Add(itemDB);
                    }

                    return newListItem;

                }

            }
            catch (Exception e)
            {
                return null;
            }


        }

        public override List<GA_LEAVE_FORM_ITEM> GetGALeaveFormItem(DataContext db,string ID)
        {
            return db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == ID).ToList();
        }
    }
}