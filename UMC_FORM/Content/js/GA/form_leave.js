
function enableButton() {
    $('#frmpaidleave_create').prop("disabled", false);
    $('#frmpaidleave_accept').prop("disabled", false);
    $('#frmpaidleave_reject').prop("disabled", false);
    $('#frmpaidleave_create').html("Create");
    $('#frmpaidleave_accept').html("Accept")
    $('#frmpaidleave_reject').html("Reject")
}
function disableButtonWhenSubmit(btn) {
    $('#frmpaidleave_create').prop("disabled", true);
    $('#frmpaidleave_accept').prop("disabled", true);
    $('#frmpaidleave_reject').prop("disabled", true);
    $(btn).html(
        '<i class="fa fa-circle-o-notch fa-spin"></i> loading...'
    );

}

function updateSTT() {
    var index = 1;
    $('#tableInfo tr').each(function (rowIndex) {
        var stt = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(0)').text();
        if (stt) {
            $('#tableInfo tr:eq(' + rowIndex + ') td:eq(0)').text(index);
            index++;
        }

    });
}
function deleteRow(e) {
    $(e).parent().parent().remove();
    updateSTT();
}

function updateleaveItems() {
    try {
        leaveItems = []
        $('.row-info').each(function (rowIndex) {
            index = rowIndex + 1
            var item = $('#FULLNAME' + index).val()
            if (item == "") return true
            var code = $('#CODE' + index).val()
            var time_from = $('#TIME_FROM' + index).val()
            var time_to = $('#TIME_TO' + index).val()
            var total = $('#TOTAL' + index).val()
            if (time_to < time_from) {
                alert('Ngày kết thúc nghỉ phải lớn hơn ngày bắt đầu nghỉ!');
                return false;
            }
            if (time_to == null || time_from == null) {
                return false;
            } 
            if (total < 0) {
                alert('Tổng ngày nghỉ không được nhỏ hơn 0! ');
                return false;
            }
            var reason = $('#REASON' + index).val()
            var speacial_leave = $('#SPEACIAL_LEAVE' + index).is(":checked")
            var remark = $('#REMARK' + index).val()
            var obj = {
                NO: index,
                FULLNAME: item,
                CODE: code,
                TIME_FROM: time_from.trim(),
                TIME_TO: time_to.trim(),
                TOTAL: total == "" ? 0 : total,
                REASON: reason,
                SPEACIAL_LEAVE: speacial_leave,
                REMARK: remark
            }
            leaveItems.push(obj)
        });
        $('#leaveItems').val(JSON.stringify(leaveItems))
    }
    catch (e) {
        alert(e)
    }
       
}
function addTd(name) {
    var col2 = $('<td/>');
    var input2 = $('<input/>', {
        class: 'form-input'
    })
    col2.append(input2);
    input2.attr('id', name)
    input2.attr('name', name)
    return col2;
}
function addTdTime(name){
    var col2 = $('<td/>');
    var input1 = $('<input/>', {
        class: 'form-input inputDefault',
       
    })
    col2.append(input1);
    input1.attr('id', name);
    input1.attr('name', name);
    input1.datetimepicker({
        rtl: false,
        format: 'd/m/Y H:i',

    });
    return col2; 
}
function OnSuccess(response) {
    if (response.result == 'success') {
        window.location.href = $("#RedirectTo").val()
    } else if (response.result == 'wait') {
        if (confirm('Ticket vừa có người thay đổi,Nếu muốn tiếp tục thì hãy nhấn OK để load lại ticket để cập nhật?')) {
            window.location.href = $("#LoadTicket").val()
        } else {
            window.location.href = $("#RedirectTo").val()
        }
    }
    else {
        if (response.message != null) {
            alert(response.message)
        }
        else alert('error')
        enableButton()
    }
}
function OnFailure(response) {
    alert("Kiểm tra lại dữ liệu nhập có kí tự đặc biệt không?" + "Detail:" + response.responseText)
    enableButton()
}
$(function () {
    
    for (var i = 1; i <= $('#tableInfo tr').length; i++) {
        $('#TIME_FROM' + i).datetimepicker({
            rtl: false,
            format: 'd/m/Y H:i',

        });
        $('#TIME_TO' + i).datetimepicker({
            rtl: false,
            format: 'd/m/Y H:i',

        });     
    }
    $('#DATE_REGISTER').datetimepicker({
        format: 'd/m/Y',
    });
    $('#frmpaidleave_accept').click(function (e) {
        $('#status').val("accept")
    })
    $('#frmpaidleave_reject').click(function (e) {
        $('#status').val("reject")
    })

    $(".type_number").keypress(function (e) {
        return onlyNumber(e)
    });

    $('#backTo').click(function () {
        window.location.href = $("#RedirectTo").val()
    })
    $('html').click(function () {
        $contextMenu.hide();
    });

    $("#contextMenu li a").click(function (e) {
        var index = $('#tableInfo tr').length + 1;
        var row = $('<tr/>');
        var col1 = $('<td/>', {
            text: (index)
        })
        row.append(col1);
        row.append(addTd("FULLNAME" + index));
        row.append(addTd("CODE" + index));
        row.append(addTdTime("TIME_FROM" + index));
        row.append(addTdTime("TIME_TO" + index));
       
        row.append(addTd("TOTAL" + index));

        var col3 = $("<td/>");
        var input3 = $('<textarea/>', {
            class: 'form-input  inputDefault',
            name: "REASON" + index,
        });
        col3.append(input3);
        row.append(col3);

        var pathname_local = "/GAFormLeave";
        var host_local = window.location.pathname
        if (host_local.includes(pathname_local)) {
            var col4 = $("<td/>", {
                class: 'text-center p-2'
            });
            var input4 = $('<input/>', {
                type: 'checkbox',
                name: "SPEACIAL_LEAVE" + index,
                checked: false
            });
            col4.append(input4);
            row.append(col4);
        }
        row.append(addTd('REMARK' + index));

        var rowDelete = $('<td/>');
        var btnXoa = $('<button/>', {
            text: 'Xóa',
            type: 'button',
            class: 'btnXoa btn btn-danger',
            click: function () {
                deleteRow(this)
            }
        });
        rowDelete.append(btnXoa);
        row.append(rowDelete);
        $('#tableInfo').append(row);
    });
    $(".btnXoa").on('click', function () {
        deleteRow(this);
    })
    $('#frmpaidleave_accept').click(function (e) {
        $('#status').val("accept")
    })
    $('#frmpaidleave_reject').click(function (e) {
        $('#status').val("reject")
    }) 
    var $contextMenu = $("#contextMenu");
    $("body").on("contextmenu", "table tr", function (e) {
        $contextMenu.css({
            display: "block",
            left: e.pageX,
            top: e.pageY
        });
        return false;
    });
    $("#formCreate").validate({
        submitHandler: function (form) {
            disableButtonWhenSubmit('#frmpaidleave_create')
            updateleaveItems()
            form.ajax.submit()
        }
    });
    $("#submitForm").validate({
        submitHandler: function (form) {
            var status = $('#status').val();
            if (status == 'reject') {
                if (confirm('Do you want to reject?')) {
                    disableButtonWhenSubmit('#frmpaidleave_' + status)
                    updateleaveItems()         
                    form.ajax.submit()
                } else {
                    return false;
                }
            }
            else {
                disableButtonWhenSubmit('#frmpaidleave_' + status)
                updateleaveItems()
                form.ajax.submit()
            }
        }     
    });
})


