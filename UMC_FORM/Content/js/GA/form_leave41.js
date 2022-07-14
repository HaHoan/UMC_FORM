


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
function deleteRow(e) {
    $(e).parent().parent().remove();
    var index = 1;
    $('#tableInfo tr').each(function (rowIndex) {
        var stt = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(0)').text();
        var column = $('.table-border-dark tr #registration_date').attr('colspan');
        var colCount = parseInt($(".table-border-dark tr th").length - 3);
        if (stt) {
            
                $('#tableInfo tr:eq(' + rowIndex + ') td:eq(0)').text(index);
                changeIdWhenDelete(rowIndex, index, 'CODE', 1)
                changeIdWhenDelete(rowIndex, index, 'FULLNAME', 2)
                changeIdWhenDelete(rowIndex, index, 'CUSTOMER', 3)
                for (var j = 1; j <= column; j++) {                    
                    changeIdRegistrationdate_WhenDelete(rowIndex, index,j , 'REGISTRATION_DATE', 3 + j);                 
                }
                changeIdWhenDelete(rowIndex, index, 'REASON', colCount)
                changeId_TdreasonWhenDelete(rowIndex, index, 'TD_REASON', colCount)
                index++;           
        }
    });
    updateNumberRegister()
}
function changeIdWhenDelete(rowIndex, index, name, indexInTd) {
    try {
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('id', name + index)
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('name', name + index)
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') textarea').attr('id', name + index )
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') textarea').attr('name', name + index)
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') span').attr('id', name + index + "_ERROR" )
   
    } catch (e) {
    }
}
function changeIdRegistrationdate_WhenDelete(rowIndex, index, column, name, indexInTd) {
    try {
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('id', name + index + '_' + column)
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('name', name + index + '_' + column)
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') span').attr('id', name + index + "_ERROR" + '_' + column)
    } catch (e) {
    }
}
function changeId_TdreasonWhenDelete(rowIndex, index, name, indexInTd) {
    try {
        $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ')').attr('class', name + index)

    } catch (e) {
    }
}
function Delete_col(index_column) {
    var column_registration_date = $('.table-border-dark tr #registration_date').attr('colspan');
    list_checkbox = []
    for (var j = 1; j <= $('#tableInfo tr').length; j++) {
        for (var i = 1; i <= column_registration_date; i++) {
            var value_checkbox = $('#REGISTRATION_DATE' + j + '_' + i).is(":checked")
            if (value_checkbox == true) {
                list_checkbox.push('REGISTRATION_DATE' + j + '_' + i);
            }
            
        }
    }
    $('.table-border-dark').delegate('#Add_col' + index_column, 'click', function () {
        var index = this.cellIndex;
        $(this).closest('.table-border-dark ').find('.row-info').each(function () {
            this.removeChild(this.cells[index + 4]);
        });
        $('#Add_col' + index_column).remove();
       
    });
    if (index_column < column_registration_date) {
        for (var i = 0; i < column_registration_date - index_column; i++) {
            
            var value_next_id = index_column + i;
            var get_valueidnext = value_next_id + 1;
            $('.table-border-dark #columns #Add_col' + get_valueidnext + ' #TIME_LEAVE' + get_valueidnext).attr('id', "TIME_LEAVE" + value_next_id);
            $('.table-border-dark #columns #Add_col' + get_valueidnext).attr('id', "Add_col" + value_next_id);
            $('#DELETE' + get_valueidnext).remove();
            Change_buttondelete_col("DELETE",value_next_id);
        }
    } 
    var count_columns = column_registration_date - 1;
    var index = 1;
    $('#registration_date').attr('colspan', count_columns);
    if (count_columns >= 1) {
        $('#tableInfo tr').each(function (rowIndex) {
            var stt = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(0)').text();
            if (stt) {
                for (var i = 1; i <= column_registration_date; i++) {
                    var value = 3 + i;
                    if (i < index_column) {
                        Change_ThAdd_Whendeletecol(rowIndex, index, i, 'REGISTRATION_DATE', value, list_checkbox)
                    }
                    else if (i > index_column) {

                        Change_ThAdd_Whendeletecol(rowIndex, index, i - 1, 'REGISTRATION_DATE', value, list_checkbox)
                    }
                }
                index++;
            }
        });
    }
}
function Change_buttondelete_col(name, index) {
    var input_deletecol = $('<input />', {
        type: 'button',
        id: name + index,
        click: function () {
            Delete_col(index)
        },
        value: 'x',
        style: 'width:10px;height:22px;float:right;padding-left: 3px;border: 0px;background-color: white;'
    });
    $('.table-border-dark #columns #Add_col' + index).append(input_deletecol);
  
}
function Change_ThAdd_Whendeletecol(rowIndex, index, column, name, indexInTd,list_checkbox) {
    try {
        var id = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('id')
        if (list_checkbox.length <= 0) {
            $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('id', name + index + '_' + column)
            $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('name', name + index + '_' + column)
        }
        else {
            if (jQuery.inArray(id, list_checkbox) !== -1) {
                $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('id', name + index + '_' + column)
                $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('name', name + index + '_' + column)
                $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('checked', true)
            }
            else {
                $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('id', name + index + '_' + column)
                $('#tableInfo tr:eq(' + rowIndex + ') td:eq(' + indexInTd + ') input').attr('name', name + index + '_' + column)
            }
        }
    }
    catch (e) {
    }
}
function updateleaveItems() {
    leaveItems = []
    var checkTime = 0;
    var duplicate_list=[]
    var count_col = $('th', $('.table-border-dark ').find('#columns')).length;
    for (var i = 1; i <= count_col; i++) {
        var date_resgistration = $("#TIME_LEAVE" + i).val()      
            duplicate_list.push(date_resgistration);
    };
    var duplicate_listarray = duplicate_list.sort();
    var reportRecipientsDuplicate = [];
    for (var i = 0; i < duplicate_listarray.length - 1; i++) {
        if (duplicate_listarray[i + 1] == duplicate_listarray[i]) {
            reportRecipientsDuplicate.push(duplicate_listarray[i]);
        }
    }
    var count = reportRecipientsDuplicate.length;
    if (count > 0) {
        $("#DATE_ERROR").text("Ngày đăng ký 70% không được trùng")
        checkTime = 1;
    }
    else {
        $('#tableInfo .row-info').each(function (rowIndex) {
            index = rowIndex + 1

            var code = $('#CODE' + index).val()
            var fullname = $('#FULLNAME' + index).val()
            if (code == '' && fullname != '') {
                $('#CODE' + index + '_ERROR').text('Mã số không được để trống')
                checkTime = 1
            }

            if (fullname == "" && code != '') {
                $('#FULLNAME' + index + '_ERROR').text('Tên không được để trống')
                checkTime = 1
            }

            var customer = $('#CUSTOMER' + index).val()
            if (customer == '' && code != '') {
                $('#CUSTOMER' + index + '_ERROR').text('Bộ phận/ Khách hàng không được để trống')
                checkTime = 1
            }
            var reason = $('#REASON' + index).val()
            if (reason == '' && code != '') {
                $('#REASON' + index + '_ERROR').text('Lý do nghỉ không được để trống')
                checkTime = 1
            }
            var list_timeleave = []

            for (var i = 1; i <= count_col; i++) {
                var value_checkbox = $('#REGISTRATION_DATE' + index + '_' + i).is(":checked")
                if (value_checkbox == true) {
                    var date_resgistration = $("#TIME_LEAVE" + i).val()
                    var detailObj = {
                        TIME_LEAVE: date_resgistration
                    }
                    list_timeleave.push(detailObj)
                }
            }

            if (code != '') {

                var obj = {
                    NO: index,
                    FULLNAME: fullname,
                    CODE: code,
                    TOTAL: count_col,
                    REASON: reason,
                    CUSTOMER: customer,
                    GA_LEAVE_FORM_ITEM_DETAILs: list_timeleave
                }
                leaveItems.push(obj)
            }

        });
        if (leaveItems.length == 0 && $('.row-info').length > 0) {
            $('#CODE' + 1 + '_ERROR').text('Mã số không được để trống')
            checkTime = 1
        }

        $('#leaveItems').val(JSON.stringify(leaveItems))
    }
    
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
function addTdCheckbox(name) {
    var col = $('<td/>');
    var input = $('<input/>', {
        type: 'checkbox',
        class: 'form-input type_number inputDefault',
    })
    col.append(input);
    input.attr('id', name)
    input.attr('name', name)
    //input.prop("checked", true);
    var span = $('<span />', {
        class: 'error'
    })
    col.append(span)
    span.attr('id', name + '_ERROR');
    return col;
}
function addTdReason(name,row) {
    var col = $('<td class="TD_REASON'+row +'"/>');
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

const FORMAT_DATE_TIME = 'd/m/Y'
const FORMAT_DATE = 'd/m/Y'
$(function () {
    var count_col = $('th', $('.table-border-dark ').find('#columns')).length;
    var name = $('#TIME_LEAVE' + count_col).attr("name");
    if (name != "TIME_LEAVE") {
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
    }
    else {
        for (var i = 1; i <= count_col; i++) {
            $('#TIME_LEAVE' + i).datetimepicker({
                format: FORMAT_DATE,

            });
        }
        
    }
    for (var i = 1; i <= $('#tableInfo tr').length; i++) {
        $('#CODE' + i).keypress(function () {
            updateNumberRegister()
        })
        
        eraseTextError('CODE' + i)
        eraseTextError('FULLNAME' + i)
        eraseTextError('CUSTOMER' + i)
        eraseTextError('REASON' + i)
    }

    $('#DATE_REGISTER_VIEW').datetimepicker({
        format: FORMAT_DATE,
        formatDate: FORMAT_DATE,
        value: moment().format(),
        onChangeDateTime() {
            $('#DATE_REGISTER').val(convertDateToValid($("#DATE_REGISTER_VIEW").val()))
        }
    });
    $('#DATE_REGISTER').val(convertDateToValid($("#DATE_REGISTER_VIEW").val()));
    $('#select_dept_manager').hide();
    $('#GROUP_LEADER').on('change',function () {
        var groupLeader = this.value;
        var userCode = $('#user_code').val();
        if (groupLeader == userCode) {
            $('#select_dept_manager').show()
        } else {
            $('#select_dept_manager').hide()
        }
    });
    //đặt giá trị colspan cho ngày đăng ký nghỉ 70%
   
    var col_register = $("#ColSpan_table").val();
    if (col_register == null) {
        $('.table-border-dark tr #registration_date').attr('colspan', 1);
    }
    else {
        $('.table-border-dark tr #registration_date').attr('colspan', col_register);
    }

    $('#frmpaidleave_accept').click(function (e) {
        $('#status').val("accept")
    })
    $('#frmpaidleave_reject').click(function (e) {
        $('#status').val("reject")
    })
    $('#frmpaidleave_delete').click(function (e) {
        $('#status').val("delete")
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
    $
    $("#contextMenu li #add_row").click(function (e) {
        addRow();
    });
    $("#contextMenu li #add_column").click(function (e) {
        addColumn();
        
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
        var width = 0
        var height = 0;
        if ($('#layoutSidenav_nav').length) {
            width = $('#layoutSidenav_nav').width()
        }
        if ($('.sb-topnav').length) {
            height = $('.sb-topnav').height()
        }
        $contextMenu.css({
            display: "block",
            left: e.pageX - width,
            top: e.pageY - height
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
        
        submitHandler: function (form) {
            var status = $('#status').val();
            if (status == 'reject' || status == 'delete') {
                if (confirm('Do you want to ' + status + '?')) {
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
function generateTable(e) {
    $(e).html(
        '<i class="fa fa-circle-o-notch fa-spin"></i> loading...'
    );

    var data = $('textarea[name=excel_data]').val();
    var rows = data.split("\n");
    var firstIndexToAdd = 0
    $('.row-info').each(function (rowIndex) {
        var cols = $(this).find('td')
        var fullname = ''
        var code = ''
        cols.each(function (index, value) {
            if (index == 1) {
                var input = $(this).find('input')
                fullname = input.val()
            } else if (index == 2) {
                var input = $(this).find('input')
                code = input.val()
            }
        })
        rowIndex++

        if (fullname == '' && code == '') {
            firstIndexToAdd = rowIndex
            return false
        }
    });
    if (firstIndexToAdd == 0) {
        firstIndexToAdd = $('.row-info').length + 1
    }

    var totalRow = firstIndexToAdd + rows.length - 1
    var y = 0;
    var cells = rows[y].split("\t");
    var value_columndate = cells.length - 1 - 4;
    if (cells.length > 6) {       
        for (var i = 2; i <= value_columndate; i++) {
            addColumn_excel(i)
        }
    }
    for (var index = firstIndexToAdd; index <= totalRow; index++) {
        var cells = rows[y].split("\t");
        
        if ($('.row-info').length < index) {
            addRow()
        }
      
        if (cells.length > 6) {
            
            if (cells.length > 1) {
                $('#CODE' + index).val(cells[1])
            }
            if (cells.length > 2) {
                $('#FULLNAME' + index).val(cells[2])
            }
            if (cells.length > 3) {
                $('#CUSTOMER' + index).val(cells[3])
            }
            for (var i = 4; i < cells.length - 1; i++) {
                var j = i - 3;
                    if (cells[i] == "v") {
                        $('#REGISTRATION_DATE' + index +'_'+ j).prop('checked', true)
                    }
            }            
            var number = cells.length - 1;
            if (cells.length > number) {
                $('#REASON' + index).val(cells[number])
            }
        } else {
            if (cells.length > 1) {
                $('#CODE' + index).val(cells[1])
            }
            if (cells.length > 2) {
                $('#FULLNAME' + index).val(cells[2])
            }
            if (cells.length > 3) {
                $('#CUSTOMER' + index).val(cells[3])
            }
            if (cells.length > 4) {
                $('#REGISTRATION_DATE' + index + '_1').prop('checked', true)
            }
            if (cells.length > 5) {
                $('#REASON' + index).val(cells[5])
            }
        }
        

        y++
    }
    updateNumberRegister()

    $(e).html(
        'Paste dữ liệu vào bảng bên dưới'
    );
}
function addColumn() {
    var column = $('.table-border-dark tr #registration_date').attr('colspan');
    var index_column = parseInt(column) + 1;
    var index_row = $('#tableInfo tr').length;
    $('#registration_date').attr('colspan', index_column);
    var col = $('<th id=' + 'Add_col' + index_column + '  style= "width:120px;"/> ');
    var input = $('<input/>', {
        class: 'form-input inputDefault',
        style: 'width:80px;'
    });

    var input_deletecol = $('<input />', {
        type: 'button',
        id: 'DELETE' + index_column,
        click: function () {
            Delete_col(index_column)
        },
        value: 'x',
        style: 'width:10px;height:22px;float:right;padding-left: 3px;border: 0px;background-color: white;'
    });

    $('.table-border-dark #columns').append(col);
    col.append(input, input_deletecol);

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
}
function addTdCheckbox_addcol(name, index_col, index_row) {

    for (var i = 1; i <= index_row; i++) {
        var col = $('<td /> ');
        var input = $('<input/>', {
            type: 'checkbox',
            class: 'form-input type_number inputDefault',
        })
        col.append(input);
        input.attr('id', name + i + '_' + index_col)
        input.attr('name', name + i + '_' + index_col)
        //input.prop("checked", true);
        $('.TD_REASON' + i).before(col)
    }
}
function addColumn_excel(value) {
    var index_row = $('#tableInfo tr').length;
    $('#registration_date').attr('colspan', value);

    var col = $('<th id=' + 'Add_col' + value + '  style= "width:120px;"/> ');
        var input = $('<input/>', {
            class: 'form-input inputDefault',
            style: 'width:80px;'
        });

        var input_deletecol = $('<input />', {
            type: 'button',
            id: 'DELETE' + value,
            click: function () {
                Delete_col(value)
            },
            value: 'x',
            style: 'width:10px;height:22px;float:right;padding-left: 3px;border: 0px;background-color: white;'
        });

        $('.table-border-dark #columns').append(col);
        col.append(input, input_deletecol);

    input.attr('id', 'TIME_LEAVE' + value);
    input.attr('name', 'TIME_LEAVE' + value);
        input.datetimepicker({
            format: FORMAT_DATE_TIME,
            formatDate: FORMAT_DATE_TIME,
        });
    addTdCheckbox_addcolexcel("REGISTRATION_DATE", value, index_row)
    $('#TIME_LEAVE' + value).datetimepicker({
            format: FORMAT_DATE,
            formatDate: FORMAT_DATE,
            value: moment().format(),
            onChangeDateTime() {
                var date_col = $("#TIME_LEAVE" + value).val()
                $('#TIME_LEAVE' + value).val(date_col)
            }
        });
    var date_col = $("#TIME_LEAVE" + value).val()
    $('#TIME_LEAVE' + value).val(date_col)
    
}
function addTdCheckbox_addcolexcel(name, index_col, index_row) {

    for (var i = 1; i <= index_row; i++) {
        var col = $('<td /> ');
        var input = $('<input/>', {
            type: 'checkbox',
            class: 'form-input type_number inputDefault',
        })
        col.append(input);
        input.attr('id', name + i + '_' + index_col)
        input.attr('name', name + i + '_' + index_col)
        $('.TD_REASON' + i).before(col)
    }
}
function addRow() {
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
    row.append(addTd("CUSTOMER" + index));
    row.append(addTdCheckbox("REGISTRATION_DATE" + index + '_' + 1));
    for (var i = 8; i <= colCount; i++) {
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
}
function updateNumberRegister() {
    var total = 0
    $('.row-info').each(function (rowIndex) {
        rowIndex++
        var fullname = $('#FULLNAME' + rowIndex).val()
        var code = $('#CODE' + rowIndex).val()
        if (fullname != '' && code != '') {
            total++
        }
    });
    $('#NUMBER_REGISTER').text(total)
}

