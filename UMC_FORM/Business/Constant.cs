﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UMC_FORM.Business
{
    public static class Constant
    {
        public static string PR_ACC_F06_NAME = "F06-PR-ACC-03-01";
        public static string LCA_Process = "LCA_Process";
        public static string LCA_01 = "LCA_01";
        public static string PR_ACC_F06_BC_NAME = "F06-PR-ACC-03-01-BC";
        public static string PR_ACC_F06_TITLE = "F06-PR ACC 03-01 Giấy yêu cầu nhà cung cấp  購入申請書";
        public static string LCA_FORM_01_TITLE = "F01-LCA GIẤY YÊU CẦU PHÒNG CƠ KHÍ  冶工具　加工依頼書";
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
        public static string ERROR_NOTICE = "Có lỗi xảy ra";
    }

    public static class PERMISSION
    {
        public static string QUOTE = "QUOTE";
        public static string EDIT_QUOTE = "EDIT_QUOTE";
        public static string ATTACH_FILE = "ATTACH_FILE";
        public static string EDIT_INFO = "EDIT_INFO";
        public static string ADD_ID = "ADD_ID";
        public static string COMMENT = "COMMENT";
    }

    public static class STATUS
    {
        public static string ACCEPT = "accept";
        public static string REJECT = "reject";
        public static string SUCCESS = "success";
        public static string ERROR = "error";
    }

    public static class PAYER
    {
        public static string CUSTOMER = "Customer";
        public static string UMCVN  = "UMCVN";
    }
}
