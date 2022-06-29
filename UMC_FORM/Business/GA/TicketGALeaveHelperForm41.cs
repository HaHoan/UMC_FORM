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
    public class TicketGALeaveHelperForm41:TicketGALeaveHelper
    {
        public TicketGALeaveHelperForm41()
        {
            ControllerName = "GAFormLeave41";
            ProcessName = Constant.GA_LEAVE_FORM41;
        }
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

        public override List<GA_LEAVE_FORM_ITEM> convertStringToListItem(string leaveItems, string prevID, string currentID)
        {
            try
            {
                using (var db = new DataContext())
                {
                    List<GA_LEAVE_FORM_ITEM> listLeaveItems = new List<GA_LEAVE_FORM_ITEM>();
                    List<GA_LEAVE_FORM_ITEM_DETAIL> listDetailTimeleave = new List<GA_LEAVE_FORM_ITEM_DETAIL>();
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
                        if (item.GA_LEAVE_FORM_ITEM_DETAILs == null)
                        {
                            var detail_timeleave = db.GA_LEAVE_FORM_ITEM_DETAIL.Where(m => m.GA_LEAVE_FORM_ITEM_ID == item.ID).ToList();

                            var itemDB = new GA_LEAVE_FORM_ITEM
                            {
                                TICKET = currentID,
                                NO = item.NO,
                                FULLNAME = item.FULLNAME,
                                CODE = item.CODE,
                                TIME_FROM = DateTime.Now,
                                TIME_TO = DateTime.Now,
                                TOTAL = item.TOTAL,
                                REASON = string.IsNullOrEmpty(item.REASON) ? "" : item.REASON,
                                SPEACIAL_LEAVE = item.SPEACIAL_LEAVE,
                                REMARK = string.IsNullOrEmpty(item.REMARK) ? "" : item.REMARK,
                                GA_LEAVE_FORM_ITEM_DETAILs = detail_timeleave,
                                CUSTOMER = item.CUSTOMER,
                            };
                            newListItem.Add(itemDB);
                        }
                        else
                        {
                            var itemDB = new GA_LEAVE_FORM_ITEM
                            {
                                TICKET = currentID,
                                NO = item.NO,
                                FULLNAME = item.FULLNAME,
                                CODE = item.CODE,
                                TIME_FROM = DateTime.Now,
                                TIME_TO = DateTime.Now,
                                TOTAL = item.TOTAL,
                                REASON = string.IsNullOrEmpty(item.REASON) ? "" : item.REASON,
                                SPEACIAL_LEAVE = item.SPEACIAL_LEAVE,
                                REMARK = string.IsNullOrEmpty(item.REMARK) ? "" : item.REMARK,
                                GA_LEAVE_FORM_ITEM_DETAILs = item.GA_LEAVE_FORM_ITEM_DETAILs,
                                CUSTOMER = item.CUSTOMER,
                            };
                            newListItem.Add(itemDB);
                        }
                    }
                    return newListItem;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override List<GA_LEAVE_FORM_ITEM> GetGALeaveFormItem(DataContext db, string ID)
        {
            var gaLeaveFormItems =  db.GA_LEAVE_FORM_ITEM.Where(m => m.TICKET == ID).ToList();
            foreach (var item in gaLeaveFormItems)
            {
                var item_detail = db.GA_LEAVE_FORM_ITEM_DETAIL.Where(m => m.GA_LEAVE_FORM_ITEM_ID == item.ID).ToList();
                item.GA_LEAVE_FORM_ITEM_DETAILs = item_detail;
            }
            return gaLeaveFormItems;
        }
    }
}