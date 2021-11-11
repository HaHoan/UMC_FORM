namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Form_Procedures
    {
        public string ID { get; set; }

        [StringLength(50)]
        public string TICKET { get; set; }

        [StringLength(50)]
        public string FORM_NAME { get; set; }

        [StringLength(50)]
        public string STATION_NO { get; set; }

        [StringLength(50)]
        public string STATION_NAME { get; set; }

        public int? FORM_INDEX { get; set; }

        public int? RETURN_INDEX { get; set; }

        [StringLength(50)]
        public string CREATER_NAME { get; set; }

        public DateTime? CREATE_DATE { get; set; }

        [StringLength(50)]
        public string UPDATER_NAME { get; set; }

        public DateTime? UPDATE_DATE { get; set; }

        [StringLength(200)]
        public string DES { get; set; }

        [StringLength(50)]
        public string RETURN_STATION_NO { get; set; }

        [StringLength(50)]
        public string APPROVAL_NAME { get; set; }

      
    }
}
