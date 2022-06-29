using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UMC_FORM.Models;
using UMC_FORM.Models.GA;

namespace UMC_FORM.Business.GA
{
    public abstract class TicketGALeaveHelper : TicketHelper
    {
        public string ControllerName { get; set; }
        public string ProcessName { get; set; }
        public override Form_Procedures CheckIsSignature(string stationName, Form_Summary summary)
        {
            using (var db = new DataContext())
            {
                var form = db.GA_LEAVE_FORM.Where(m => m.TICKET == summary.TICKET && m.STATION_NAME.Trim() == stationName
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
        public abstract List<GA_LEAVE_FORM_ITEM> GetGALeaveFormItem(DataContext db, string ID);
        public abstract List<GA_LEAVE_FORM_ITEM> convertStringToListItem(string leaveItems, string prevID, string currentID);
        public abstract BaseResult SaveGALeaveItemDetail(DataContext db, GA_LEAVE_FORM_ITEM item);
        public BaseResult Create(object obj, Form_User _sess)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ticket = (GA_LEAVE_FORM)obj;
                        ticket.ID = Guid.NewGuid().ToString();
                        ticket.TICKET = DateTime.Now.ToString("yyyyMMddHHmmss");
                        ticket.CREATOR = _sess.CODE;
                        ticket.ORDER_HISTORY = 1;
                        ticket.PROCEDURE_INDEX = 0;
                        ticket.DEPT = _sess.DEPT;
                        ticket.SUBMIT_USER = _sess.CODE;
                        ticket.IS_SIGNATURE = 1;
                        ticket.UPD_DATE = DateTime.Now;
                        var process = db.Form_Process.Where(m => m.FORM_NAME == ProcessName).ToList();
                        var station = process.Where(m => m.FORM_INDEX == ticket.PROCEDURE_INDEX).FirstOrDefault();
                        ticket.STATION_NAME = station.STATION_NAME;
                        ticket.STATION_NO = station.STATION_NO;
                        ticket.GA_LEAVE_FORM_ITEMs = convertStringToListItem(ticket.leaveItems, ticket.ID, ticket.ID);

                        if (ticket.GA_LEAVE_FORM_ITEMs == null)
                        {
                            transaction.Rollback();
                            return new BaseResult(STATUS.ERROR, "Xem lại thông tin người đăng kí!");
                        }

                        foreach (var item in ticket.GA_LEAVE_FORM_ITEMs)
                        {
                            db.GA_LEAVE_FORM_ITEM.Add(item);
                            db.SaveChanges();
                            var result = SaveGALeaveItemDetail(db, item);
                            if (result.result == STATUS.ERROR)
                            {
                                transaction.Rollback();
                                return result;
                            }
                        }

                        ticket.NUMBER_REGISTER = ticket.GA_LEAVE_FORM_ITEMs.Count();
                        db.GA_LEAVE_FORM.Add(ticket);
                        db.SaveChanges();
                        var listProcedures = FormProcedureResponsitory.SetUpFormProceduce(ProcessName, ticket, process, _sess.CODE);

                        if (listProcedures == null)
                        {
                            if (ticket.GA_LEAVE_FORM_ITEMs == null)
                            {
                                transaction.Rollback();
                                return new BaseResult(STATUS.ERROR, "Không lấy được giá trị của process!");
                            }

                        }

                        foreach (var pro in listProcedures)
                        {
                            db.Form_Procedures.Add(pro);
                            db.SaveChanges();
                        }

                        var summary = new Form_Summary()
                        {
                            ID = Guid.NewGuid().ToString(),
                            IS_FINISH = false,
                            IS_REJECT = false,
                            PROCEDURE_INDEX = 0,
                            TICKET = ticket.TICKET,
                            CREATE_USER = _sess.CODE,
                            UPD_DATE = DateTime.Now,
                            TITLE = ticket.TITLE,
                            PURPOSE = ticket.TITLE,
                            PROCESS_ID = ProcessName,
                            LAST_INDEX = process.Count() - 1
                        };
                        db.Form_Summary.Add(summary);
                        db.SaveChanges();
                        transaction.Commit();
                        // Nếu người tạo trùng với quản lý ca => thực hiện tự động accept bước tiếp theo
                        var currentProceduce = db.Form_Procedures.Where(m => m.TICKET == ticket.TICKET
                        && m.FORM_INDEX == (ticket.PROCEDURE_INDEX + 1)).FirstOrDefault();

                        if (_sess.CODE == currentProceduce.APPROVAL_NAME)
                        {
                            if (string.IsNullOrEmpty(ticket.DEPT_MANAGER))
                            {
                                return new BaseResult(STATUS.ERROR, "Bạn cần chọn trưởng phòng!");
                            }
                            return Accept(ticket, _sess);
                        }

                        if (!MailResponsitory.SendMail(summary, STATUS.ACCEPT, ControllerName))
                        {
                            return new BaseResult(STATUS.ERROR, "Error when send mail!");
                        };
                        return new BaseResult(STATUS.SUCCESS, summary.TICKET);

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new BaseResult(STATUS.ERROR, ex.ToString());
                    }
                }
            }
        }
        public BaseResult Accept(object obj, Form_User _sess)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ticket = (GA_LEAVE_FORM)obj;
                        var formDB = db.GA_LEAVE_FORM.Where(m => m.ID == ticket.ID).FirstOrDefault();
                        if (formDB == null)
                        {
                            return new BaseResult()
                            {
                                result = STATUS.ERROR,
                                message = "Ticket không tồn tại"
                            };
                        }
                        var form = formDB.CloneObject() as GA_LEAVE_FORM;
                        var summary = db.Form_Summary.Where(m => m.TICKET == ticket.TICKET).FirstOrDefault();
                        if (summary.IS_REJECT)
                        {
                            // check xem đã đi hết các bước reject chưa
                            FormRejectResponsitory.CheckFormReject(summary, (procedure_index, is_reject) =>
                            {
                                form.PROCEDURE_INDEX = procedure_index;
                                summary.IS_REJECT = is_reject;
                            });
                        }
                        else
                        {
                            form.PROCEDURE_INDEX += 1;
                        }
                        form.IS_SIGNATURE = 1;
                        summary.PROCEDURE_INDEX = form.PROCEDURE_INDEX;
                        if (summary.PROCEDURE_INDEX == summary.LAST_INDEX)
                        {
                            summary.IS_FINISH = true;
                        }

                        form.ID = Guid.NewGuid().ToString();
                        form.SUBMIT_USER = _sess.CODE;
                        form.UPD_DATE = DateTime.Now;
                        form.ORDER_HISTORY++;
                        if (!summary.IS_FINISH)
                        {
                            var isCheckNextStep = false;
                            if (form.PROCEDURE_INDEX == 1)
                            {
                                if (!string.IsNullOrEmpty(ticket.DEPT_MANAGER) && ticket.DEPT_MANAGER != "0")
                                {
                                    var nextStation = db.Form_Procedures.Where(m => m.TICKET == form.TICKET && m.FORM_INDEX == 2).FirstOrDefault();
                                    nextStation.APPROVAL_NAME = ticket.DEPT_MANAGER;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    isCheckNextStep = true;
                                }

                            }
                            else if (form.PROCEDURE_INDEX == 0)
                            {
                                if (!string.IsNullOrEmpty(ticket.GROUP_LEADER) && ticket.GROUP_LEADER != "0")
                                {
                                    var nextStation = db.Form_Procedures.Where(m => m.TICKET == form.TICKET && m.FORM_INDEX == 1).FirstOrDefault();
                                    nextStation.APPROVAL_NAME = ticket.GROUP_LEADER;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    isCheckNextStep = true;
                                }
                            }
                            else
                            {
                                isCheckNextStep = true;
                            }
                            if (isCheckNextStep)
                            {
                                var isHaveDeptManager = FormProcedureResponsitory.CheckNextStepHaveApprover(form.TICKET, form.PROCEDURE_INDEX + 1);
                                if (!isHaveDeptManager)
                                {
                                    transaction.Rollback();
                                    return new BaseResult()
                                    {
                                        result = STATUS.ERROR,
                                        message = "Bước tiếp theo chưa có người xác nhận!"
                                    };
                                }
                            }
                        }

                        var process = db.Form_Process.Where(m => m.FORM_NAME == ProcessName).ToList();
                        var station = process.Where(m => m.FORM_INDEX == form.PROCEDURE_INDEX).FirstOrDefault();
                        form.STATION_NAME = station.STATION_NAME;
                        form.STATION_NO = station.STATION_NO;
                        form.GA_LEAVE_FORM_ITEMs = convertStringToListItem(ticket.leaveItems, ticket.ID, form.ID);
                        if (form.GA_LEAVE_FORM_ITEMs == null)
                        {
                            transaction.Rollback();
                            return new BaseResult()
                            {
                                result = STATUS.ERROR,
                                message = "Xem lại thông tin người đăng kí!"
                            };
                        }
                        foreach (var item in form.GA_LEAVE_FORM_ITEMs)
                        {
                            db.GA_LEAVE_FORM_ITEM.Add(item);
                            db.SaveChanges();
                            var result = SaveGALeaveItemDetail(db, item);
                            if (result.result == STATUS.ERROR)
                            {
                                transaction.Rollback();
                                return result;
                            }
                        }
                        form.NUMBER_REGISTER = form.GA_LEAVE_FORM_ITEMs.Count();
                        db.GA_LEAVE_FORM.Add(form);
                        db.SaveChanges();

                        transaction.Commit();

                        // Nếu người tạo trùng với quản lý ca => thực hiện tự động accept bước tiếp theo
                        var currentProceduces = db.Form_Procedures.Where(m => m.TICKET == ticket.TICKET && m.FORM_INDEX == (form.PROCEDURE_INDEX + 1)).ToList();
                        foreach (var currentProceduce in currentProceduces)
                        {
                            if (currentProceduce != null && _sess.CODE == currentProceduce.APPROVAL_NAME)
                            {
                                return Accept(form, _sess);
                            }
                        }

                        if (!MailResponsitory.SendMail(summary, STATUS.ACCEPT, "GAFormLeave"))
                        {
                            transaction.Rollback();
                            return new BaseResult()
                            {
                                result = STATUS.ERROR,
                                message = "Error when send mail!"
                            };
                        };

                        return new BaseResult()
                        {
                            result = STATUS.SUCCESS,
                            message = summary.TICKET
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new BaseResult()
                        {
                            result = STATUS.ERROR,
                            message = ex.Message.ToString()
                        };
                    }
                }
            }
        }
        public BaseResult Reject(object obj, Form_User _sess)
        {
            using (var db = new DataContext())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ticket = (GA_LEAVE_FORM)obj;
                        var formDb = db.GA_LEAVE_FORM.Where(m => m.ID == ticket.ID).FirstOrDefault();
                        if (formDb == null)
                            return new BaseResult()
                            {
                                result = STATUS.ERROR,
                                message = "Ticket không tồn tại"
                            };
                        var form = formDb.CloneObject() as GA_LEAVE_FORM;

                        var summary = db.Form_Summary.Where(m => m.TICKET == formDb.TICKET).FirstOrDefault();
                        var process = db.Form_Process.Where(m => m.FORM_NAME == summary.PROCESS_ID).ToList();
                        var currentProcess = db.Form_Process.Where(m => m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1)
                        && m.FORM_NAME == summary.PROCESS_ID).FirstOrDefault();

                        if (currentProcess == null)
                        {
                            return new BaseResult(STATUS.ERROR, "Check reject process again!");
                        }
                        var station = process.Where(m => m.FORM_INDEX == (form.PROCEDURE_INDEX + 1)).FirstOrDefault();
                        form.STATION_NAME = station.STATION_NAME;
                        form.STATION_NO = station.STATION_NO;
                        form.PROCEDURE_INDEX = currentProcess.RETURN_INDEX is int returnIndex ? returnIndex - 1 : 0;
                        form.ORDER_HISTORY += 1;
                        form.IS_SIGNATURE = 0;
                        form.ID = Guid.NewGuid().ToString();
                        form.SUBMIT_USER = _sess.CODE;
                        form.COMMENT = ticket.COMMENT;
                        form.GA_LEAVE_FORM_ITEMs = convertStringToListItem(ticket.leaveItems, ticket.ID, form.ID);
                        if (form.GA_LEAVE_FORM_ITEMs == null)
                        {
                            transaction.Rollback();
                            return new BaseResult(STATUS.ERROR, "Xem lại thông tin người đăng kí!");
                        }
                        foreach (var item in form.GA_LEAVE_FORM_ITEMs)
                        {
                            db.GA_LEAVE_FORM_ITEM.Add(item);
                            db.SaveChanges();
                            var result = SaveGALeaveItemDetail(db, item);
                            if (result.result == STATUS.ERROR)
                            {
                                transaction.Rollback();
                                return result;
                            }
                        }
                        form.NUMBER_REGISTER = form.GA_LEAVE_FORM_ITEMs.Count();
                        db.GA_LEAVE_FORM.Add(form);

                        // để lưu lại bước sẽ quay lại sau khi luồng reject được thực hiện xong
                        summary.REJECT_INDEX = formDb.PROCEDURE_INDEX;
                        summary.IS_REJECT = true;

                        // Để lưu lại bước bị reject về
                        summary.RETURN_TO = form.PROCEDURE_INDEX;
                        summary.PROCEDURE_INDEX = form.PROCEDURE_INDEX;

                        db.SaveChanges();
                        transaction.Commit();
                        if (!MailResponsitory.SendMail(summary, STATUS.REJECT, "GAFormLeave"))
                        {
                            transaction.Rollback();
                            return new BaseResult(STATUS.ERROR, "Error when send mail!");
                        };
                        return new BaseResult(STATUS.SUCCESS, "");
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return new BaseResult(STATUS.ERROR, e.Message.ToString());
                    }

                }
            }
        }
        public static BaseResult Delete(object obj)
        {
            var ticket = (GA_LEAVE_FORM)obj;
            var result = FormSummaryRepository.Delete(ticket.TICKET);
            if (result == STATUS.SUCCESS)
            {
                return new BaseResult(STATUS.SUCCESS, "");
            }
            else
            {
                return new BaseResult(STATUS.ERROR, result);
            }
        }

        public GA_LEAVE_FORM_DETAIL_MODEL GetDetailTicket(string ticket, Form_User _sess)
        {
            using (var db = new DataContext())
            {
                var modelDetail = new GA_LEAVE_FORM_DETAIL_MODEL();
                var list = db.GA_LEAVE_FORM.Where(m => m.TICKET == ticket).OrderByDescending(m => m.ORDER_HISTORY).ToList();

                if (list.Count == 0)
                {
                    return null;
                }

                modelDetail.TICKET = list.FirstOrDefault();
                if (modelDetail.TICKET == null)
                {
                    return null;
                }
                modelDetail.TICKET.GA_LEAVE_FORM_ITEMs = GetGALeaveFormItem(db,modelDetail.TICKET.ID);
                modelDetail.SUMARY = db.Form_Summary.Where(m => m.TICKET == ticket).FirstOrDefault();

                if (modelDetail.SUMARY == null)
                {
                    return null;
                }

                var deptMng = db.Form_Procedures.Where(m => m.TICKET == ticket && m.STATION_NO == "DEPT_MANAGER").FirstOrDefault();

                if (deptMng != null && !string.IsNullOrEmpty(deptMng.APPROVAL_NAME))
                {
                    modelDetail.TICKET.DEPT_MANAGER_OBJECT = UserRepository.GetUser(deptMng.APPROVAL_NAME);
                }

                var groupLeader = db.Form_Procedures.Where(m => m.TICKET == ticket && m.STATION_NO == "GROUP_LEADER").FirstOrDefault();

                if (groupLeader != null && !string.IsNullOrEmpty(groupLeader.APPROVAL_NAME))
                {
                    modelDetail.TICKET.GROUP_LEADER_OBJECT = UserRepository.GetUser(groupLeader.APPROVAL_NAME);
                }

                modelDetail.PERMISSION = new List<string>();
                modelDetail.SUBMITS = new List<string>();
                if (_sess.ROLE_ID == ROLE.CanEdit || _sess.ROLE_ID == ROLE.Approval)
                {
                    modelDetail.PERMISSION = PermissionResponsitory.GetListPermission(modelDetail.SUMARY.PROCEDURE_INDEX + 1, modelDetail.SUMARY.PROCESS_ID);
                    modelDetail.SUBMITS = FormProcedureResponsitory.GetListApprover(modelDetail.SUMARY, _sess.CODE);
                }

                modelDetail.STATION_APPROVE = new TicketGALeaveHelperForm35().GetListApproved(modelDetail.SUMARY);
                return modelDetail;
            }
        }
    }
}