$(function () {
    /* Mutil Language */
    var lang = localStorage.getItem('lang') || 'en';
    changeLanguage(lang);

})
function changeLanguage(lang) {
    var base_url = window.location.origin;
    if (lang == "en") {
        $(".icon-lang").attr('src', base_url + "/Content/assets/img/united-states.png");
        $(".text-lang").text("Tiếng Anh")
    } else if (lang == "vi") {
        $(".icon-lang").attr('src', base_url + "/Content/assets/img/vietnam.png");
        $(".text-lang").text("Tiếng Việt")
    } else if (lang == "ja") {
        $(".icon-lang").attr('src', base_url + "/Content/assets/img/japan.png");
        $(".text-lang").text("Tiếng Nhật")
    }

    $.getJSON(base_url + "/Content/Json/lang.json", function (data) {
        $(".lang").each(function (index, element) {
            $(this).text(data[lang][$(this).attr("key")]);
            $(this).attr("placeholder", data[lang][$(this).attr("key")]);
        });
    });
}
$(".translate").click(function () {
    var lang = $(this).attr("id");
    localStorage.setItem('lang', lang);
    changeLanguage(lang);

});
//var arrLang = {
//    "en": {
//        "Home": "Home",
//        "Summary": "Summary",
//        "Search-for": "Search for...",
//        "Username": "Username",
//        "create-request": "Create New Request",
//        "send-to-me": "Send To Me",
//        "my-request": "My Request",
//        "cancel-request": "Cancel Request",
//        "finish-request": "Finish Request",
//        "list-request": "REQUEST LIST",
//        "all": "All",
//        "in-turn": "In Turn",
//        "overdue": "Overdue",
//        "title-supply-req": "Supply Request",
//        "state": "State",
//        "waitting": "Waiting",
//        "accept": "Accept",
//        "reject": "Reject",
//        "req-info": "Request Info",
//        "code": "Code",
//        "creator": "Creator",
//        "date-create": "Create Time",
//        "issus-date": "Issue Date",
//        "dept": "Dept",
//        "type-supply": "Type Supply",
//        "staff_code": "Staff Code:",
//        "table-no": "No",
//        "table-code": "Code No",
//        "table-item-name": "Item Name",
//        "table-desc": "Description",
//        "table-vendor": "Vendor",
//        "table-unit-price": "Unit Price",
//        "table-qty": "Qty",
//        "table-unit": "Unit",
//        "table-unit-detail": "(kg•pcs)",
//        "table-amount": "Amount",
//        "table-amount-detail": "(USD or VND)",
//        "table-owner-of-item": "Owner of Item",
//        "table-owner-of-item-detail": "U→UMC, C→Customer",
//        "table-cost-center": "Cost Center",
//        "table-cost-center-detail": "AI/SMT/FAT/ Other/Office",
//        "table-account": "Account",
//        "table-asset-no": "Asset No",
//        "table-total": "Total",
//        "more-info": "**More Infomation**",
//        "req-delivery-date": "- Request Delivery Date: ",
//        "note": "- Remark: ",
//        "round-robin": "- Round robin: ",
//        "yes": "YES",
//        "no": "NO",
//        "req-checkout": "Có yêu cầu khách hàng thanh toán : Có / Không",
//        "attach": "**File Attach**",
//        "main-action": "Main Action",
//        "apply": "1. Applicant",
//        "dept-manager": "2. Dept Manager",
//        "asset-center": "3. Asset Center",
//        "factory-manager": "4. Factory Manager",
//        "general-director": "5. General Director",
//        "purchasing-dept": "6. Purchasing Dept",
//        "purchasing-dept-detail": "Approved by",
//        "Receive_check": "7. Recieve Check",
//        "applicant": "Applicant",
//        "asset-center-recive-check": "Asset Center",
//        "comment": "Comment",
//        "send": "Send",
//        "name": "Name",
//        "add-row": "Add row",
//        "cancel": "Cancel",
//        "create": "Create",
//        "return": "Back to",
//        "type_form": "Choose Type Form",
//        "save_change": "Save changes",
//        'title': "UMC FORM REQUEST",
//        'select_dept': "Select Dept",
//        'reconfirm': "Reconfirm"
//    },
//    "vi": {
//        "Home": "TRANG CHỦ",
//        "Summary": "Tổng kết",
//        "Search-for": "Tìm kiếm...",
//        "Username": "Tên đăng nhập",
//        "create-request": "Tạo Đề Xuất Mới",
//        "send-to-me": "Gửi Cho Tôi",
//        "my-request": "Tôi Gửi Đi",
//        "cancel-request": "Bị Từ Chối",
//        "finish-request": "Đã Hoàn Thành",
//        "following": "Đang Theo Dõi",
//        "list-request": "DANH SÁCH ĐỀ XUẤT",
//        "all": "Tất Cả",
//        "in-turn": "Đến Lượt",
//        "overdue": "Quá Hạn",
//        "title-supply-req": "Yêu Cầu Cung Cấp",
//        "state": "Trạng thái",
//        "waitting": "Đang chờ duyệt",
//        "accept": "Chấp thuận",
//        "reject": "Từ chối",
//        "req-info": "Thông Tin Đề Xuất",
//        "code": "Mã Đề Xuất: ",
//        "creator": "Người Tạo",
//        "date-create": "Thời Gian Tạo",
//        "issus-date": "Ngày Phát Hành",
//        "dept": "Bộ Phận",
//        "type-supply": "Loại Yêu Cầu",
//        "staff_code": "Mã Nhân Viên:",
//        "table-no": "STT",
//        "table-code": "Mã Số",
//        "table-item-name": "Tên Hàng",
//        "table-desc": "Quy Cách",
//        "table-vendor": "Nhà Cung Cấp",
//        "table-unit-price": "Đơn Giá",
//        "table-qty": "Số Lượng",
//        "table-unit": "Đơn Vị ",
//        "table-unit-detail": "(kg*pcs)",
//        "table-amount": "Số Tiền",
//        "table-amount-detail": "(USD or VND)",
//        "table-owner-of-item": "Owner of Item",
//        "table-owner-of-item-detail": "U→UMC, C→Customer",
//        "table-cost-center": "Cost Center",
//        "table-cost-center-detail": "AI/SMT/FAT/ Other/Office",
//        "table-account": "Tài Khoản",
//        "table-asset-no": "Số Tài Sản",
//        "table-total": "Tổng Số",
//        "more-info": "**Thông Tin Thêm**",
//        "req-delivery-date": "- Ngày yêu cầu giao hàng: ",
//        "note": "- Ghi chú: ",
//        "round-robin": "- Round robin: ",
//        "yes": "Có",
//        "no": "Không",
//        "req-checkout": "Có yêu cầu khách hàng thanh toán : Có/ Không",
//        "attach": "**File đính kèm**",
//        "main-action": "hoạt động chính",
//        "apply": "1. Người Đề Xuất",
//        "dept-manager": "2. Trưởng Phòng",
//        "asset-center": "3. Qly Tài Sản",
//        "factory-manager": "4. Giám Đốc Xưởng",
//        "general-director": "5. Tổng Giám Đốc",
//        "purchasing-dept": "6. Phòng Mua Hàng",
//        "purchasing-dept-detail": "Người Duyệt",
//        "Receive_check": "7. Recieve Check",
//        "applicant": "Applicant",
//        "asset-center-recive-check": "Asset Center",
//        "c": "Thảo Luận",
//        "send": "Gửi",
//        "name": "Họ Tên",
//        "add-row": "Thêm dòng",
//        "cancel": "Hủy",
//        "create": "Đề xuất",
//        "return": "Quay lại bước",
//        "type_form": "Chọn Mẫu Form",
//        "save_change": "Lưu thay đổi",
//        'title': "UMC FORM REQUEST",
//        'select_dept': 'Chọn Phòng Ban',
//        'reconfirm': "Xác nhận lại"
//    },
//    "ja": {

//    }
//};
