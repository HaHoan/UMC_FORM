using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class Common
    {
        public static List<SignatureEntity> GetSignatures(List<PR_ACC_F06> forms)
        {
            List<SignatureEntity> authors = new List<SignatureEntity>();
            var formAccept = forms.Where(r => r.IS_SIGNATURE == true).OrderBy(o => o.ORDER_HISTORY);
            var index = formAccept.GroupBy(r => r.PROCEDURE_INDEX).Select(h => h.Key);
            foreach (var item in formAccept)
            {
                var orderMin = formAccept.Where(r => r.PROCEDURE_INDEX == item.PROCEDURE_INDEX).Select(r => r.ORDER_HISTORY).Min();
                var form = formAccept.FirstOrDefault(r => r.PROCEDURE_INDEX == item.PROCEDURE_INDEX && r.ORDER_HISTORY == orderMin);
                if (form != null)
                {
                    var user = UserRepository.GetUser(form.CREATE_USER);
                    authors.Add(new SignatureEntity()
                    {
                        signature = user.SIGNATURE,
                        date = form.UPD_DATE.ToString("dd-M-yyyy")
                    });
                }
            }
            return authors;
        }

    }
}
