using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public partial class Form_Dept
    {
        [Key]
        public string DEPT { get; set; }

        [StringLength(50)]
        public string CODE_MNG { get; set; }
    }
}
