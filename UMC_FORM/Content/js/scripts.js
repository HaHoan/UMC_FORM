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
                    costCenter:costCenter,
                    assetType: assetType,
                    accountCode: accountCode,
                    assetNo: assetNo
                }
                list.push(obj)
            }
        });
        var listJson = JSON.stringify(list);
        $('.loadImg').addClass('d-block')
        $.ajax({
            type: "POST",
            url: base_url + "/PurAccF06/Accept",
            data: { ticket: ticket, list: listJson, usePur:usePur },

            success: function (data) {
                //alert(data.msg);
                window.location.href = base_url + '/Home?type=SENDTOME';
            },
            error: function (err) {
                alert('error' + err.msg);
            }
        });
    });
    $("#reject").on("click", function (e) {
        e.preventDefault();
        var base_url = window.location.origin;
        var ticket = $("#ticketNo").text();
        $.ajax({
            type: "POST",
            url: base_url + "/PurAccF06/Reject",
            data: { ticket: ticket },

            success: function (data) {
                //alert(data.msg);
                window.location.href = base_url + '/Home?type=SENDTOME';
            },
            error: function (err) {
                alert('error' + err.msg);
            }
        });
    });
})(jQuery);