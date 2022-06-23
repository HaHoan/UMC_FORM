
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
    $('#tableInfo .row-info').each(function (rowIndex) {
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
        var customer = $('#CUSTOMER' + index).val()
        var reason = $('#REASON' + index).val()
        var count_col = $('th', $('.table-border-dark ').find('#columns')).length;
        var list_timeleave = []
        for (var i = 1; i <= count_col; i++) {
            var value_checkbox = $('#REGISTRATION_DATE' + index +'_'+ i).is(":checked")
            if (value_checkbox == true) {
                var date_resgistration = $("#TIME_LEAVE" + i).val()
                
                list_timeleave.push(date_resgistration)
            }

        }
        
        if (fullname != '') {
            var obj = {
                NO: index,
                FULLNAME: fullname,
                CODE: code,
                REASON: reason,
                CUSTOMER: customer,
           
                TIME_LEAVE: list_timeleave
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
        class: 'form-input type_number inputDefault',
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

function addTdReason(name,row) {
    var col = $('<td id="TD_REASON'+row +'"/>');
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

const FORMAT_DATE_TIME = 'd-M'
const FORMAT_DATE = 'd-M'
$(function () {
    var count_col = $('th', $('.table-border-dark ').find('#columns')).length;
    $('#TIME_LEAVE' + count_col).datetimepicker({
        format: FORMAT_DATE,
        value: moment().format(),
        onChangeDateTime() {
            var date = $("#TIME_LEAVE" + count_col).val()
            $('#TIME_LEAVE' + count_col).val(date)
        }
    });
    var date = $("#TIME_LEAVE" + count_col).val()
    $('#TIME_LEAVE' + count_col).val(date)

    $('#DATE_REGISTER_VIEW' + count_col).datetimepicker({
        format: FORMAT_DATE,
        formatDate: FORMAT_DATE,
        value: moment().format(),
        onChangeDateTime() {
            $('#DATE_REGISTER' + count_col).val(convertDateToValid($("#DATE_REGISTER_VIEW" + count_col).val()))
        }
    });
    $('#DATE_REGISTER' + count_col).val(convertDateToValid($("#DATE_REGISTER_VIEW" + count_col).val()))
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
        var column = $('.table-border-dark tr #registration_date').attr('colspan');
        var index = $('#tableInfo tr').length + 1;
        var colCount = $(".table-border-dark tr th").length - 1;
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
        row.append(addTdCheckbox("REGISTRATION_DATE" + index + '_' + span));
        for (var i = 7; i <= colCount; i++) {
            row.append(addTdCheckbox("REGISTRATION_DATE" + index + '_' + column));
        }
        row.append(addTdReason("REASON" + index, index));
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
        var column = $('.table-border-dark tr #registration_date').attr('colspan');
        var index_column = parseInt(column) + 1;
        var index_row = $('#tableInfo tr').length;
        $('#registration_date').attr('colspan', index_column);
        var col = $('<th/>');
        var input = $('<input/>', {
            class: 'form-input inputDefault',
        })
        $('.table-border-dark #columns').append(col);
        col.append(input);
        input.attr('id', 'TIME_LEAVE' + index_column);
        input.attr('name', 'TIME_LEAVE' + index_column);
        input.datetimepicker({
            format: FORMAT_DATE_TIME,
            formatDate: FORMAT_DATE_TIME,
        });
        addTdCheckbox_addcol("REGISTRATION_DATE", index_column, index_row)

        $('#TIME_LEAVE' + index_column).datetimepicker({
            format: FORMAT_DATE,
            formatDate: FORMAT_DATE,
            value: moment().format(),
            onChangeDateTime() {
                var date_col = $("#TIME_LEAVE" + index_column).val()
                $('#TIME_LEAVE' + index_column).val(date_col)
            }
        });
        var date_col = $("#TIME_LEAVE" + index_column).val()
        $('#TIME_LEAVE' + index_column).val(date_col)
    });
    function addTdCheckbox_addcol(name, index_col, index_row) {

        for (var i = 1; i <= index_row; i++) {
            var col = $('<td/>');
            var input = $('<input/>', {
                type: 'checkbox',
                class: 'form-input type_number inputDefault',
            })
            col.append(input);
            input.attr('id', name + i + '_' + index_col)
            input.attr('name', name + i + '_' + index_col)
            $('#TD_REASON' + i).before(col)

        }
    }
    $("#contextMenu li #delete_column").click(function (e) {
        $("#tableInfo").contextmenu({
            selector: 'td',
            callback: function (key, options) {
                var content = $(this).text();
                alert("You clicked on: " + content);
            },
        });
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


