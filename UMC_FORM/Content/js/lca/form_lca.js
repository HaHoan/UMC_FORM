$(function () {

    var $contextMenu = $("#contextMenu");
    $('#formCreate').keydown(function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();
            return false;
        }
    });
    //$('#submitForm').keydown(function (e) {
    //    if (e.keyCode == 13) {
    //        e.preventDefault();
    //        return false;
    //    }
    //});
    $('html').click(function () {
        $contextMenu.hide();
    });
    $(".type_number").keypress(function (e) {
        return onlyNumber(e)
    });
    $("#contextMenu li a").click(function (e) {
        var index = $('.row-info').length + 1;

        var row = $('<tr/>', {
            class: "row-info"
        });
        var col1 = $('<td/>', {
            text: (index)
        })
        row.append(col1);
        row.append(addTd("ITEM_NAME_" + index));
        row.append(addTdQty(index));
        row.append(addTdUnitPrice('UNIT_PRICE_LCA_', index, 15));
        row.append(addTdUnitPrice('TOTAL_LCA_', index, 15));
        row.append(addTdUnitPrice('UNIT_PRICE_CUSTOMER_', index, 20));
        row.append(addTdUnitPrice('TOTAL_CUSTOMER_', index, 20));

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
        $('.summary').remove();

        $('#tableInfo').append(addTdSumary);
        $('.totalLCA').text(getTotalAmountLCA())
        $('.totalCustomer').text(getTotalAmountCustomer())
    });
    $(".btnXoa").on('click', function () {
        deleteRow(this);
    })

    var $contextMenu = $("#contextMenu");

    $("body").on("contextmenu", "#tableInfo tr", function (e) {
        $contextMenu.css({
            display:'block',
            left: e.pageX,
            top: e.pageY
        });
        return false;
    });

    $(".type_number").keypress(function (e) {
        return onlyNumber(e)
    });

    // update total
    for (var i = 1; i <= $('.row-info').length; i++) {

        $('#QTY_' + i).keyup(function (e) {
            var id = $(this).attr('id')
            var rStr = id.substr(4, id.length - 4)
            var rowIndex = parseInt(rStr) - 1
            var textCommas = addCommas($(this).val());
            $(this).val(textCommas);
            var originValue = textCommas.split(",").join("");
            $('#QTY_HIDDEN_' + rowIndex).val(originValue);
            updateEachRowAmount(rowIndex)
            updateQuote()
        })

        $('#UNIT_PRICE_LCA_' + i).keyup(function () {
            var id = $(this).attr('id')
            var rStr = id.substr(15, id.length - 15)
            var rowIndex = parseInt(rStr)
            var textCommas = addCommas($(this).val());
            $(this).val(textCommas);
            var originValue = textCommas.split(",").join("");
            $('#UNIT_PRICE_LCA_HIDDEN_' + rowIndex + '').val(originValue);
            updateEachRowAmount(rowIndex - 1)
            updateQuote()
        });
        $('#UNIT_PRICE_CUSTOMER_' + i).keyup(function () {
            var id = $(this).attr('id')
            var rStr = id.substr(20, id.length - 20)
            var rowIndex = parseInt(rStr)
            var textCommas = addCommas($(this).val());
            $(this).val(textCommas);
            var originValue = textCommas.split(",").join("");
            $('#UNIT_PRICE_CUSTOMER_HIDDEN_' + rowIndex + '').val(originValue);
            updateEachRowAmount(rowIndex - 1)
            updateQuote()
        });

    }

    $('.form input').keydown(function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();

        }
    });

    //for get data
    $('.totalLCA').text(getTotalAmountLCA())
    $('.totalCustomer').text(getTotalAmountCustomer())
    updateSTT()
    $(".form-input").each(function () {
        if ($(this).val() == '0')
            $(this).val('')
    });

    $('[id^="QTY_"]').each(function () {
        $(this).addClass('text-center')
    })
});
function updateQuote() {
    try {
        quotes = []
        $('.row-info').each(function (rowIndex) {
            index = rowIndex + 1
            var item = $('#ITEM_NAME_' + index).val()
            if (item == "") return true
            var quantity = $('#QTY_' + index).val().split(",").join("")
            var lca_unit_price = $('#UNIT_PRICE_LCA_' + index).val().split(",").join("")
            var lca_total = $('#TOTAL_LCA_' + index).val().split(",").join("")
            var customer_unit_price = $('#UNIT_PRICE_CUSTOMER_' + index).val().split(",").join("")
            var customer_total = $('#TOTAL_CUSTOMER_' + index).val().split(",").join("")
            var obj = {
                NO: index,
                REQUEST_ITEM: item,
                QUANTITY: quantity == "" ? 0 : parseInt(quantity),
                LCA_UNIT_PRICE: lca_unit_price == "" ? 0 : parseInt(lca_unit_price),
                LCA_TOTAL_COST: lca_total == "" ? 0 : parseInt(lca_total),
                CUSTOMER_UNIT_PRICE: customer_unit_price == "" ? 0 : parseInt(customer_unit_price),
                CUSTOMER_TOTAL_COST: customer_total == "" ? 0 : parseInt(customer_total),
            }
            quotes.push(obj)
        });

        $('#quotes').val(JSON.stringify(quotes))
    } catch (e) {
        alert(e)
    }

}
function addTd(name) {
    var col2 = $('<td/>');
    var input2 = $('<input/>', {
        class: 'form-input inputDefault'
    })
    col2.append(input2);
    input2.attr('id', name)
    input2.attr('name', name)
    return col2;
}
function addTdQty(rowIndex) {
    var col = $('<td/>');
    var input1 = $('<input/>', {
        class: 'form-input type_number inputDefault',
        keyup: function (e) {
            var id = $(this).attr('id')
            var rStr = id.substr(4, id.length - 4)
            var rowIndex = parseInt(rStr) - 1
            var textCommas = addCommas($(this).val());
            $(this).val(textCommas);
            var originValue = textCommas.split(",").join("");
            $('#QTY_HIDDEN_' + rowIndex).val(originValue);
            updateEachRowAmount(rowIndex)
            updateQuote()
        }
    })
    col.append(input1);
    input1.attr('id', 'QTY_' + rowIndex);
    var input2 = $('<input/>', {
        type: "hidden"
    })
    col.append(input2);
    input2.attr('id', 'QTY_HIDDEN_' + rowIndex);
    input2.attr('name', 'QTY_' + rowIndex);
    return col;
}
function addTdUnitPrice(name, rowIndex, length) {
    var col = $('<td/>');
    var input1 = $('<input/>', {
        class: 'form-input type_number inputDefault',
        keyup: function (e) {
            var id = $(this).attr('id')
            var rStr = id.substr(length, id.length - length)
            var rowIndex = parseInt(rStr) - 1
            var textCommas = addCommas($(this).val());
            $(this).val(textCommas);
            var originValue = textCommas.split(",").join("");
            $('#' + name + '_HIDDEN_' + rowIndex).val(originValue);
            updateEachRowAmount(rowIndex)
            updateQuote()
        }
    })
    input1.prop('readonly',true)
    col.append(input1);
    input1.attr('id', name + rowIndex);
    var input2 = $('<input/>', {
        type: "hidden"
    })
    col.append(input2);
    input2.attr('id', name + 'HIDDEN_' + rowIndex);
    input2.attr('name', name + rowIndex);
    return col;
}
function addTdSumary() {

    var row = $('<tr />', {
        class: "p-3 summary"
    })
    var td = $('<td />', {
        class: "border-0 font-italic",
        colspan: "4",
        text: " * If request Items are over 5, please issue more request sheet"
    })
    row.append(td)
    var td1 = $('<td />', {
        class: "border-0 font-weight-bold text-right"

    })
    var span1 = $('<span />', {
        text: "Total: "
    })
    var span2 = $('<span />', {
        class: 'totalLCA'
    })
    var span3 = $('<span />', {
        text: '$'
    })
    td1.append(span1)
    td1.append(span2)
    td1.append(span3)

    row.append(td1)

    var td2 = $('<td />', {
        class: "border-0 font-weight-bold text-right",
        colspan: 2

    })
    var span4 = $('<span />', {
        text: "Total: "
    })
    var span5 = $('<span />', {
        class: 'totalCustomer'
    })
    var span6 = $('<span />', {
        text: '$'
    })
    td2.append(span4)
    td2.append(span5)
    td2.append(span6)

    row.append(td2)
    return row;
}
function updateEachRowAmount(rowIndex) {
    var totalLCA = getTotalPriceLCA(rowIndex)
    $('#TOTAL_LCA_' + (rowIndex + 1)).val(totalLCA)
    $('#TOTAL_LCA_' + (rowIndex + 1)).val(addCommas(totalLCA.toString()))
    $('.totalLCA').text(getTotalAmountLCA())

    var totalCustomer = getTotalPriceCustomer(rowIndex)
    $('#TOTAL_CUSTOMER_' + (rowIndex + 1)).val(totalCustomer)
    $('#TOTAL_CUSTOMER_' + (rowIndex + 1)).val(addCommas(totalCustomer.toString()))
    $('.totalCustomer').text(getTotalAmountCustomer())

}
function getTotalPriceCustomer(rowIndex) {
    var totalCustomer = 0;

    var qty = $('#QTY_' + (rowIndex + 1)).val();
    qty = qty.split(",").join("");
    if (typeof qty === "undefined") {
        qty = $('#QTY_' + (index + 1)).text();
    }
    var unit_price_customer = $('#UNIT_PRICE_CUSTOMER_' + (rowIndex + 1)).val();
    if (typeof unit_price_customer === "undefined") {
        unit_price_customer = "0"
    }
    unit_price_customer = convertCommas(unit_price_customer)
    if (typeof unit_price_customer !== "undefined" && unit_price_customer && typeof qty !== "undefined" && qty) {
        totalCustomer += parseInt(unit_price_customer) * parseInt(qty)
    }

    if (isNaN(totalCustomer)) return "0"
    else
        return totalCustomer;
}
function getTotalPriceLCA(rowIndex) {
    var totalLCA = 0;

    var qty = $('#QTY_' + (rowIndex + 1)).val();
    qty = qty.split(",").join("");
    if (typeof qty === "undefined") {
        qty = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(2)').text();
    }
    var unit_price_lca = $('#UNIT_PRICE_LCA_' + (rowIndex + 1)).val();
    if (typeof unit_price_lca === "undefined") {
        unit_price_lca = "0"
    }
    unit_price_lca = convertCommas(unit_price_lca)
    if (typeof unit_price_lca !== "undefined" && unit_price_lca && typeof qty !== "undefined" && qty) {
        totalLCA += parseInt(unit_price_lca) * parseInt(qty)
    }

    if (isNaN(totalLCA)) return "0"
    else
        return totalLCA;
}
function getTotalAmountLCA() {
    var total = 0;
    $('.row-info').each(function (rowIndex) {
        var amount = $('#TOTAL_LCA_' + (rowIndex + 1)).val();
        if (typeof amount !== 'undefined') {
            var result = amount.split(",").join("");
            if (typeof result !== "undefined" && result) {
                total += parseInt(result);
            }
        }
    });

    if (isNaN(total)) return "0"
    else
        return addCommas(total.toString());
}
function getTotalAmountCustomer() {
    var total = 0;
    $('.row-info').each(function (rowIndex) {
        var amount = $('#TOTAL_CUSTOMER_' + (rowIndex + 1)).val();
        if (typeof amount !== 'undefined') {
            var result = amount.split(",").join("");
            if (typeof result !== "undefined" && result) {
                total += parseInt(result);
            }
        }
    });

    if (isNaN(total)) return "0"
    else
        return addCommas(total.toString());
}
function deleteRow(e) {
    $(e).parent().parent().remove();
    $('.totalLCA').text(getTotalAmountLCA())
    $('.totalCustomer').text(getTotalAmountCustomer())
    updateSTT();
}
function updateSTT() {
    var index = 1;
    $('#tableInfo tr').each(function (rowIndex) {
        var stt = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(0)').text();
        if (!isNaN(stt)) {
            $('#tableInfo tr:eq(' + rowIndex + ') td:eq(0)').text(index);
            index++;
        }

    });
}
