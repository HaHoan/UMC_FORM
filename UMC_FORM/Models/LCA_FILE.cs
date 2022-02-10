namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LCA_FILE
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string TICKET { get; set; }

        [Required]
        public string FILE_URL { get; set; }

        [Required]
        [StringLength(50)]
        public string ID_TICKET { get; set; }

        [Required]
        public string FILE_NAME { get; set; }

        public string FILE_TYPE { get; set; }
    }
}
