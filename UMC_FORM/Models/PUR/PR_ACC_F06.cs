namespace UMC_FORM.Models.PUR
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using UMC_FORM.Business;

    public partial class PR_ACC_F06
    {
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(50)]
        public string TICKET { get; set; }

        [StringLength(50)]
        public string STATION_NO { get; set; }

        public int PROCEDURE_INDEX { get; set; }

        [StringLength(50)]
        public string FORM_NAME { get; set; }

        [StringLength(50)]
        public string TITLE { get; set; } = Constant.PR_ACC_F06_TITLE;

        public string TYPE_SUPPLY { get; set; }

       
        [Column(TypeName = "date")]
        public DateTime? ISSUE_DATE { get; set; }

        [StringLength(50)]
        public string DEPT { get; set; }

        [StringLength(50)]
        public string NAME { get; set; }

     

        [StringLength(50)]
        public string CREATE_USER { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime UPD_DATE { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime REQUEST_DATE { get; set; }

        [StringLength(100)]
        public string REMARK { get; set; }

        public bool IS_ROUND_ROBIN { get; set; }

        public bool IS_REQ_CUS { get; set; }

        public bool IS_CUS_PAY { get; set; }

        [StringLength(200)]
        public string CUS_PAY_REMARK { get; set; }
      
        public bool? IS_SIGNATURE { get; set; }

        public int? ORDER_HISTORY { get; set; }
        [NotMapped]
        public List<CostChangeEntity> histories { get; set; }

        [NotMapped]
        public AuthorEntity author { get; set; }

        [StringLength(50)]
        public string EXCHANGE { get; set; }


    }
}
