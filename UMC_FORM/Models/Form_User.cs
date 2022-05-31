namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Form_User
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string CODE { get; set; }

        [Required]
        [StringLength(100)]
        public string PASSWORD { get; set; }

        [Required]
        [StringLength(50)]
        public string NAME { get; set; }

        [StringLength(20)]
        public string SHORT_NAME { get; set; }

        [StringLength(20)]
        public string DEPT { get; set; }

        [StringLength(50)]
        public string EMAIL { get; set; }

        [StringLength(50)]
        public string SIGNATURE { get; set; }

        public int ROLE_ID { get; set; }

        [NotMapped]
        public string POSITIONs { get; set; }
        [NotMapped]
        public List<Form_Position> POSTION_LIST { get; set; }

    }
}
