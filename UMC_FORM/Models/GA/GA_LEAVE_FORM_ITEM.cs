using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models.GA
{
    public class GA_LEAVE_FORM_ITEM : BaseModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string TICKET { get; set; }

        [Required]
        public string FULLNAME { get; set; }

        [Required]
        public string CODE { get; set; }

        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        public DateTime TIME_FROM { get; set; }

        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        public DateTime TIME_TO { get; set; }

        public double TOTAL { get; set; }
        public int NO { get; set; }
        [Required]
        public string REASON { get; set; }

        public bool SPEACIAL_LEAVE { get; set; }
        public string REMARK { get; set; }
    }
}