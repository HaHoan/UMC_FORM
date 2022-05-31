using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class Form_Position
    {
        [Key]
        public int ID { get; set; }
        public string NAME { get; set; }
        public string CODE { get; set; }
        public int POSITION_CODE { get; set; }

    }
}