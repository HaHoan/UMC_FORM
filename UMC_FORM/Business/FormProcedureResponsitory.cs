using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;
using UMC_FORM.Models.GA;

namespace UMC_FORM.Business
{
    public static class FormProcedureResponsitory
    {
        public static bool CheckNextStepHaveApprover(string ticket, int nextProcedureIndex)
        {
            using (var db = new DataContext())
            {
                var nextStation = db.Form_Procedures.Where(m => m.TICKET == ticket && m.FORM_INDEX == nextProcedureIndex).ToList();
                foreach(var station in nextStation)
                {
                    if (!string.IsNullOrEmpty(station.APPROVAL_NAME) && station.APPROVAL_NAME != "0")
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        public static List<Form_Procedures> SetUpFormProceduce(string processName, GA_LEAVE_FORM ticket, List<Form_Process> process, string code)
        {
            using (var db = new DataContext())
            {
                var listProceduces = new List<Form_Procedures>();
                var formStation = db.Form_Stations.Where(m => m.PROCESS == processName).ToList();

                foreach (var pro in process)
                {
                    if (pro.FORM_INDEX == 0)
                    {
                        var proceduce = new Form_Procedures()
                        {
                            ID = Guid.NewGuid().ToString(),
                            TICKET = ticket.TICKET,
                            FORM_NAME = processName,
                            STATION_NO = pro.STATION_NO,
                            STATION_NAME = pro.STATION_NAME,
                            FORM_INDEX = pro.FORM_INDEX,
                            RETURN_INDEX = pro.RETURN_INDEX,
                            CREATER_NAME = pro.CREATER_NAME,
                            CREATE_DATE = pro.CREATE_DATE,
                            UPDATER_NAME = pro.UPDATER_NAME,
                            UPDATE_DATE = pro.UPDATE_DATE,
                            DES = pro.DES,
                            RETURN_STATION_NO = pro.RETURN_STATION_NO,
                            APPROVAL_NAME = code
                        };
                        listProceduces.Add(proceduce);
                    }
                    else if (pro.FORM_INDEX == 1)
                    {
                        var proceduce = new Form_Procedures()
                        {
                            ID = Guid.NewGuid().ToString(),
                            TICKET = ticket.TICKET,
                            FORM_NAME = processName,
                            STATION_NO = pro.STATION_NO,
                            STATION_NAME = pro.STATION_NAME,
                            FORM_INDEX = pro.FORM_INDEX,
                            RETURN_INDEX = pro.RETURN_INDEX,
                            CREATER_NAME = pro.CREATER_NAME,
                            CREATE_DATE = pro.CREATE_DATE,
                            UPDATER_NAME = pro.UPDATER_NAME,
                            UPDATE_DATE = pro.UPDATE_DATE,
                            DES = pro.DES,
                            RETURN_STATION_NO = pro.RETURN_STATION_NO,
                            APPROVAL_NAME = ticket.GROUP_LEADER
                        };
                        listProceduces.Add(proceduce);
                    }
                    else if (pro.FORM_INDEX == 2)
                    {

                        var proceduce = new Form_Procedures()
                        {
                            ID = Guid.NewGuid().ToString(),
                            TICKET = ticket.TICKET,
                            FORM_NAME = processName,
                            STATION_NO = pro.STATION_NO,
                            STATION_NAME = pro.STATION_NAME,
                            FORM_INDEX = pro.FORM_INDEX,
                            RETURN_INDEX = pro.RETURN_INDEX,
                            CREATER_NAME = pro.CREATER_NAME,
                            CREATE_DATE = pro.CREATE_DATE,
                            UPDATER_NAME = pro.UPDATER_NAME,
                            UPDATE_DATE = pro.UPDATE_DATE,
                            DES = pro.DES,
                            RETURN_STATION_NO = pro.RETURN_STATION_NO,
                            APPROVAL_NAME = ticket.DEPT_MANAGER
                        };
                        listProceduces.Add(proceduce);
                    }
                    else
                    {
                        var listUserApprover = formStation.Where(m => m.FORM_INDEX == pro.FORM_INDEX).ToList();
                        foreach (var userApprover in listUserApprover)
                        {
                            var proceduce = new Form_Procedures()
                            {
                                ID = Guid.NewGuid().ToString(),
                                TICKET = ticket.TICKET,
                                FORM_NAME = processName,
                                STATION_NO = pro.STATION_NO,
                                STATION_NAME = pro.STATION_NAME,
                                FORM_INDEX = pro.FORM_INDEX,
                                RETURN_INDEX = pro.RETURN_INDEX,
                                CREATER_NAME = pro.CREATER_NAME,
                                CREATE_DATE = pro.CREATE_DATE,
                                UPDATER_NAME = pro.UPDATER_NAME,
                                UPDATE_DATE = pro.UPDATE_DATE,
                                DES = pro.DES,
                                RETURN_STATION_NO = pro.RETURN_STATION_NO,
                                APPROVAL_NAME = userApprover.USER_ID
                            };
                            listProceduces.Add(proceduce);

                        }

                    }


                }
                return listProceduces;
            }

        }

        public static List<string> GetListApprover(Form_Summary summary, string currentUserCode)
        {
            using (var db = new DataContext())
            {
                var list = new List<string>();
                var userApprover = db.Form_Procedures.Where(m => m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1)
                                       && m.FORM_NAME == summary.PROCESS_ID && m.TICKET == summary.TICKET).ToList();
                if (userApprover.Where(m => m.APPROVAL_NAME.ToLower() == currentUserCode.ToLower()).FirstOrDefault() != null)
                {
                    if (summary.IS_REJECT)
                    {
                        list.Add(SUBMIT.RE_APPROVE);
                        if (summary.PROCEDURE_INDEX == -1)
                        {
                            list.Add(SUBMIT.DELETE);
                        }
                    }
                    else
                    {
                        list
.Add(SUBMIT.APPROVE);
                    }

                }
                return list;
            }
        }

        public static List<StationApproveModel> GetListApproved(Form_Summary summary, List<GA_LEAVE_FORM> list)
        {
            using(var db = new DataContext())
            {
                var listApproved = new List<StationApproveModel>();
                var process = db.Form_Procedures.Where(m => m.TICKET == summary.TICKET && m.FORM_NAME == summary.PROCESS_ID).OrderBy(m => m.FORM_INDEX).ToList();
                foreach (var pro in process)
                {
                    if (listApproved.Where(m => m.STATION_NAME.Trim() == pro.STATION_NAME.Trim()).FirstOrDefault() != null) continue;
                    var station = new StationApproveModel()
                    {
                        STATION_NAME = pro.STATION_NAME,
                        IS_APPROVED = false

                    };
                    var listForm = list.Where(m => m.STATION_NAME.Trim() == pro.STATION_NAME.Trim()
                    && m.IS_SIGNATURE == 1 && m.PROCEDURE_INDEX <= summary.PROCEDURE_INDEX).OrderByDescending(m => m.ORDER_HISTORY).FirstOrDefault();
                    
                    if (listForm != null)
                    {
                        station.IS_APPROVED = true;
                        station.APPROVE_DATE = listForm.UPD_DATE;
                        station.APPROVER = listForm.SUBMIT_USER;
                        station.COMPANY = "UMCVN";
                        var user = db.Form_User.Where(m => m.CODE == station.APPROVER).FirstOrDefault();
                        if (user != null)
                        {
                            station.SIGNATURE = user.SIGNATURE;
                        }
                        else
                        {
                            station.SIGNATURE = station.APPROVER;
                        }
                    }
                    listApproved.Add(station);
                }
                return listApproved;
            }
            
        }
    }
}