namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Form_Comment
    {
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(50)]
        public string TICKET { get; set; }

        [StringLength(200)]
        public string COMMENT_DETAIL { get; set; }

        [StringLength(50)]
        public string COMMENT_USER { get; set; }

        public DateTime? UPD_DATE { get; set; }

        public int? ORDER_HISTORY { get; set; }
    }
}
