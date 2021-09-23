namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LCA_QUOTE
    {
        public int ID { get; set; }

        public int NO { get; set; }

        [Required]
        [StringLength(50)]
        public string REQUEST_ITEM { get; set; }

        public int QUANTITY { get; set; }

        public double LCA_UNIT_PRICE { get; set; }

        public double LCA_TOTAL_COST { get; set; }

        public double CUSTOMER_UNIT_PRICE { get; set; }

        public double CUSTOMER_TOTAL_COST { get; set; }

        [Required]
        [StringLength(50)]
        public string ID_TICKET { get; set; }
    }
}
