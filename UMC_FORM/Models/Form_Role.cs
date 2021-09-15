using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public partial class Form_Role
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string NAME { get; set; }
    }
}
