/*!
    * Start Bootstrap - SB Admin v6.0.3 (https://startbootstrap.com/template/sb-admin)
    * Copyright 2013-2021 Start Bootstrap
    * Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-sb-admin/blob/master/LICENSE)
    */
(function ($) {
    "use strict";
    $('.modal').removeClass('d-block')
    // Add active state to sidbar nav links
    var path = window.location.href; // because the 'href' property of the DOM element is the absolute path
    $("#layoutSidenav_nav .sb-sidenav a.nav-link").each(function () {
        if (this.href === path) {
            $(this).addClass("active");
        }
    });

    // Toggle the side navigation
    $("#sidebarToggle").on("click", function (e) {
        e.preventDefault();
        $("body").toggleClass("sb-sidenav-toggled");
    });


    $("#searchRequestForm").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#listRequestForm li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $("#accept").on("click", function (e) {
        $('.loading').show()
        e.preventDefault();
        var base_url = window.location.origin;
        var ticket = $("#ticketNo").text();
        var usePur = $('#usePur').is(":checked");
        var list = [];
        $('#tableInfo tr').each(function (rowIndex) {
            var costCenter = $("#COST_CENTER_" + (rowIndex + 1)).val();
            var assetType = $("input[name='AK_" + (rowIndex + 1) + "']:checked").val();
            var accountCode = $("#ACOUNT_CODE_" + (rowIndex + 1)).val();
            var assetNo = $("#ASSET_NO_" + (rowIndex + 1)).val();
            if (typeof assetType !== 'undefined') {

                var obj = {
                    assetIndex: rowIndex + 1,
                    costCenter: costCenter,
                    assetType: assetType,
                    accountCode: accountCode,
                    assetNo: assetNo
                }
                list.push(obj)
            }
        });
        var listJson = JSON.stringify(list);
        $.ajax({
            type: "POST",
            url: base_url + "/PurAccF06/Accept",
            data: { ticket: ticket, list: listJson, usePur: usePur },

            success: function (data) {
                window.location.href = base_url + '/Home?type=SENDTOME';
            },
            error: function (err) {
                $('.loading').hide()
                alert('error' + err.msg);
            }
        });
    });
    $('.loading').hide()
    $('#btnEdit').on('click', function (e) {
        $('.loading').show()
        $('#form_edit').submit()
    })
    $("#reject").on("click", function (e) {

        $('.loading').show()
        e.preventDefault();
        var base_url = window.location.origin;
        var ticket = $("#ticketNo").text();
        $.ajax({
            type: "POST",
            url: base_url + "/PurAccF06/Reject",
            data: {

                ticket: ticket
            },

            success: function (data) {
                //alert(data.msg);
                window.location.href = base_url + '/Home?type=SENDTOME';
            },
            error: function (err) {
                alert('ehihirror' + err.msg);
            }
        });
    });
    filter()
})(jQuery);
function filter() {
    var filter = $('#filter').val()
    $('#myTab').addClass('d-none')

    if (filter == 'SENDTOME') { // Gửi cho tôi
        $("#sendToMe").prop('checked', true)
        $("#myRequest").prop('checked', false)
        $("#myCancel").prop('checked', false)
        $("#myFinish").prop('checked', false)
        $("#myFollow").prop('checked', false)

        localStorage.setItem('filter', 'SENDTOME');
    } else if (filter == 'MYREQUEST') { //Tôi cần phê duyệt
        $("#sendToMe").prop('checked', false)
        $("#myRequest").prop('checked', true)
        $("#myCancel").prop('checked', false)
        $("#myFinish").prop('checked', false)
        $("#myFollow").prop('checked', false)
        localStorage.setItem('filter', 'MYREQUEST');
    } else if (filter == 'CANCEL') { //Bị từ chối
        $("#sendToMe").prop('checked', false)
        $("#myRequest").prop('checked', false)
        $("#myCancel").prop('checked', true)
        $("#myFinish").prop('checked', false)
        $("#myFollow").prop('checked', false)
        localStorage.setItem('filter', 'CANCEL');
        $(".status-ticket").addClass("badge-danger").removeClass("badge-success");
    }
    else if (filter == 'FINISH') { //Đã hoàn thành
        $("#sendToMe").prop('checked', false)
        $("#myRequest").prop('checked', false)
        $("#myCancel").prop('checked', false)
        $("#myFinish").prop('checked', true)
        $("#myFollow").prop('checked', false)

        $(".status-ticket").text("Finish");
        localStorage.setItem('filter', 'FINISH');
    } else if (filter == 'FOLLOW') { // Tôi cần hoàn thành
        $("#sendToMe").prop('checked', false)
        $("#myRequest").prop('checked', false)
        $("#myCancel").prop('checked', false)
        $("#myFinish").prop('checked', false)
        $("#myFollow").prop('checked', true)
        localStorage.setItem('filter', 'FOLLOW');
        $('#myTab').removeClass('d-none')

    }
    else {

    }
}
function searchByText() {
    var value = $('#searchRequestForm').val().toLowerCase();
    $("#listRequestForm li").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
}
