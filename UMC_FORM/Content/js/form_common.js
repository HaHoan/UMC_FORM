
function addCommas(nStr) {
    if (typeof nStr !== 'undefined') {
        nStr = nStr.split(",").join("");
        nStr += '';
        x = nStr.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    }
    else return "0"
}
function getTotalPrice(rowIndex) {
    var total = 0;
    var unit_price = $('#UNIT_PRICE_' + (rowIndex + 1)).val();
    unit_price = convertCommas(unit_price)
    var qty = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(6) input').val();
    if (typeof qty === "undefined") {
        qty = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(6)').text();
    }

    if (typeof unit_price !== "undefined" && unit_price && typeof qty !== "undefined" && qty) {
        total += parseInt(unit_price) * parseInt(qty)
    }

    if (isNaN(total)) return "0"
    else
        return total;
}
function onlyNumber(e) {
    if (/\d+|,+|[/b]+|-+/i.test(e.key)) {
        return true
    } else {
        return false;
    }
}

function convertCommas(nStr) {
    if (typeof nStr === "undefined") return ""
    return nStr.split(',').join('')
}
function getTotalAmount() {
    var total = 0;
    $('#tableInfo tr').each(function (rowIndex) {
        var amount = $('#AMOUNT_' + (rowIndex + 1)).val();
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

$("#fileAttach").change(function () {
    var file = $(this)[0].files;
    if (file) {
        $('.files').empty()
        for (var i = 0; i < file.length; i++) {
            var a = $('<a/>', {
                class: 'badge badge-danger bg-success',
                target: '_blank',
                text:file[i].name
            })
            var br = $('<br/>')
            $('.files').append(a)
            $('.files').append(br)
        }
    }
})