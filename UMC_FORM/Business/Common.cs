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
            List<SignatureEntity> signatures = new List<SignatureEntity>();
            var formAccept = forms.Where(r => r.IS_SIGNATURE == true).OrderBy(o => o.ORDER_HISTORY);
            var index = formAccept.GroupBy(r => r.PROCEDURE_INDEX).Select(h => h.Key).Distinct();
            List<PR_ACC_F06> lst = new List<PR_ACC_F06>();
            foreach (var item in index)
            {
                var form = formAccept.FirstOrDefault(r => r.PROCEDURE_INDEX == item);
                var user = UserRepository.GetUser(form.CREATE_USER);
                signatures.Add(new SignatureEntity()
                {
                    signature = user.SIGNATURE,
                    date = form.UPD_DATE.ToString("dd-M-yyyy")
                });
            }
            return signatures;
        }
        public static string FormatPrice(this double str)
        {
            return str == 0 ? "" : str.ToString("#,##0.00");
        }
    }
}
