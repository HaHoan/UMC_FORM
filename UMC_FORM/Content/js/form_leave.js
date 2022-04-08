function updateEachRowAmount(rowIndex) {
    var amount = getTotalDay(rowIndex)
    $('#TOTAL_DAY' + (rowIndex + 1)).val(amount)
    $('#TOTAL_DAY' + (rowIndex + 1) + "_VIEW").text(addCommas(amount.toString()))
    $('#tableInfo tr:eq(' + rowIndex + ') td:eq(8) .form-input').val(addCommas(amount.toString()))
    $('.total').text(getTotalAmount())
}
function getTotalDay(rowIndex) {
    var total = 0;
    var startleave = Math.floor($('#START_LEAVE' + (rowIndex)).getTime());
    var finishleave = Math.floor($('#FINISH_LEAVE' + (rowIndex + 1)).getTime());
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
    var input1 = $('<input/>', {
        class: 'form-input inputDefault',
        type: 'datetime-local',
        change: function () {
            var id = $(this).attr('id')
            var rStr = id.substr(4, id.length - 4)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        }
    })

    col2.append(input1);
    input1.attr('id', 'START_LEAVE' + rowIndex);
    input1.attr('name', 'START_LEAVE' + rowIndex);
    return col2;
}
function addTdFinishleave(rowIndex) {
    var col2 = $('<td/>');
    var input1 = $('<input/>', {
        class: 'form-input inputDefault',
        type: 'datetime-local',
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



$(function () {
    var formType = document.getElementById("formCreate");
    // Create Form
    if (formType != null) {

    }
    else { // Detail Form


    }
    for (var i = 1; i <= $('#tableInfo tr').length; i++) {
        $('#FINISH_LEAVE' + i).change(function (e) {
            var id = $(this).attr('id')
            var rStr = id.substr(4, id.length - 4)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        })
        $('#START_LEAVE' + i).change(function () {
            var id = $(this).attr('id')
            var rStr = id.substr(11, id.length - 11)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        });

    }

    $('#robinAttach').hide();
    $('#customer-checkout-info').hide();

    $('input[type=radio][name=robin]').change(function () {
        if (this.value == 'y') {
            $('#robinAttach').show();
        }
        else if (this.value == 'n') {
            $('#robinAttach').hide();
        }
    });
    $('input[type=radio][name=IS_CUS_PAY]').change(function () {
        if (this.value == 'True') {
            $('#customer-checkout-info').show();
        }
        else if (this.value == 'False') {
            $('#customer-checkout-info').hide();
        }
    });


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
        row.append(addTd("NAME_" + index));
        row.append(addTd("CODE_" + index));
        row.append(addTdstartleave(index));
        row.append(addTdFinishleave(index));
        row.append(addTdtotalday(index));
        row.append(addTd('REASON_LEAVE' + index));
        row.append(addTd('SPECIAL_LEAVE' + index));
        row.append(addTd('RMKS' + index));
        /* row.append(addTdAmount(index));*/

        var rowDelete = $('<td/>');
        var btnXoa = $('<button/>', {
            text: 'Xóa',
            type: 'button',
            class: 'btnXoa btn btn-danger',
            click: function () {
                deleteRow(this)
            }
        });
        //row.append(addTd('ACOUNT_CODE_' + index))
        //row.append(addTd('ASSET_NO_' + index))
        rowDelete.append(btnXoa);

        row.append(rowDelete);
        $('#tableInfo').append(row);
        K
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





})