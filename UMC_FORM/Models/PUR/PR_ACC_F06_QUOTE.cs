using System.ComponentModel.DataAnnotations;
namespace UMC_FORM.Models.PUR
{
    public class PR_ACC_F06_QUOTE
    {
        [StringLength(50)]
        public string TICKET { get; set; }

        [StringLength(50)]
        public string NO { get; set; }

        [StringLength(50)]
        public string CODE_NAME { get; set; }

        [StringLength(50)]
        public string ITEM_NAME { get; set; }

        [StringLength(50)]
        public string DES { get; set; }

        [StringLength(50)]
        public string VENDOR { get; set; }

        public int? UNIT_PRICE { get; set; }

        public int? QTY { get; set; }

        [StringLength(50)]
        public string UNIT { get; set; }

        public decimal? AMOUNT { get; set; }

        [StringLength(50)]
        public string OWNER_OF_ITEM { get; set; } = "U";

        [StringLength(50)]
        public string COST_CENTER { get; set; }

        [StringLength(50)]
        public string AK { get; set; }

        [StringLength(50)]
        public string ACOUNT_CODE { get; set; }

        [StringLength(50)]
        public string ASSET_NO { get; set; }

    }
}