using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models.GA
{
    public class GA_LEAVE_FORM_ITEM_DETAIL
    {
        [Key]
        public int ID { get; set; }
        public int GA_LEAVE_FORM_ITEM_ID { get; set; }
        public DateTime ? TIME_LEAVE { get; set; }
    }
}