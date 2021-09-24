using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public partial class Form_Summary
    {
        [Key]
        public string ID { get; set; }

        public string TICKET { get; set; }

        public bool IS_FINISH { get; set; }

        public bool IS_REJECT { get; set; }

        public bool IS_RETURN { get; set; }

        public int PROCEDURE_INDEX { get; set; }

        public string CREATE_USER { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime UPD_DATE { get; set; }

        public string TITLE { get; set; }
        public int RETURN_TO { get; set; } = 0;
        public int? LAST_INDEX { get; set; } = 0;
        public string PROCESS_ID { get; set; }
    }
}
