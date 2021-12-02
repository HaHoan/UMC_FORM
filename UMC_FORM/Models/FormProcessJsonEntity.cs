using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class FormProcessJsonEntity
    {
        public int index { get; set; }

        public string key { get; set; }

        public string name { get; set; }
        public string no { get; set; }

        public string _return { get; set; }

        public int? returnTo { get; set; }

        public List<Form_Reject> rejectList { get; set; }
        public List<LCA_PERMISSION> permission { get; set; }

    }
}
