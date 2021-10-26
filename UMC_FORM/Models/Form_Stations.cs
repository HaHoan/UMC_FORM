namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Form_Stations
    {
        public string ID { get; set; }

        [StringLength(50)]
        public string STATION_NO { get; set; }

        [StringLength(50)]
        public string STATION_NAME { get; set; }

        [StringLength(50)]
        public string USER_ID { get; set; }

        [StringLength(50)]
        public string USER_NAME { get; set; }

        public int? FORM_INDEX { get; set; }

        [StringLength(50)]
        public string PROCESS { get; set; }
    }
}
