using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class StationMemberModel
    {
        public int index { get; set; }
        public List<Member> member { get; set; }
    }
    public class Member
    {
        public string code { get; set; }
        public string name { get; set; }
    }

}
