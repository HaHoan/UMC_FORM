using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public partial class Form_ProcessName
    {
        [Key]
        public string PROCESS_ID { get; set; }

        [StringLength(50)]
        public string PROCESS_NAME { get; set; }


        [StringLength(50)]
        public string UPD_USER { get; set; }

        public DateTime UPD_TIME { get; set; }

    }
}
