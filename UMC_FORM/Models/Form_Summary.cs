namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Form_Summary
    {
        [Key]
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(50)]
        public string TICKET { get; set; }

        [StringLength(50)]
        public string PROCESS_ID { get; set; }

        public bool IS_FINISH { get; set; }

        public bool IS_REJECT { get; set; }

        public bool? IS_RETURN { get; set; }

        public int PROCEDURE_INDEX { get; set; }

        [StringLength(50)]
        public string CREATE_USER { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime UPD_DATE { get; set; }

        [StringLength(100)]
        public string TITLE { get; set; }

        public int RETURN_TO { get; set; }

        public int? LAST_INDEX { get; set; }

        public int? TOTAL_MONEY { get; set; }
        
        public int REJECT_INDEX { get; set; }

       
        public bool USE_PUR { get; set; }

        [StringLength(50)]
        public string PURPOSE { get; set; }
    }
}
