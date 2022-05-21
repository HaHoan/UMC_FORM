
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

        if (!CheckNullUndefined(fullname) && !CheckNullUndefined(time_from) && !CheckNullUndefined(time_to)) {
            var obj = {
                NO: index,
                FULLNAME: fullname,
                CODE: code,
                TIME_FROM: convertStringToCorrectFormat(time_from.trim()),
                TIME_TO: convertStringToCorrectFormat(time_to.trim()),
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
            updateNumberRegister()
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
function addTdNumber(name) {
    var col = $('<td/>');
    var input = $('<input/>', {
        class: 'form-input inputDefault type_number',
        keypress: function (e) {
            return onlyNumber(e)
        },
        change: function (e) {
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
function addTdTime(name) {
    var col = $('<td/>');
    var input = $('<input/>', {
        class: 'form-input inputDefault',

    })
    col.append(input);
    input.attr('id', name);
    input.attr('name', name);
    if (name.includes('FROM')) {
        input.datetimepicker({
            format: FORMAT_DATE,
            formatDate: FORMAT_DATE,
            onShow: function (ct, $input) {
                timeFromOnShow($input, this)
            },
            onChangeDateTime: function (currentTime, $input) {
                validateTime(getRowIndexFromId($input.attr('id')))
            }
        });
    } else if (name.includes('TO')) {
        input.datetimepicker({
            format: FORMAT_DATE,
            formatDate: FORMAT_DATE,
            onShow: function (ct, $input) {
                timeToOnShow($input, this)
            },
            onChangeDateTime: function (currentTime, $input) {
                validateTime(getRowIndexFromId($input.attr('id')))
            }

        });

    }
    input.change(function () {
        validateTime(getRowIndexFromId($(this).attr('id')))
    })

    var span = $('<span />', {
        class: 'error'
    })
    col.append(span)
    span.attr('id', name + '_ERROR');
    return col;
}
function addTdReason(name) {
    var col = $("<td/>");
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
function validateTime(index) {
    var time_from = $('#TIME_FROM' + index).val()
    var time_to = $('#TIME_TO' + index).val()
    if (!time_from || !time_to) return
    if (convertDateTimeToDateValid(time_from) > convertDateTimeToDateValid(time_to)) {
        $('#TIME_TO' + index + '_ERROR').text("Ngày kết thúc phải lớn hơn ngày bắt đầu")
    } else {
        $('#TIME_TO' + index + '_ERROR').text("")
    }
}
function timeFromOnShow($input, context) {
    var rowIndex = getRowIndexFromId($input.attr('id'))
    var time_to = $('#TIME_TO' + rowIndex).val();
    if (time_to != '') {
        context.setOptions({
            maxDate: time_to,
        })
    }
    $('#TIME_FROM' + rowIndex + '_ERROR').text("")
}
function timeToOnShow($input, context) {
    var rowIndex = getRowIndexFromId($input.attr('id'))
    var time_from = $('#TIME_FROM' + rowIndex).val();
    if (time_from != '') {
        context.setOptions({
            minDate: time_from,
        })
    }
    $('#TIME_TO' + rowIndex + '_ERROR').text("")
}
const FORMAT_DATE_TIME = 'd/m/Y H:i'
const FORMAT_DATE = 'd/m/Y'
$(function () {

    for (var i = 1; i <= $('#tableInfo tr').length; i++) {
        $('#CODE' + i).keypress(function () {
            updateNumberRegister()
        })
        $('#TIME_FROM' + i).datetimepicker({
            format: FORMAT_DATE,
            formatDate: FORMAT_DATE,
            onShow: function (ct, $input) {
                timeFromOnShow($input, this)
            },
            onChangeDateTime: function (currentTime, $input) {
                validateTime(getRowIndexFromId($input.attr('id')))
            }
        });
        $('#TIME_FORM' + i).change(function () {
            validateTime(getRowIndexFromId($(this).attr('id')))
        })
        $('#TIME_TO' + i).datetimepicker({
            format: FORMAT_DATE,
            formatDate: FORMAT_DATE,
            onShow: function (ct, $input) {
                timeToOnShow($input, this)
            },
            onChangeDateTime: function (currentTime, $input) {
                validateTime(getRowIndexFromId($input.attr('id')))
            }

        });
        $('#TIME_TO' + i).change(function () {
            validateTime(getRowIndexFromId($(this).attr('id')))
        })
        eraseTextError('FULLNAME' + i)
        eraseTextError('CODE' + i)
        eraseTextError('REASON' + i)
        eraseTextError('TOTAL' + i)
    }
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

    $("#contextMenu li a").click(function (e) {
        addRow();
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
    $.validator.addMethod("valueNotEquals", function (value, element, arg) {
        return arg !== value;
    }, "")

    $("#formCreate").validate({
        rules: {
            "GROUP_LEADER": {
                valueNotEquals: '0'
            }
        },
        submitHandler: function (form) {
            if (confirm('Do you want to create?')) {
                disableButtonWhenSubmit('#frmpaidleave_create')
                var result = updateleaveItems()
                if (result == 0) {
                    form.ajax.submit()
                } else {
                    enableButton()
                }
            } else {
                return false;
            }


        }
    });

    $("#submitForm").validate({
        rules: {
            "TICKET.DEPT_MANAGER": {
                valueNotEquals: '0'
            }
        },
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
                if (confirm('Do you want to ' + status + '?')) {
                    disableButtonWhenSubmit('#frmpaidleave_' + status)
                    var result = updateleaveItems()
                    if (result == 0) {
                        form.ajax.submit()
                    } else {
                        enableButton()
                    }
                } else {
                    return false;
                }

            }
        }
    });

})
function addRow() {
    var index = $('#tableInfo tr').length + 1;
    var row = $('<tr/>', {
        class: 'row-info'
    });

    var col = $('<td/>', {
        text: (index),
        class: 'text-center'
    })
    row.append(col);
    row.append(addTd("FULLNAME" + index));
    row.append(addTd("CODE" + index));
    row.append(addTdTime("TIME_FROM" + index));
    row.append(addTdTime("TIME_TO" + index));
    row.append(addTdNumber("TOTAL" + index));
    row.append(addTdReason("REASON" + index));
    var name_page = $('input[name="formName"]').val();
    if (name_page == "GA_35") {
        var col4 = $("<td/>", {
            class: 'text-center p-2'
        });
        var input4 = $('<input/>', {
            type: 'checkbox',
            id: "SPEACIAL_LEAVE" + index,
            name: "SPEACIAL_LEAVE" + index,
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
}

function generateTable(e) {
    $(e).html(
        '<i class="fa fa-circle-o-notch fa-spin"></i> loading...'
    );

    var data = $('textarea[name=excel_data]').val();
    var rows = data.split("\n");
    var firstIndexToAdd = 0
    $('.row-info').each(function (rowIndex) {
        rowIndex++
        var fullname = $('#FULLNAME' + rowIndex).val()
        var code = $('#CODE' + rowIndex).val()
        if (fullname == '' && code == '') {
            firstIndexToAdd = rowIndex
            return false
        }
    });
    var totalRow = firstIndexToAdd + rows.length - 1
    var y = 0;
    for (var index = firstIndexToAdd; index <= totalRow; index++) {

        var cells = rows[y].split("\t");
        if ($('.row-info').length < index) {
            addRow()
        }
        if (cells.length > 1) {
            $('#FULLNAME' + index).val(cells[1])
        }
        if (cells.length > 2) {
            $('#CODE' + index).val(cells[2])
        }
        if (cells.length > 3) {
            $('#TIME_FROM' + index).val(cells[3])
        }
        if (cells.length > 4) {
            $('#TIME_TO' + index).val(cells[4])
        }
        if (cells.length > 5) {
            $('#TOTAL' + index).val(cells[5])
        }
        if (cells.length > 6) {
            $('#REASON' + index).val(cells[6])
        }
        if (cells.length > 7) {
            if (cells[7] != '') {
                $('#REMARK' + index).val(cells[7])
            }
        }
        if (cells.length > 8) {
            if (cells[8] != '')
                $('#SPEACIAL_LEAVE' + index).prop('checked',true)
        }
        y++
    }
    updateNumberRegister()
    
    $(e).html(
        'Paste dữ liệu vào bảng bên dưới'
    );
}
function updateNumberRegister() {
    var total = 0
    $('.row-info').each(function (rowIndex) {
        rowIndex++
        var fullname = $('#FULLNAME' + rowIndex).val()
        var code = $('#CODE' + rowIndex).val()
        if (fullname != '' && code != '') {
            total ++ 
        }
    });
    $('#NUMBER_REGISTER').text(total)
}