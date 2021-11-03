using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models.LCA
{
    public class LCADetailModel
    {
        public LCA_FORM_01 TICKET { get; set; }
        public Form_Summary SUMARY { get; set; }

        public List<string> PERMISSION { get; set; }
        public List<string> SUBMITS { get; set; }
        
        public List<StationApproveModel> STATION_APPROVE { get; set; }
        public string  USERS { get; set; }
        public List<Form_Comment> LIST_COMMENT { get; set; }
    }
}