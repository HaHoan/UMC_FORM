namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LCA_FORM_01
    {

        [StringLength(50)]
        public string ID { get; set; }

        [Required]
        [StringLength(50)]
        public string TICKET { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime? LEAD_TIME { get; set; }

        public int PROCEDURE_INDEX { get; set; }

        [StringLength(50)]
        public string FORM_NAME { get; set; }

        [Required]
        [StringLength(50)]
        public string TITLE { get; set; }

        [Required]
        [StringLength(50)]
        public string SUBMIT_USER { get; set; }

        [Required]
        [StringLength(50)]
        public string CREATE_USER { get; set; }

        public DateTime UPD_DATE { get; set; }

        [StringLength(50)]
        public string REMARK { get; set; }

        public int IS_SIGNATURE { get; set; }

        public int ORDER_HISTORY { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime REQUEST_DATE { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime TARGET_DATE { get; set; }

        [StringLength(500)]
        public string REQUEST_TARGET { get; set; }

        public int DECREASE_PERSON { get; set; }

        [StringLength(500)]
        public string CONTENT_ERROR { get; set; }

        [StringLength(50)]
        public string ERROR_RATE_CURRENT { get; set; }

        [StringLength(500)]
        public string IMPROVED_EFICIENCY { get; set; }

        [StringLength(500)]
        public string COST_SAVING { get; set; }

        [StringLength(500)]
        public string OTHER { get; set; }

        [StringLength(50)]
        public string PAYER { get; set; }

        [StringLength(50)]
        public string CUSTOMER { get; set; }

        [StringLength(50)]
        public string PCB { get; set; }

        [StringLength(50)]
        public string MODEL { get; set; }

        [StringLength(500)]
        public string REQUEST_CONTENT { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime? RECEIVE_DATE { get; set; }

        [StringLength(500)]
        public string COMMENT { get; set; }

        [Required]
        [StringLength(50)]
        public string DEPT { get; set; }

        [StringLength(50)]
        public string LCA_ID { get; set; }

        [NotMapped]
        public List<LCA_QUOTE> LCA_QUOTEs { get; set; }

        [NotMapped]
        public List<LCA_FILE> FILES { get; set; }
    }
}
