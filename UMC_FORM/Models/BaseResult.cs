using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class BaseResult
    {
        public BaseResult()
        {
        }

        public BaseResult(string result, string v)
        {
            this.result = result;
            this.message = v;
        }

        public string result { get; set; }
        public object message { get; set; }
    }
}