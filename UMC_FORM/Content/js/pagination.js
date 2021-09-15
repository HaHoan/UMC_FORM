$(function () {
    var items = $(".table-request .row-request");
    var numItems = items.length;
    var perPage = 50;

    items.slice(perPage).hide();

    $('#pagination-container').pagination({
        items: numItems,
        itemsOnPage: perPage,
        prevText: "&laquo;",
        nextText: "&raquo;",
        onPageClick: function (pageNumber) {
            var showFrom = perPage * (pageNumber - 1);
            var showTo = showFrom + perPage;
            items.hide().slice(showFrom, showTo).show();
        }
    });

    //list in form
    var itemsList = $(".list-request .list-request-item");
    var numItemsList = itemsList.length;
    var perPageList = 2;

    itemsList.slice(perPageList).hide();

    $('#pagination-container-list').pagination({
        items: numItemsList,
        itemsOnPage: perPageList,
        prevText: "&laquo;",
        nextText: "&raquo;",
        onPageClick: function (pageNumber) {
            var showFrom = perPageList * (pageNumber - 1);
            var showTo = showFrom + perPageList;
            itemsList.hide().slice(showFrom, showTo).show();
        }
    });
});