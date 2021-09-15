using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class CostChangeEntity
    {
        public string unitPrice { get; set; }
        //public int? price { get; set; }
        public List<int?> prices { get; set; }
    }
}
