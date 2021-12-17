$(function () {
    /* Mutil Language */
    var lang = localStorage.getItem('lang') || 'en';

    changeLanguage(lang);

})
function changeLanguage(lang) {
    if (lang == null || lang == '') lang = 'en'
    var base_url = window.location.origin;
    if (lang == "en") {
        $(".icon-lang").attr('src', base_url + "/Content/assets/img/united-states.png");
        $(".text-lang").text("Tiếng Anh")
    } else if (lang == "vi") {
        $(".icon-lang").attr('src', base_url + "/Content/assets/img/vietnam.png");
        $(".text-lang").text("Tiếng Việt")
    } else if (lang == "ja") {
        $(".icon-lang").attr('src', base_url + "/Content/assets/img/japan.png");
        $(".text-lang").text("Tiếng Nhật")
    } else {
        $(".icon-lang").attr('src', base_url + "/Content/assets/img/united-states.png");
        $(".text-lang").text("Tiếng Anh")
    }

    $.getJSON(base_url + "/Content/Json/lang.json", function (data) {
        $(".lang").each(function (index, element) {
            $(this).text(data[lang][$(this).attr("key")]);
            $(this).attr("placeholder", data[lang][$(this).attr("key")]);
        });
    });
}
$(".translate").click(function () {
    var lang = $(this).attr("id");
    localStorage.setItem('lang', lang);
    changeLanguage(lang);

});
