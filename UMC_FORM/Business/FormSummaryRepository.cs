using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class FormSummaryRepository
    {
        public static Form_Summary GetSummary(string ticket)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Summary.FirstOrDefault(r => r.TICKET == ticket);
            }
        }
        public static List<Form_Summary> GetSummaryLike(string ticket)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Summary.Where(r => r.TICKET.Contains(ticket)).ToList();
            }
        }

        public static string Delete(string ticket)
        {
            using (var db = new DataContext())
            {
                try
                {
                    var ticketDb = db.Form_Summary.Where(m => m.TICKET == ticket).FirstOrDefault();
                    db.Form_Summary.Remove(ticketDb);
                    db.SaveChanges();
                    return STATUS.SUCCESS;
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }

            }
        }
        
        public static List<Form_Summary> GetAllMyRequest(string userCode)
        {
            using(var db = new DataContext())
            {
                var formSummaries = new List<Form_Summary>();
                try
                {
                    var result = from p in db.Form_Summary
                                 where p.IS_FINISH == false
                                 join c in db.Form_Procedures on p.TICKET equals c.TICKET
                                 where c.APPROVAL_NAME == userCode && p.PROCEDURE_INDEX >= c.FORM_INDEX
                                 select new
                                 {
                                     ID = p.ID,
                                     IS_FINISH = p.IS_FINISH,
                                     IS_REJECT = p.IS_REJECT,
                                     PROCEDURE_INDEX = p.PROCEDURE_INDEX,
                                     TICKET = p.TICKET,
                                     CREATE_USER = p.CREATE_USER,
                                     UPD_DATE = p.UPD_DATE,
                                     TITLE = p.TITLE,
                                     PROCESS_ID = p.PROCESS_ID,
                                     PURPOSE = p.PURPOSE
                                 };
                    foreach (var p in result)
                    {
                        if (formSummaries.Find(m => m.TICKET == p.TICKET) == null)
                        {
                            formSummaries.Add(new Form_Summary()
                            {
                                ID = p.ID,
                                IS_FINISH = p.IS_FINISH,
                                IS_REJECT = p.IS_REJECT,
                                PROCEDURE_INDEX = p.PROCEDURE_INDEX,
                                TICKET = p.TICKET,
                                CREATE_USER = p.CREATE_USER,
                                UPD_DATE = p.UPD_DATE,
                                TITLE = p.TITLE,
                                PROCESS_ID = p.PROCESS_ID,
                                PURPOSE = p.PURPOSE
                            });

                        }

                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message.ToString());
                }
                return formSummaries;
            }
          
           
        }

        public static List<Form_Summary> GetAllSendToMe(string userCode)
        {
            using (var db = new DataContext())
            {
                var formSummaries = new List<Form_Summary>();
                try
                {
                    var result = from p in db.Form_Summary
                                 join c in db.Form_Procedures on
                                 p.TICKET equals c.TICKET
                                 where c.APPROVAL_NAME == userCode && c.FORM_INDEX == (p.PROCEDURE_INDEX + 1) && p.IS_FINISH == false
                                 select new
                                 {
                                     ID = p.ID,
                                     IS_FINISH = p.IS_FINISH,
                                     IS_REJECT = p.IS_REJECT,
                                     PROCEDURE_INDEX = p.PROCEDURE_INDEX,
                                     TICKET = p.TICKET,
                                     CREATE_USER = p.CREATE_USER,
                                     UPD_DATE = p.UPD_DATE,
                                     TITLE = p.TITLE,
                                     PROCESS_ID = p.PROCESS_ID,
                                     PURPOSE = p.PURPOSE
                                 };
                    foreach (var p in result)
                    {
                        if (formSummaries.Find(m => m.TICKET == p.TICKET) == null)
                        {
                            formSummaries.Add(new Form_Summary()
                            {
                                ID = p.ID,
                                IS_FINISH = p.IS_FINISH,
                                IS_REJECT = p.IS_REJECT,
                                PROCEDURE_INDEX = p.PROCEDURE_INDEX,
                                TICKET = p.TICKET,
                                CREATE_USER = p.CREATE_USER,
                                UPD_DATE = p.UPD_DATE,
                                TITLE = p.TITLE,
                                PROCESS_ID = p.PROCESS_ID,
                                PURPOSE = p.PURPOSE
                            });

                        }

                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message.ToString());
                }
                return formSummaries;
            }
        }
        public static List<Form_Summary> GetAllRejectToMe(string userCode)
        {
            using (var db = new DataContext())
            {
                var formSummaries = new List<Form_Summary>();
                try
                {
                    var result = from p in db.Form_Summary
                                 join c in db.Form_Procedures on
                                 p.TICKET equals c.TICKET
                                 where c.APPROVAL_NAME == userCode && c.FORM_INDEX == (p.PROCEDURE_INDEX + 1) && p.IS_FINISH == false && p.IS_REJECT == true
                                 select new
                                 {
                                     ID = p.ID,
                                     IS_FINISH = p.IS_FINISH,
                                     IS_REJECT = p.IS_REJECT,
                                     PROCEDURE_INDEX = p.PROCEDURE_INDEX,
                                     TICKET = p.TICKET,
                                     CREATE_USER = p.CREATE_USER,
                                     UPD_DATE = p.UPD_DATE,
                                     TITLE = p.TITLE,
                                     PROCESS_ID = p.PROCESS_ID,
                                     PURPOSE = p.PURPOSE
                                 };
                    foreach (var p in result)
                    {
                        if (formSummaries.Find(m => m.TICKET == p.TICKET) == null)
                        {
                            formSummaries.Add(new Form_Summary()
                            {
                                ID = p.ID,
                                IS_FINISH = p.IS_FINISH,
                                IS_REJECT = p.IS_REJECT,
                                PROCEDURE_INDEX = p.PROCEDURE_INDEX,
                                TICKET = p.TICKET,
                                CREATE_USER = p.CREATE_USER,
                                UPD_DATE = p.UPD_DATE,
                                TITLE = p.TITLE,
                                PROCESS_ID = p.PROCESS_ID,
                                PURPOSE = p.PURPOSE
                            });

                        }

                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message.ToString());
                }
                return formSummaries;
            }
        }

        public static List<Form_Summary> GetAllFollow(string userCode, string dept)
        {
            using (var db = new DataContext())
            {
                var formSummaries = new List<Form_Summary>();
                try
                {
                    var listFollow = db.Form_Summary.Where(r => r.IS_FINISH == false).ToList();
                    var listPermission = db.LCA_PERMISSION.ToList();
                    foreach (var item in listFollow)
                    {
                        var listPermission1 = listPermission.Where(m => m.ITEM_COLUMN_PERMISSION == (item.PROCEDURE_INDEX + 1).ToString()
                                          && m.PROCESS == item.PROCESS_ID).ToList();
                        if (listPermission1.Where(m => m.DEPT == dept).FirstOrDefault() != null)
                        {
                            formSummaries.Add(item);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message.ToString());
                }
                return formSummaries;
            }
        }

        public static List<Form_Summary> GetAllFinish(Form_User session)
        {
            using (var db = new DataContext())
            {
                var formSummaries = new List<Form_Summary>();
                try
                {
                    if (session.POSTION_LIST.Where(m => m.POSITION_CODE == POSITION.FM || m.POSITION_CODE == POSITION.GD).FirstOrDefault() != null)
                    {
                        formSummaries = db.Form_Summary.Where(r => r.IS_FINISH == true).ToList();
                    }
                    else
                    {
                        var deptHavePermission = db.LCA_PERMISSION.Where(m => m.DEPT == session.DEPT).Select(m => m.PROCESS).ToList();
                        var tickets = db.Form_Procedures.Where(m => m.APPROVAL_NAME == session.CODE || deptHavePermission.Contains(m.FORM_NAME)).Select(m => m.TICKET).ToList();
                        formSummaries = db.Form_Summary
.Where(m => m.IS_FINISH == true && tickets.Contains(m.TICKET)).ToList();
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message.ToString());
                }
                return formSummaries;
            }
        }
        public static int GetNumberFollowNotDone(Form_User session)
        {
            using (var db = new DataContext())
            {
                var formSummaries = new List<Form_Summary>();
                try
                {
                    var list = db.Form_Summary.Where(r => r.IS_FINISH == false).ToList();
                    var numberNotYet = 0;
                    var listPermission = db.LCA_PERMISSION.ToList();
                    foreach (var item in list)
                    {
                        if (item.STATUS == STATUS.QUOTED) continue;
                        var listPermission1 = listPermission.Where(m => m.ITEM_COLUMN_PERMISSION == (item.PROCEDURE_INDEX + 1).ToString()
                                             && m.PROCESS == item.PROCESS_ID).ToList();
                        if (listPermission1.Where(m => m.DEPT == session.DEPT).FirstOrDefault() != null)
                        {
                            numberNotYet++;
                        }
                    }
                    return numberNotYet;
                }
                catch (Exception e)
                {
                    Console.Write(e.Message.ToString());
                }
                return 0;
            }

        }
      
    }
}
