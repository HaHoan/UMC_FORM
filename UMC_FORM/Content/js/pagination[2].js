$(function () {
    var items = $(".table-request .row-request");
    var numItems = items.length;
    var perPage = 19;

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

    //list in done
    var itemsList = $(".list-request .list-request-item");
    var numItemsList = itemsList.length;
    var perPageList = 20;

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
    //list in not yet
    var itemsList = $(".list-request .list-request-item");
    var numItemsList = itemsList.length;
    var perPageList = 20;

    itemsList.slice(perPageList).hide();

    $('#pagination-not-yet-list').pagination({
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