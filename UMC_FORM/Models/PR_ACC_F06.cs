namespace UMC_FORM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;
    using Bet.Util;
    using UMC_FORM.Business;

    public partial class PR_ACC_F06
    {
        #region Property
        public string ID { get; set; }

        [StringLength(20)]
        public string TICKET { get; set; }

        [StringLength(50)]
        public string STATION_NO { get; set; }

        public int PROCEDURE_INDEX { get; set; } = 0;

        [StringLength(50)]
        public string FORM_NAME { get; set; }

        [StringLength(50)]
        public string TITLE { get; set; } = Constant.PR_ACC_F06_TITLE;

        public bool IS_S1 { get; set; }

        public bool IS_S2 { get; set; }

        public bool IS_S3 { get; set; }

        public bool IS_S4 { get; set; }

        public bool IS_S5 { get; set; }

        public bool IS_S6 { get; set; }

        public bool IS_S7 { get; set; }

        public bool IS_S8 { get; set; }

        public bool IS_S9 { get; set; }

        public bool IS_S10 { get; set; }

        [Column(TypeName = "date")]
        public DateTime ISSUE_DATE { get; set; }

        [StringLength(50)]
        public string DEPT { get; set; }

        [StringLength(50)]
        public string NAME { get; set; }

        [StringLength(50)]
        public string NO_1 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_1 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_1 { get; set; }

        [StringLength(50)]
        public string DES_1 { get; set; }

        [StringLength(50)]
        public string VENDOR_1 { get; set; }

        public int? UNIT_PRICE_1 { get; set; }

        public int? QTY_1 { get; set; }

        public string UNIT_1 { get; set; }

        public decimal? AMOUNT_1 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_1 { get; set; } = "U";

        public string COST_CENTER_1 { get; set; }

        [StringLength(50)]
        public string AK_1 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_1 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_1 { get; set; }

        [StringLength(50)]
        public string NO_2 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_2 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_2 { get; set; }

        [StringLength(50)]
        public string DES_2 { get; set; }

        [StringLength(50)]
        public string VENDOR_2 { get; set; }

        public int? UNIT_PRICE_2 { get; set; }

        public int? QTY_2 { get; set; }

        public string UNIT_2 { get; set; }

        public decimal? AMOUNT_2 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_2 { get; set; } = "U";

        public string COST_CENTER_2 { get; set; }

        [StringLength(50)]
        public string AK_2 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_2 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_2 { get; set; }

        [StringLength(50)]
        public string NO_3 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_3 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_3 { get; set; }

        [StringLength(50)]
        public string DES_3 { get; set; }

        [StringLength(50)]
        public string VENDOR_3 { get; set; }

        public int? UNIT_PRICE_3 { get; set; }

        public int? QTY_3 { get; set; }

        public string UNIT_3 { get; set; }

        public decimal? AMOUNT_3 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_3 { get; set; } = "U";

        public string COST_CENTER_3 { get; set; }

        [StringLength(50)]
        public string AK_3 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_3 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_3 { get; set; }

        [StringLength(50)]
        public string NO_4 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_4 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_4 { get; set; }

        [StringLength(50)]
        public string DES_4 { get; set; }

        [StringLength(50)]
        public string VENDOR_4 { get; set; }

        public int? UNIT_PRICE_4 { get; set; }

        public int? QTY_4 { get; set; }

        public string UNIT_4 { get; set; }

        public decimal? AMOUNT_4 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_4 { get; set; } = "U";

        public string COST_CENTER_4 { get; set; }

        [StringLength(50)]
        public string AK_4 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_4 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_4 { get; set; }

        [StringLength(50)]
        public string NO_5 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_5 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_5 { get; set; }

        [StringLength(50)]
        public string DES_5 { get; set; }

        [StringLength(50)]
        public string VENDOR_5 { get; set; }

        public int? UNIT_PRICE_5 { get; set; }

        public int? QTY_5 { get; set; }

        public string UNIT_5 { get; set; }

        public decimal? AMOUNT_5 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_5 { get; set; } = "U";

        public string COST_CENTER_5 { get; set; }

        [StringLength(50)]
        public string AK_5 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_5 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_5 { get; set; }

        [StringLength(50)]
        public string NO_6 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_6 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_6 { get; set; }

        [StringLength(50)]
        public string DES_6 { get; set; }

        [StringLength(50)]
        public string VENDOR_6 { get; set; }

        public int? UNIT_PRICE_6 { get; set; }

        public int? QTY_6 { get; set; }

        public string UNIT_6 { get; set; }

        public decimal? AMOUNT_6 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_6 { get; set; } = "U";

        public string COST_CENTER_6 { get; set; }

        [StringLength(50)]
        public string AK_6 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_6 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_6 { get; set; }

        [StringLength(50)]
        public string NO_7 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_7 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_7 { get; set; }

        [StringLength(50)]
        public string DES_7 { get; set; }

        [StringLength(50)]
        public string VENDOR_7 { get; set; }

        public int? UNIT_PRICE_7 { get; set; }

        public int? QTY_7 { get; set; }

        public string UNIT_7 { get; set; }

        public decimal? AMOUNT_7 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_7 { get; set; } = "U";

        public string COST_CENTER_7 { get; set; }

        [StringLength(50)]
        public string AK_7 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_7 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_7 { get; set; }

        [StringLength(50)]
        public string NO_8 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_8 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_8 { get; set; }

        [StringLength(50)]
        public string DES_8 { get; set; }

        [StringLength(50)]
        public string VENDOR_8 { get; set; }

        public int? UNIT_PRICE_8 { get; set; }

        public int? QTY_8 { get; set; }

        public string UNIT_8 { get; set; }

        public decimal? AMOUNT_8 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_8 { get; set; } = "U";

        public string COST_CENTER_8 { get; set; }

        [StringLength(50)]
        public string AK_8 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_8 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_8 { get; set; }

        [StringLength(50)]
        public string NO_9 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_9 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_9 { get; set; }

        [StringLength(50)]
        public string DES_9 { get; set; }

        [StringLength(50)]
        public string VENDOR_9 { get; set; }

        public int? UNIT_PRICE_9 { get; set; }

        public int? QTY_9 { get; set; }

        public string UNIT_9 { get; set; }

        public decimal? AMOUNT_9 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_9 { get; set; } = "U";

        public string COST_CENTER_9 { get; set; }

        [StringLength(50)]
        public string AK_9 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_9 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_9 { get; set; }

        [StringLength(50)]
        public string NO_10 { get; set; }

        [StringLength(50)]
        public string CODE_NAME_10 { get; set; }

        [StringLength(50)]
        public string ITEM_NAME_10 { get; set; }

        [StringLength(50)]
        public string DES_10 { get; set; }

        [StringLength(50)]
        public string VENDOR_10 { get; set; }

        public int? UNIT_PRICE_10 { get; set; }

        public int? QTY_10 { get; set; }

        public string UNIT_10 { get; set; }

        public decimal? AMOUNT_10 { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM_10 { get; set; } = "U";

        public string COST_CENTER_10 { get; set; }

        [StringLength(50)]
        public string AK_10 { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE_10 { get; set; }

        [StringLength(50)]
        public string ASSET_NO_10 { get; set; }

        [StringLength(50)]
        public string CREATE_USER { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime UPD_DATE { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime REQUEST_DATE { get; set; }

        [StringLength(100)]
        public string REMARK { get; set; }

        public bool IS_ROUND_ROBIN { get; set; }

        public bool IS_REQ_CUS { get; set; }

        public bool IS_CUS_PAY { get; set; }

        [StringLength(200)]
        public string CUS_PAY_REMARK { get; set; }

        [StringLength(150, ErrorMessage = "Max Length 150")]
        public string FILE_PATH_1 { get; set; }

        [StringLength(50)]
        public string FILE_NAME_1 { get; set; }

        [StringLength(150, ErrorMessage = "Max Length 150")]
        public string FILE_PATH_2 { get; set; }

        [StringLength(50)]
        public string FILE_NAME_2 { get; set; }

        [StringLength(150, ErrorMessage = "Max Length 150")]
        public string FILE_PATH_3 { get; set; }

        [StringLength(50)]
        public string FILE_NAME_3 { get; set; }

        [StringLength(150, ErrorMessage = "Max Length 150")]
        public string FILE_PATH_4 { get; set; }
        [StringLength(50)]
        public string FILE_NAME_4 { get; set; }

        [StringLength(150, ErrorMessage = "Max Length 150")]
        public string FILE_PATH_5 { get; set; }

        [StringLength(50)]
        public string FILE_NAME_5 { get; set; }

        public bool IS_SIGNATURE { get; set; }

        public int ORDER_HISTORY { get; set; }

        //[NotMapped]
        //public HttpPostedFileBase ImageFile { get; set; }

        [NotMapped]
        public List<CostChangeEntity> histories { get; set; }

        [NotMapped]
        public AuthorEntity author { get; set; }
        #endregion
    }
}
