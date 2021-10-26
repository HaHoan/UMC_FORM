namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Form_Dept
    {
        [Key]
        [StringLength(50)]
        public string DEPT { get; set; }

        [StringLength(50)]
        public string CODE_MNG { get; set; }
    }
}
