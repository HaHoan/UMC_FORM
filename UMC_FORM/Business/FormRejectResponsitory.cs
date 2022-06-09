using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class FormRejectResponsitory
    {
        public static void CheckFormReject(Form_Summary summary, Action<int, bool> result)
        {
            using(var db = new DataContext())
            {
                var processReject = db.Form_Reject.Where(m => m.PROCESS_NAME == summary.PROCESS_ID && m.START_INDEX == summary.RETURN_TO).ToList();
                var currentStep = processReject.Where(m => m.FORM_INDEX == summary.PROCEDURE_INDEX).FirstOrDefault();
                if (currentStep != null)
                {

                    if (currentStep.STEP_ORDER == currentStep.TOTAL_STEP)
                    {
                        result(summary.REJECT_INDEX, false);
                    }
                    else
                    {
                        var nextStep = processReject.Where(m => m.STEP_ORDER == currentStep.STEP_ORDER + 1).FirstOrDefault();
                        if (nextStep != null && nextStep.FORM_INDEX < summary.REJECT_INDEX)
                        {
                            result(nextStep.FORM_INDEX, true);
                        }
                        else
                        {
                            result(summary.REJECT_INDEX, false);
                        }

                    }
                }
                else
                {
                    result(summary.REJECT_INDEX, false);
                }
            }
          
        }
    }
}