using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Models
{
    public class AssetEntity
    {
        public int assetIndex { get; set; }
        public string assetType { get; set; }
        public string accountCode { get; set; }
        public string assetNo { get; set; }

        public string costCenter { get; set; }
    }
}
