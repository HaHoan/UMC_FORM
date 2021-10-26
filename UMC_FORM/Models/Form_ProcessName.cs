namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Form_ProcessName
    {
        [Key]
        [StringLength(50)]
        public string PROCESS_ID { get; set; }

        [StringLength(50)]
        public string PROCESS_NAME { get; set; }

        public DateTime? UPD_TIME { get; set; }

        [StringLength(50)]
        public string UPD_USER { get; set; }
    }
}
