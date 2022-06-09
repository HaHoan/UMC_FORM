using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models.GA
{
    public class GA_LEAVE_FORM : BaseModel
    {
        [StringLength(50)]
        public string DEPT { get; set; }

        [StringLength(50)]
        public string CREATOR { get; set; }

        public DateTime DATE_REGISTER { get; set; }

        public int NUMBER_REGISTER { get; set; }

        [NotMapped]
        public string DEPT_MANAGER { get; set; }

        [NotMapped]
        public Form_User DEPT_MANAGER_OBJECT { get; set; }

        [NotMapped]
        public string GROUP_LEADER { get; set; }

        [NotMapped]
        public Form_User GROUP_LEADER_OBJECT { get; set; }



        [NotMapped]
        public List<GA_LEAVE_FORM_ITEM> GA_LEAVE_FORM_ITEMs { get; set; }

        [NotMapped]
        public string leaveItems { get; set; }

        [NotMapped]
        public string status { get; set; }
    }
}