$(function () {

    $('#pass').keydown(function (e) {
        if (e.keyCode == 13) {
            $('#rePass').focus()
            e.preventDefault();
        }
    });
    $('#rePass').keydown(function (e) {
        if (e.keyCode == 13) {
            e.preventDefault();

        }
    });
    $("#formChangePassword").validate({

        submitHandler: function (form) {
            var pass = $('#pass').val();
            var rePass = $('#rePass').val();
            if (pass == rePass) {
                form.submit()
            } else {

                $("#match").addClass('d-none')
                $("#not-match").removeClass('d-none')
                $('#rePass').focus()
            }

        }

    });
})
function mathPassword() {
    var pass = $('#pass').val()
    var rePass = $('#rePass').val()
    if (pass == rePass) {
        $("#match").removeClass('d-none')
        $("#not-match").addClass('d-none')

    } else {
        $("#match").addClass('d-none')
        $("#not-match").removeClass('d-none')
    }
}
