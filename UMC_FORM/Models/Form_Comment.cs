using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public partial class Form_Comment
    {
        public string ID { get; set; }

        [StringLength(50)]
        public string TICKET { get; set; }

        [StringLength(50)]
        public string COMMENT_DETAIL { get; set; }

        [StringLength(50)]
        public string COMMENT_USER { get; set; }

        public DateTime UPD_DATE { get; set; }

        public int ORDER_HISTORY { get; set; }
    }
}
