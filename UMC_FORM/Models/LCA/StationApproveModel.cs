using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models.LCA
{
    public class StationApproveModel
    {
        public string STATION_NAME { get; set; }
        public string STATION_NO { get; set; }
        public bool IS_APPROVED { get; set; }

        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime APPROVE_DATE { get; set; }
        public string APPROVER { get; set; }
        public string COMPANY { get; set; }
        public string SIGNATURE { get; set; }
    }
}