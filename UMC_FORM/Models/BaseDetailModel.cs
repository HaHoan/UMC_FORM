using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using UMC_FORM.Models.LCA;

namespace UMC_FORM.Models
{
    public class BaseDetailModel
    {
        public BaseDetailModel()
        {
            using (var db = new DataContext())
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                USERS = js.Serialize(db.Form_User.Select(r => new { name = r.CODE, username = r.NAME }).ToList());
            }
        }
        public Form_Summary SUMARY { get; set; }

        public List<string> PERMISSION { get; set; }
        public List<string> SUBMITS { get; set; }

        public List<StationApproveModel> STATION_APPROVE { get; set; }
        public string USERS
        {
            get;
            set;
        }
        public List<Form_Comment> LIST_COMMENT { get; set; }
    }
}