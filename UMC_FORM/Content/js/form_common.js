
$(function () {
    $("#fileAttach").change(function () {
        changeFileAttach(this, '#listFiles','.files')
    })
    $('#fileQuoteAttach').change(function () {
        changeFileAttach(this, '#listFilesQuote','.fileQuotes')
    })
})


function changeFileAttach(e,listFilesToElement,viewFileElement) {
    var fileInput = $(e);

    var filePath = fileInput.val();

    // Allowing file type
    var allowedExtensions =
        /(\.pdf|\.xlsx|\.xls|\.doc|\.docx|\.ppt|\.pptx|\.jpg|\.jpeg|\.mp3|\.mp4|\.png|\.pps|\.ppsx|\.pptm|\.txt)$/i;

    if (!allowedExtensions.exec(filePath)) {
        alert('File có thể chứa virut hoặc mã độc!');
        fileInput.val('')
        return false;
    }
    else {
        var formdata = new FormData(); //FormData object
        var fileInput = $(e)[0].files;
        //Iterating through each files selected in fileInput
        for (i = 0; i < fileInput.length; i++) {
            //Appending each file to FormData object
            formdata.append(fileInput[i].name, fileInput[i]);
        }
        //Creating an XMLHttpRequest and sending
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/LCA/UploadFiles');
        xhr.send(formdata);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var response = JSON.parse(xhr.responseText)
                if (response.result == 'success' && response.message != null) {
                    var listFilesStr = $(listFilesToElement).val()
                    var listFiles = []
                    if (listFilesStr != '') {
                        listFiles = JSON.parse(listFilesStr)
                    }
                    for (var i = 0; i < response.message.length; i++) {
                        listFiles.push(response.message[i])
                        var div = $('<div />', {
                            class: 'badge badge-danger bg-light p-2',
                            name: 'TEXT' + response.message[i].FILE_URL

                        })
                        var br = $('<br />', {
                            name: 'TEXT' + response.message[i].FILE_URL
                        })
                        var a = $('<a/>', {
                            class: 'font-weight-bold text-primary',
                            style: 'font-weight:900;font-size:15px; text-decoration: underline;',
                            target: '_blank',
                            text: response.message[i].FILE_NAME,
                            href: response.message[i].FILE_URL
                        })
                        var button = $('<button />', {
                            class: 'btn-image border-0 ',
                            name: response.message[i].FILE_URL,
                            click: function (e) {
                                e.preventDefault();
                                deleteFiles($(this).attr('name'))
                            }

                        })
                        var i = $('<i />', {
                            class: 'fas fa-times ml-5 text-dark',
                            style: 'font-size:15px',

                        })
                        button.append(i)
                        div.append(a)
                        div.append(button)
                        div.append($('<br/>'))

                        $(viewFileElement).append(div)
                        $(viewFileElement).append(br)

                    }
                    $(listFilesToElement).val(JSON.stringify(listFiles))
                    $(e).val('')
                }

            }
        }
    }

}
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
function validate(s) {
    var rgx = /^[0-9]*\.?[0-9]*$/;
    return s.match(rgx);
}
function onlyNumber(e) {

    if (validate(e.key)) {
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

    if (isNaN(total) || total == 0) return "0"
    else
        return addCommas(total.toString());
}
function convertDateToValid(dateStr) {
    if (!dateStr) return
    var from = dateStr.split("/")
    var f = new Date(from[2], from[1] - 1, from[0])
    return from[2] + '-' + from[1] + '-' + from[0]
}

function convertDateTimeToDateValid(dateStr) {
    try {
        if (!dateStr) return
        var dt = dateStr.split(" ")
        var date = dt[0]
        var time = dt[1]
        var dateArr = date.split("/")
        var timeArr = time.split(":")
        return new Date(dateArr[2], dateArr[1] - 1, dateArr[0], timeArr[0], timeArr[1])
    } catch {
        return null;
    }
}
function getRowIndexFromId(id) {
    var suffix = id.match(/\d+/);
    return parseInt(suffix)
}

function deleteFiles(name) {
    try {
        $.ajax({
            url: "/LCA/DeleteFiles",
            type: "Post",
            data: {
                deleteFile: name
            },
            success: function (response) {
                if (response.result == 'success') {
                    $('div[name="TEXT' + name + '"').remove()
                    $('br[name="TEXT' + name + '"').remove()
                    var listFilesStr = $('#listFiles').val()
                    var listFiles = []
                    if (listFilesStr != '') {
                        listFiles = JSON.parse(listFilesStr)
                    }
                    var temp = [];
                    $.each(listFiles, function (index, value) {
                        if (value.FILE_URL != name) {
                            temp.push(value)
                        }
                    });

                    listFiles = temp;
                    $('#listFiles').val(JSON.stringify(listFiles))
                } else {
                    alert(response.message)
                }

            },
            error: function (e) {
                console.log(e);
            }
        });
    } catch (e) {
        alert(e)
    }

}
function checkUnicode(text) {
    regex = /^[a-zA-Z0-9_ ぁ-んァ-ン一-龥\.０-９@\n]+$/; // Chỉ chấp nhận ký tự alphabet thường hoặc ký tự hoa
    if (regex.test(text)) { // true nếu text chỉ chứa ký tự alphabet thường hoặc hoa, false trong trường hợp còn lại. 
        return true
    } else {
        return false
    }
}

