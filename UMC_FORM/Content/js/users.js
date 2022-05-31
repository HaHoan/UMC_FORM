$(function () {
    $('.checkbox-in-ddl').on('click', function (e) {
        if ($(this).hasClass('checkbox-in-ddl') || $(this).hasClass('dropdown-header')) {
            e.stopPropagation(); // <-------- POINT :D
        }
    });
    $('.checkbox-in-ddl input').change(function () {
        var arr = []
        $('.checkbox-in-ddl input').each(function () {
            if (this.checked) {
                var newPos = $(this).val()
                var newPosName = $(this).next('span').text().toUpperCase()
                var obj = {
                    POSITION_CODE: newPos,
                    NAME: newPosName
                }
                arr.push(obj)
            }
        })
        $('#POSITIONs').val(JSON.stringify(arr))
    })
})