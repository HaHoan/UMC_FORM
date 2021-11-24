using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public partial class Form_Log
    {
        [Key]
        public int ID { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string IP_ADDRESS { get; set; }
        public string HOST_NAME { get; set; }
        public int EXECUTE_RESULT { get; set; }
        [MaxLength(2000, ErrorMessage = "Vượt quá độ dài cho phép")]
        public string EXECUTE_RESULT_DETAILS { get; set; }
        public string OPERATE_TYPE { get; set; }
        public DateTime OPERATE_TIME { get; set; }
        public string DESCRIPTION { get; set; }

    }
}
