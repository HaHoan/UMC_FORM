namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Form_Reject
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string PROCESS_NAME { get; set; }

        public int FORM_INDEX { get; set; }

        public int STEP_ORDER { get; set; }

        public int START_INDEX { get; set; }

        public int TOTAL_STEP { get; set; }
    }
}
