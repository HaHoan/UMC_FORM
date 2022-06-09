using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class MailResponsitory
    {
        public static  bool SendMail(Form_Summary summary, string typeMail, string controller)
        {
            try
            {
                using (var db = new DataContext())
                {
                    List<string> userMails = new List<string>();
                    var dear = "Dear All !";
                    if (summary.PROCEDURE_INDEX == -1)
                    {
                        var userCreate = UserRepository.GetUser(summary.CREATE_USER);
                        userMails.Add(userCreate.EMAIL);
                        var name = string.IsNullOrEmpty(userCreate.SHORT_NAME) ? userCreate.NAME : userCreate.SHORT_NAME;
                        dear = $"Dear {name} san !";
                    }
                    else
                    {
                        var stations = db.Form_Procedures.Where(m => m.TICKET == summary.TICKET &&
                        m.FORM_INDEX == (summary.PROCEDURE_INDEX + 1) &&
                        m.FORM_NAME == summary.PROCESS_ID).Select(m => m.APPROVAL_NAME).ToList();
                        userMails = UserRepository.GetUsers((List<string>)stations);
                        if (userMails.Count == 1)
                        {
                            var userApproval = db.Form_User.Where(m => stations.Contains(m.CODE) && m.EMAIL == userMails.FirstOrDefault()).FirstOrDefault();
                            if (userApproval != null)
                            {
                                var name = string.IsNullOrEmpty(userApproval.SHORT_NAME) ? userApproval.NAME : userApproval.SHORT_NAME;
                                dear = $"Dear {name} san !";
                            }

                        }
                    }


                    string body = "";
                    var domain = Bet.Util.Config.GetValue("domain");
                    if (typeMail == STATUS.REJECT)
                    {
                        body = $@"
                                                <h3>{dear}</h3>
                                                <h3 style='color: red' >Request reject. Please click below link view details:</h3>
	                                            <a href='{domain}{controller}/Details?ticket={summary.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                    }
                    else if (typeMail == STATUS.ACCEPT)
                    {
                        body = $@"
                                                <h3>{dear}</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='{domain}{controller}/Details?ticket={summary.TICKET}'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
                    }

                    BackgroundJob.Enqueue(() => MailHelper.SenMailOutlookAsync(userMails, body, null));
                    return true;
                }
            }
            catch (Exception)
            {
                return false;

            }
        }

    }
}