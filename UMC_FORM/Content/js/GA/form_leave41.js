
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
    leaveItems = []
    var checkTime = 0;
    $('.row-info').each(function (rowIndex) {
        index = rowIndex + 1
        var fullname = $('#FULLNAME' + index).val()
        var code = $('#CODE' + index).val()
        if (fullname == "" && code != '') {
            $('#FULLNAME' + index + '_ERROR').text('Tên không được để trống')
            checkTime = 1
        }
        if (code == '' && fullname != '') {
            $('#CODE' + index + '_ERROR').text('Mã số không được để trống')
            checkTime = 1
        }
        var time_from = $('#TIME_FROM' + index).val()

        if (time_from == '' && code != '') {
            $('#TIME_FROM' + index + '_ERROR').text('Thời gian bắt đầu không được để trống')
            checkTime = 1
        }
        var time_to = $('#TIME_TO' + index).val()
        if (time_to == '' && code != '') {
            $('#TIME_TO' + index + '_ERROR').text('Thời gian bắt đầu không được để trống')
            checkTime = 1
        }
        var total = $('#TOTAL' + index).val()
        if (total <= 0 && code != '') {
            $('#TOTAL' + index + '_ERROR').text('Tổng số không được bằng 0')
            checkTime = 1
        }
        var reason = $('#REASON' + index).val()
        if (reason == '' && code != '') {
            $('#REASON' + index + '_ERROR').text('Lý do nghỉ không được để trống')
            checkTime = 1
        }
        var speacial_leave = $('#SPEACIAL_LEAVE' + index).is(":checked")
        var remark = $('#REMARK' + index).val()
        if (fullname != '') {
            var obj = {
                NO: index,
                FULLNAME: fullname,
                CODE: code,
                TIME_FROM: time_from.trim(),
                TIME_TO: time_to.trim(),
                TOTAL: total == "" ? 0 : total,
                REASON: reason,
                SPEACIAL_LEAVE: speacial_leave,
                REMARK: remark
            }
            leaveItems.push(obj)
        }

    });
    if (leaveItems.length == 0 && $('.row-info').length > 0) {
        $('#FULLNAME' + 1 + '_ERROR').text('Tên không được để trống')
        checkTime = 1
    }
    $('#leaveItems').val(JSON.stringify(leaveItems))
    return checkTime;
}
function addTd(name) {
    var col = $('<td/>');
    var input = $('<input/>', {
        class: 'form-input inputDefault',
        keypress: function () {
            $('#' + name + '_ERROR').text('')
        }
    })
    col.append(input);
    input.attr('id', name)
    input.attr('name', name)
    var span = $('<span />', {
        class: 'error'
    })
    col.append(span)
    span.attr('id', name + '_ERROR');
    return col;
}
function eraseTextError(name) {
    $('#' + name).change(function () {
        $('#' + name + '_ERROR').text('')
    })
}
function addTdCheckbox(name) {
    var col = $('<td/>');
    var input = $('<input/>', {
        type: 'checkbox',
        class:'form-input type_number inputDefault',
    })
    col.append(input);
    input.attr('id', name)
    input.attr('name', name)
    var span = $('<span />', {
        class: 'error'
    })
    col.append(span)
    span.attr('id', name + '_ERROR');
    return col;
}

function addTdReason(name) {
    var col = $('<td id="reason"/>');
    var input = $('<textarea/>', {
        class: 'form-input  inputDefault',
        name: name,
        id: name,
        keypress: function () {
            $('#' + name + '_ERROR').text('')
        }
    });
    col.append(input);
    var span = $('<span />', {
        class: 'error'
    })
    col.append(span)
    span.attr('id', name + '_ERROR');
    return col;
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

const FORMAT_DATE_TIME = 'd/m/Y H:i'
const FORMAT_DATE = 'd/m/Y'
$(function () {
    $('#DATE_REGISTION_VIEW').datetimepicker({
        format: FORMAT_DATE,
        formatDate: FORMAT_DATE,
        value: moment().format(),
        onChangeDateTime() {
            $('#DATE_REGISTION').val(convertDateToValid($("#DATE_REGISTION_VIEW").val()))
        }
    });
    $('#DATE_REGISTION').val(convertDateToValid($("#DATE_REGISTION_VIEW").val()))
   
    $('#DATE_REGISTER_VIEW').datetimepicker({
        format: FORMAT_DATE,
        formatDate: FORMAT_DATE,
        value: moment().format(),
        onChangeDateTime() {
            $('#DATE_REGISTER').val(convertDateToValid($("#DATE_REGISTER_VIEW").val()))
        }
    });
    $('#DATE_REGISTER').val(convertDateToValid($("#DATE_REGISTER_VIEW").val()))
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
    $('#registration_date').attr('colspan', 1);
    $("#contextMenu li #add_row").click(function (e) {
        var index = $('#tableInfo tr').length + 1;
        var colCount = $(".table-border-dark tr th").length-1;
        var row = $('<tr/>', {
            class: 'row-info'
        });
        var col = $('<td/>', {
            text: (index),
            class: 'text-center'
        })
        row.append(col);
        row.append(addTd("CODE" + index));
        row.append(addTd("FULLNAME" + index));
        row.append(addTd("DEPARTMENT_CUS" + index));
        row.append(addTdCheckbox("REGISTRATION_DATE" + index));
        for (var i = 7; i <= colCount; i++) {
            row.append(addTdCheckbox("REGISTRATION_DATE" + index));
        }
        row.append(addTdReason("REASON" + index));
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
    $("#contextMenu li #add_column").click(function (e) {
        var span = $('.table-border-dark tr #registration_date').attr('colspan');
        var index_column = parseInt(span) + 1;
        var index_row = $('#tableInfo tr').length + 1;
        $('#registration_date').attr('colspan', index_column);
        var col = $('<th/>');
        var input = $('<input/>', {
            class: 'form-input inputDefault',
        })     
        $('.table-border-dark #columns').append(col);
        col.append(input);
        input.attr('id', name);
        input.attr('name', name);
        input.datetimepicker({
            format: FORMAT_DATE_TIME,
            formatDate: FORMAT_DATE_TIME,
        });
        $("#tableInfo tr").find("#reason").before(addTdCheckbox("REGISTRATION_DATE" + index_row));      
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
            var result = updateleaveItems()
            if (result == 0) {
                form.ajax.submit()
            } else {
                enableButton()
            }

        }
    });
    $("#submitForm").validate({
        submitHandler: function (form) {
            var status = $('#status').val();
            if (status == 'reject') {
                if (confirm('Do you want to reject?')) {
                    disableButtonWhenSubmit('#frmpaidleave_' + status)
                    form.ajax.submit()
                } else {
                    return false;
                }
            }
            else {
                disableButtonWhenSubmit('#frmpaidleave_' + status)
                var result = updateleaveItems()
                if (result == 0) {
                    form.ajax.submit()
                } else {
                    enableButton()
                }
            }
        }
    });
})


