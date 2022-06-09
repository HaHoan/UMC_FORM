using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class BaseModel
    {
        [Key]
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(50)]
        public string TICKET { get; set; }

        public int PROCEDURE_INDEX { get; set; }

        public DateTime UPD_DATE { get; set; }

        public int IS_SIGNATURE { get; set; }

        public int ORDER_HISTORY { get; set; }

        [StringLength(50)]
        public string STATION_NAME { get; set; }
        public string STATION_NO { get; set; }
        public string FORM_NAME { get; set; }

        public string SUBMIT_USER { get; set; }

        public string TITLE { get; set; }
        public string COMMENT { get; set; }
    }
}