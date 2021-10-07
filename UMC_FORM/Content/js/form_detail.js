var numberComment = 0;
var base_url = window.location.origin;
var imgAvatarSource = base_url + "/Content/images/icons8-user-100.png";
function updateEachRowAmount(rowIndex) {
    var amount = getTotalPrice(rowIndex)
    $('#AMOUNT_' + (rowIndex + 1)).val(amount)
    $('#AMOUNT_' + (rowIndex + 1) + "_VIEW").text(addCommas(amount.toString()))
    $('#tableInfo tr:eq(' + rowIndex + ') td:eq(8) .form-input').val(addCommas(amount.toString()))
    $('.total').text(getTotalAmount())
}
function getTotalAmountView() {
    var total = 0;
    $('#tableInfo tr').each(function (rowIndex) {
        var amount = $('#tableInfo tr:eq(' + rowIndex + ') td:eq(8)').text();
        if (typeof amount !== "undefined" && amount) {
            total += parseInt(amount);
        }

    });
    if (isNaN(total)) return "0"
    else
        return total;
}
$(function () {
    for (var i = 1; i <= $('#tableInfo tr').length; i++) {
        var textCommas1 = addCommas($('#UNIT_PRICE_' + i).val());
        var amount1 = addCommas($('#AMOUNT_' + i).val());

        $("#UNIT_PRICE_" + i + "_VIEW").text(textCommas1);
        $("#UNIT_PRICE_" + i + "_VIEW").val(textCommas1);
        $("#AMOUNT_" + i + "_VIEW").text(amount1);
        $("#UNIT_PRICE_" + i + "_VIEW").text(textCommas1);
        $("#AMOUNT_" + i + "_VIEW").text(amount1);

        updateEachRowAmount(i - 1)

        $('#UNIT_PRICE_' + i + "_VIEW").keyup(function () {
            var id = $(this).attr('id')
            var rStr = id.substr(11, id.length - 11)
            var rowIndex = parseInt(rStr)
            var textCommas = addCommas($(this).val());
            $(this).val(textCommas);
            var originValue = textCommas.split(",").join("");
            $('#UNIT_PRICE_' + rowIndex).val(originValue);
            updateEachRowAmount(rowIndex - 1)
        });
        $('#UNIT_PRICE_' + i + "_VIEW").change(function () {
            var id = $(this).attr('id')
            var rStr = id.substr(11, id.length - 11)
            var rowIndex = parseInt(rStr) - 1
            updateEachRowAmount(rowIndex)
        });
    }
  
   
})