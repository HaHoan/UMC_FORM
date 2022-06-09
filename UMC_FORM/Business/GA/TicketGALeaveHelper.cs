using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UMC_FORM.Models;
using UMC_FORM.Models.GA;

namespace UMC_FORM.Business.GA
{
    public class TicketGALeaveHelper : TicketHelper
    {
        public override Form_Procedures CheckIsSignature(string stationName, Form_Summary summary)
        {
            using(var db = new DataContext())
            {
                var form = db.GA_LEAVE_FORM.Where(m => m.TICKET == summary.TICKET &&  m.STATION_NAME.Trim() == stationName
                && m.IS_SIGNATURE == 1 && m.PROCEDURE_INDEX <= summary.PROCEDURE_INDEX).OrderByDescending(m => m.ORDER_HISTORY).FirstOrDefault();
                if (form != null)
                {
                    return new Form_Procedures()
                    {
                        UPDATE_DATE = form.UPD_DATE,
                        APPROVAL_NAME = form.SUBMIT_USER
                    };
                }
                else return null;

            }
        }

        public  static List<GA_LEAVE_FORM_ITEM> convertStringToListItem(string leaveItems, string prevID, string currentID)
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
                            SPEACIAL_LEAVE = item.SPEACIAL_LEAVE,
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


    }
}