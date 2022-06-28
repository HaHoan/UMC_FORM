using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class BaseResult
    {
        private string eRROR;
        private string v;

        public BaseResult()
        {
        }

        public BaseResult(string eRROR, string v)
        {
            this.eRROR = eRROR;
            this.v = v;
        }

        public string result { get; set; }
        public object message { get; set; }
    }
}