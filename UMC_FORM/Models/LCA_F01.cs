namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LCA_F01
    {
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(50)]
        public string TICKET { get; set; }

        [StringLength(50)]
        public string STATION_NO { get; set; }

        public int? PROCEDURE_INDEX { get; set; }
    }
}
