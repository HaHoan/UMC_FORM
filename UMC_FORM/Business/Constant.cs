using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Business
{
    public static class Constant
    {
        public static string PR_ACC_F06_NAME = "F06-PR-ACC-03-01";
        public static string PR_ACC_F06_TITLE = "F06-PR ACC 03-01 Giấy yêu cầu nhà cung cấp  購入申請書";
        public static string FM = "IWASAKI";
        public static string GD = "YOKOUCHI";
        //public static readonly string MAIL = "quyetpv@umcvn.com";
        //public static readonly string PASS = "Quyet@123";
        public static readonly string SUBJECT = "REQUEST ONLINE APPROVAL";
        public static readonly string BODY = @"
                                                <h3>Dear Mr!</h3>
                                                <h3 style='color: red' >You have a new Request need to be approved. Please click below link to approve it:</h3>
	                                            <a href='http://172.28.10.17:98'>Click to approval</a>
                                                <br />
                                                <h3>Thanks & Best regards</h3>
                                                <h4>*********************</h4>
                                                <h4>PE-IT</h4>
                                                <h4 style='font-weight: bold;'>UMC Electronic Viet Nam Ltd. </h4>
                                                <h4>Tan Truong IZ, Cam Giang, Hai Duong. </h4>
                                             ";
    }
}
