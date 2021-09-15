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

    }
}
