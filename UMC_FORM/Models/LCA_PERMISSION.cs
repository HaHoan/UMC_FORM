namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LCA_PERMISSION
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string ITEM_COLUMN { get; set; }

        [StringLength(50)]
        public string ITEM_COLUMN_PERMISSION { get; set; }

        [StringLength(50)]
        public string DEPT { get; set; }

        [Required]
        [StringLength(50)]
        public string PROCESS { get; set; }
    }
}
