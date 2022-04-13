using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models.GA
{
    public class GA_LEAVE_FORM
    {
        [Key]
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(50)]
        public string TICKET { get; set; }

        [StringLength(50)]
        public string DEPT { get; set; }

        [StringLength(50)]
        public string CREATOR { get; set; }

        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime DATE_REGISTER { get; set; }

        public int NUMBER_REGISTER { get; set; }
        public string FORM_NAME { get; set; }

        [NotMapped]
        public string DEPT_MANAGER { get; set; }

        [NotMapped]
        public string SHIFT_MANAGER { get; set; }

        public int PROCEDURE_INDEX { get; set; }

        public DateTime UPD_DATE { get; set; }

        public int IS_SIGNATURE { get; set; }

        public int ORDER_HISTORY { get; set; }

        [StringLength(50)]
        public string STATION_NAME { get; set; }
        public string STATION_NO { get; set; }


        public string SUBMIT_USER { get; set; }

        public string TITLE { get; set; }
        public string COMMENT { get; set; }

        [NotMapped]
        public List<GA_LEAVE_FORM_ITEM> GA_LEAVE_FORM_ITEMs { get; set; }
    }
}