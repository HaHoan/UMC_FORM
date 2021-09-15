function updateEachRowAmount(rowIndex) {
    var amount = getTotalPrice(rowIndex)
    $('#AMOUNT_' + (rowIndex + 1)).val(amount)
    $('#AMOUNT_' + (rowIndex + 1) + "_VIEW").text(addCommas(amount.toString()))
    $('#tableInfo tr:eq(' + rowIndex + ') td:eq(8) .form-input').val(addCommas(amount.toString()))
    $('.total').text(getTotalAmount())
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
function addTdQty(rowIndex) {
    var col2 = $('<td/>');
    var input1 = $('<input/>', {
        class: 'form-input type_number',

        change: function () {
            var id = $(this).attr('id')
            var rStr = id.substr(4, id.length - 4)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        }
    })

    col2.append(input1);
    input1.attr('id', 'QTY_' + rowIndex);
    input1.attr('name', 'QTY_' + rowIndex);
    return col2;
}
function addTdAmount(rowIndex) {
    var col2 = $('<td/>');
    var input1 = $('<input/>', {
        class: 'form-input'
    })

    col2.append(input1);
    input1.attr('id', 'AMOUNT_' + rowIndex);
    input1.attr('name', 'AMOUNT_' + rowIndex);
    input1.prop('readonly', true);
    return col2;
}
function addTdPrice(rowIndex) {
    var col = $('<td/>');
    var input = $('<input/>', {
        class: 'form-input type_number',
        change: function () {
            var id = $(this).attr('id')
            var rStr = id.substr(11, id.length - 11)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        },
        keyup: function () {
            var id = $(this).attr('id')
            var rStr = id.substr(11, id.length - 11)
            var rowIndex = parseInt(rStr)
            var textCommas = addCommas($(this).val());
            $(this).val(textCommas);
            var originValue = textCommas.split(",").join("");
            $('#UNIT_PRICE_' + rowIndex + '_HIDDEN').val(originValue);
            updateEachRowAmount(rowIndex - 1)
        }
    })
    var input1 = $('<input/>', {
        type: 'hidden'
    })
    col.append(input);
    col.append(input1);
    input1.attr('id', 'UNIT_PRICE_' + rowIndex + '_HIDDEN');
    input1.attr('name', 'UNIT_PRICE_' + rowIndex);
    input.attr('id', 'UNIT_PRICE_' + rowIndex);

    return col;
}



$(function () {
    //var formType = document.getElementById("formCreate");
    //// Create Form
    //if (formType != null) {

    //}
    //else { // Detail Form
       

    //}
    for (var i = 1; i <= $('#tableInfo tr').length; i++) {
        $('#QTY_' + i).change(function (e) {
            var id = $(this).attr('id')
            var rStr = id.substr(4, id.length - 4)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        })

        $('#UNIT_PRICE_' + i).keyup(function () {
            var id = $(this).attr('id')
            var rStr = id.substr(11, id.length - 11)
            var rowIndex = parseInt(rStr)
            var textCommas = addCommas($(this).val());
            $(this).val(textCommas);
            var originValue = textCommas.split(",").join("");
            $('#UNIT_PRICE_' + rowIndex + '_HIDDEN').val(originValue);
            updateEachRowAmount(rowIndex - 1)
        });
        $('#UNIT_PRICE_' + i).change(function () {
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
        var index = $('#tableInfo tr').length;
        var row = $('<tr/>');
        var col1 = $('<td/>', {
            text: (index)
        })
        row.append(col1);
        row.append(addTd("NO_" + index));
        row.append(addTd("ITEM_NAME_" + index));
        row.append(addTd("DES_" + index));
        row.append(addTd("VENDOR_" + index));
        row.append(addTdPrice(index));
        row.append(addTdQty(index));
        row.append(addTd('UNIT_' + index));
        row.append(addTdAmount(index));
        var col3 = $("<td/>");
        var div3 = $('<div/>', {
            class: "form-inline"
        });
        var input3 = $('<input/>', {
            type: 'radio',
            name: "OWNER_OF_ITEM_" + index,
            val: "u",
            checked: true
        });
        var label3 = $("<label/>", {
            for: "u",
            text: "U"
        })
        div3.append(input3);
        div3.append(label3);
        var input3 = $('<input/>', {
            type: 'radio',
            name: "OWNER_OF_ITEM_" + index,
            val: "c",

        });
        var label3 = $("<label/>", {
            for: "c",
            text: "C"
        })
        div3.append(input3);
        div3.append(label3);
        col3.append(div3);
        row.append(col3);

        row.append(addTd('COST_CENTER_' + index))

        var col4 = $("<td/>");
        var div4 = $('<div/>', {
            class: "form-inline"
        });
        var input4 = $('<input/>', {
            type: 'radio',
            name: "AK_" + index,
            val: "a",
            checked: true
        });
        var label4 = $("<label/>", {
            for: "a",
            text: "A"
        })
        div4.append(input4);
        div4.append(label4);
        var input4 = $('<input/>', {
            type: 'radio',
            name: "AK_" + index,
            val: "K"
        });
        var label4 = $("<label/>", {
            for: "k",
            text: "K"
        })
        div4.append(input4);
        div4.append(label4);
        col4.append(div4);
        row.append(col4);
        var rowDelete = $('<td/>');
        var btnXoa = $('<button/>', {
            text: 'Xóa',
            type: 'button',
            class: 'btnXoa btn btn-danger',
            click: function () {
                deleteRow(this)
            }
        });
        row.append(addTd('ACOUNT_CODE_' + index))
        row.append(addTd('ASSET_NO_' + index))
        rowDelete.append(btnXoa);

        row.append(rowDelete);
        $('#tableInfo').append(row);
        $('.summary').remove();
        var row = $('<tr/>', {
            class: 'summary'
        });
        var td1 = $('<td/>', {
            colspan: "6"
        });
        var area = $('<textarea/>', {
            class: "form-input",
            row: 2
        })
        td1.append(area)
        var td2 = $('<td/>', {
            colspan: "2",
            class: "font-weight-bold",
            text: "TOTAL"
        });
        var td3 = $('<td/>', {
            text: getTotalAmount(),
            class: "font-weight-bold total"
        });
        var td4 = $('<td/>', {
            colspan: "2",
            class: "font-weight-bold",
            text: "（USD or VND）"
        });
        row.append(td1);
        row.append(td2);
        row.append(td3);
        row.append(td4);
        row.append($('<td/>'));
        row.append($('<td/>'));
        row.append($('<td/>'));
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

    $('#btnAddComment').click(function (e) {
        if (!$.trim($("#txbComment").val())) {
            $('#txbComment').focus();
            return;
        }
        var cmt = $("#txbComment").val();
        var ticket = $("#ticketNo").text();
        var user = $("#user").text();
        $.ajax({
            type: "POST",
            url: "/PurAccF06/SaveComments",
            data: { ticket: ticket, comments: cmt },
            success: function (data) {
                var li = $('<li/>', {
                    class: "list-group-item"
                });
                var row = $('<div/>', {
                    class: "row"
                });
                var col1 = $('<div/>', {
                    class: "d-inline ml-2"
                });
                var img = $('<img/>', {
                    src: imgAvatarSource,
                    alt: "avatar",
                    class: "avatar"
                });
                col1.append(img);
                row.append(col1);
                var col2 = $('<div/>', {
                    class: "d-inline ml-1"
                });

                var name = $('<div/>', {
                    class: "font-weight-bold mt-2 ml-1",
                    text: user
                });
                col2.append(name);
                var date = $('<div/>', {
                    text: moment().calendar(),
                    class: "ml-2 text-muted"
                });

                col2.append(date);
                row.append(col2);
                li.append(row);

                var msg = $('#txbComment').val();
                var arrChar = msg.split(' ');
                msg = "";
                var content = $('<p/>');
                var userTag = [];
                arrChar.forEach(function (item) {

                    var startChar = item.substr(0, 1);
                    if (startChar == "@") {
                        var username = item.substr(1)
                        var user = users.find(x => x.username === username);
                        if (user != null) {
                            if (!userTag.includes(user.name)) {
                                userTag.push(user.name);
                            }

                            var span = $('<span/>', {

                            });
                            span.css("background", "#bbdefb")
                            span.append(item);
                            content.append(span);
                            content.append(" ");
                        } else {
                            content.append(item + " ");
                        }

                    } else {
                        content.append(item + " ");
                    }

                })

                li.append(content);
                var a = $('<a/>', {
                    href: "javascript:void(0)",
                    style: "text-decoration:none",
                    class: "text-secondary",

                });
                var i = $('<i/>', {
                    class: "fas fa-thumbs-up mr-1"
                });
                a.append(i);
                a.append("Like");
                li.append(a);
                $('.list-comment').prepend(li);
                numberComment++;
                $('#totalComment').text(numberComment);
                $('#txbComment').val("");
                $('#txbComment').focus();
            },
            error: function (err) {
                alert('error' + err.msg);
            }
        });


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