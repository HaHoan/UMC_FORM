$(function () {
    var imgAvatarSource = base_url + "/Content/images/icons8-user-100.png";
    var ticket = $("#ticketNo").text();
    $.ajax({
        type: "GET",
        url: "/Comment/GetAllComment",
        dataType: 'json',
        data: { ticket: ticket },
        success: function (data) {
            var items = '';
            $.each(data, function (i, item) {
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
                    text: item.COMMENT_USER
                });
                col2.append(name);
                var dt = moment(item.UPD_DATE).format("YYYYMMDDhhmmss");
                var date = $('<div/>', {
                    text: moment(dt, "YYYYMMDDhhmmss").fromNow(),
                    class: "ml-2 text-muted"
                });

                col2.append(date);
                row.append(col2);
                li.append(row);

                var msg = $('#txbComment').val();
                var content = $('<p/>');
                content.append(item.COMMENT_DETAIL);
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
            });
            numberComment = data.length;
            $('#totalComment').text(numberComment);
            $('#txbComment').val("");
            $('#txbComment').focus();
        },
        error: function (err) {
            alert('error' + err.msg);
        }
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
            url: "/Comment/SaveComments",
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
})
