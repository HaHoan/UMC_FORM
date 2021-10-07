using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class Form_Reject
    {
        [Key]
        public int ID { get; set; }

        public int FORM_INDEX { get; set; }
        public string PROCESS_NAME { get; set; }

        public int STEP_ORDER { get; set; }

        public int START_INDEX { get; set; }

        public int TOTAL_STEP { get; set; }
    }
}