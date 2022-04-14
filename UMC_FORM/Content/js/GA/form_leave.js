
function enableButton() {
    $('#frmpaidleave_create').prop("disabled", false);
    $('#frmpaidleave_create').html("Create")
}
function disableButtonWhenSubmit(btn) {
    $('#frmpaidleave_create').prop("disabled", true);
    $(btn).html(
        '<i class="fa fa-circle-o-notch fa-spin"></i> loading...'
    );
}
function updateEachRowAmount(rowIndex) {
    var amount = getTotalDay(rowIndex)
    $('#TOTAL' + (rowIndex + 1)).val(amount)
    $('#TOTAL' + (rowIndex + 1) + "_VIEW").text(addCommas(amount.toString()))
    $('#tableInfo tr:eq(' + rowIndex + ') td:eq(8) .form-input').val(addCommas(amount.toString()))
    $('.total').text(getTotalAmount())
}
function getTotalDay(rowIndex) {
    var total = 0;
    var startleave = Math.floor($('#TIME_FROM' + (rowIndex)).getTime());
    var finishleave = Math.floor($('#TIME_TO' + (rowIndex + 1)).getTime());
    if (typeof qty === "undefined") {
        qty = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(6)').text();
    }
    if (typeof startleave !== "undefined" && startleave && typeof finishleave != "undefined" && finishleave) {
        total += finishleave - startleave
    }
    if (isNaN(total)) return "0"
    else
        return total;
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
    $(".total").text(getTotalAmount())
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
            time_from.toLocaleString()
            var time_to = $('#TIME_TO' + index).val()
            var total = $('#TOTAL' + index).val()
            total = $('#TOTAL' + index).val().split(",").join("")
            var reason = $('#REASON' + index).val()
            var speacial_leave = $('#SPEACIAL_LEAVE' + index).is(":checked");
            var remark = $('#REMARK' + index).val()
            var obj = {
                NO: index,
                FULLNAME: item,
                CODE: code,
                TIME_FROM: time_from.toLocaleString(),
                TIME_TO: time_to.toLocaleString(),
                TOTAL: total == "" ? 0 : parseInt(total),           
                REASON: reason,
                SPEACIAL_LEAVE: speacial_leave,
                REMARK: remark
            }
            leaveItems.push(obj)
        });

        $('#leaveItems').val(JSON.stringify(leaveItems))
    } catch (e) {
        alert(e)
    }

}
function addTd(name) {
    var col2 = $('<td/>');
    var input2 = $('<input/>', {
        class: 'form-input',
        change: function () {
            $('.total').text(getTotalAmount())
        }
    })
    col2.append(input2);
    input2.attr('id', name)
    input2.attr('name', name)
    return col2;
}
function addTdstartleave(rowIndex) {
    var col2 = $('<td/>');
    var date = new Date();
     date.setMinutes(now.getMinutes() - date.getTimezoneOffset());
    var input1 = $('<input/>', {
        class: 'form-input inputDefault',
        type: 'datetime-local',
        value: date.toISOString().slice(0, 16),

    })

    col2.append(input1);
    input1.attr('id', 'TIME_FROM' + rowIndex);
    input1.attr('name', 'TIME_FROM' + rowIndex);
    return col2;
}
function addTdFinishleave(rowIndex) {
    var col2 = $('<td/>');
    var date = new Date();
    date.setMinutes(now.getMinutes() - date.getTimezoneOffset());
    var input1 = $('<input/>', {
        class: 'form-input inputDefault',
        type: 'datetime-local',
        value: date.toISOString().slice(0, 16),
    })

    col2.append(input1);
    input1.attr('id', 'FINISH_LEAVE' + rowIndex);
    input1.attr('name', 'FINISH_LEAVE' + rowIndex);
    return col2;
}
function addTdtotalday(rowIndex) {
    var col2 = $('<td/>');
    var input1 = $('<input/>', {
        class: 'form-input inputDefault',
        change: function () {
            var id = $(this).attr('id')
            var rStr = id.substr(4, id.length - 4)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        }
    })

    col2.append(input1);
    input1.attr('id', 'TOTAL_DAY' + rowIndex);
    input1.attr('name', 'TOTAL_DAY' + rowIndex);
    input1.prop('readonly', true);
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
    var formType = document.getElementById("formCreate");
    // Create Form
    if (formType != null) {

    }
    else { // Detail Form


    }
    for (var i = 1; i <= $('#tableInfo tr').length; i++) {
        $('#TIME_TO' + i).change(function (e) {
            var id = $(this).attr('id')
            var rStr = id.substr(4, id.length - 4)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        })
        $('#TIME_FROM' + i).change(function () {
            var id = $(this).attr('id')
            var rStr = id.substr(11, id.length - 11)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        });

    }

    
    $(".type_number").keypress(function (e) {
        return onlyNumber(e)
    });


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
        row.append(addTdstartleave(index));
        row.append(addTdFinishleave(index));
        row.append(addTdtotalday(index));
        var col3 = $("<td/>");
        var input3 = $('<textarea/>', {
            class: 'form-input  inputDefault',
            name: "REASON" + index,
        });
        col3.append(input3);
        row.append(col3);
        if (window.location.pathname == "/GAFormLeave/CreateFormPaidLeave") {
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

    $('.form-input').on('change', function () {
        $('.total').text(getTotalAmount())
    });
    $('.form-input').keypress(function (e) {
        if (e.which == 13) {
            $('.total').text(getTotalAmount())
        }
    });
    $('form input').keydown(function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();

        }
    });
    $('.request-item').click(function (e) {
        var item = $(this).prop('name');
        console.log(item);
    });

    var $contextMenu = $("#contextMenu");

    $("body").on("contextmenu", "table tr", function (e) {
        $contextMenu.css({
            display: "block",
            left: e.pageX,
            top: e.pageY
        });
        return false;
    });
    //$("#formCreate").validate({
    //    rules: {

    //    },
    //    messages: {

    //    },
    //    submitHandler: function (form) {
    //        disableButtonWhenSubmit('#frmpaidleave_create')
    //        form.ajax.submit()
    //    }

    //});
    $("#formCreate").validate({
      
        submitHandler: function (form) {
            disableButtonWhenSubmit('#frmpaidleave_create')
            updateleaveItems()
            form.ajax.submit()
        }

    });


})


