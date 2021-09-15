using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class ApprovalEntity
    {
        public int index { get; set; }
         public List<MemberEntity> member { get; set; }
    }

    public class MemberEntity
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
