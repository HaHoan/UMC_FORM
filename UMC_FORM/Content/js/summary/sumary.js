$(function () {
    $('#filter_month_from').val(moment().format("yyyy-MM"));
    $('#filter_month_to').val(moment().format("yyyy-MM"));
    setFilterDateWhenChange();

    $('#filter_month_from').change(function () {
        var start = moment($(this).val() + "-01");
        var end = moment($('#filter_month_to').val() + "-01");
        if (end < start) {
            alert("Bạn phải chọn thời gian nhỏ hơn end month!");
            return;
        }
        setFilterDateWhenChange($(this).val());
        loadData();
    });
    $('#filter_month_to').change(function () {
        var start = moment($('#filter_month_from').val() + "-01");
        var end = moment($('#filter_month_to').val() + "-01");
        if (end < start) {
            alert("Bạn phải chọn thời gian lớn hơn start month!");
            return;
        }
        setFilterDateWhenChange($(this).val());
        loadData();
    });
    $('#filter_date_from').change(function () {
        var start = moment($(this).val());
        var end = moment($('#filter_date_to').val());
        if (end < start) {
            alert("Bạn phải chọn thời gian nhỏ hơn end date!");
            return;
        }
        setFilterMonthWhenChange($(this).val());
        loadData();
    });
    $('#filter_date_to').change(function () {
        var start = moment($('#filter_date_from').val());
        var end = moment($(this).val() + "-01");
        if (end < start) {
            alert("Bạn phải chọn thời gian lớn hơn start date!");
            return;
        }
        setFilterMonthWhenChange($(this).val());
        loadData();
    });
    loadData()
})
function setFilterDateWhenChange() {
    $('#filter_date_from').val($('#filter_month_from').val() + "-01");
    $('#filter_date_to').val(moment($('#filter_month_to').val()).endOf('month').format('yyyy-MM-DD'));
}
function setFilterMonthWhenChange() {
    $('#filter_month_from').val(moment($('#filter_date_from').val()).format('yyyy-MM'));
    $('#filter_month_to').val(moment($('#filter_date_to').val()).format('yyyy-MM'));
}

function loadData() {
    var selectedFilter = $('#daily').is(':checked') ? 'MONTHLY' : 'DAILY'
    $.ajax({
        url: "/Summary/LoadData",
        data: {
            filter: selectedFilter,
            startDate: $('#filter_date_from').val(),
            endDate: $('#filter_date_to').val()
        },
        success: function (response) {
            createChartMonthly(response.monthly, "chartMonthly","MONTHLY CHART")
            createChartMonthly(response.daily, "chartDaily", "DAILY CHART")
            var totalMoney = addCommas(response.totalMoney.toString())
            $("#totalMoney").text(totalMoney)

        },
        error: function (e) {
            console.log(e);
        }
    });

}

function toogleDataSeries(e) {
    if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
        e.dataSeries.visible = false;
    } else {
        e.dataSeries.visible = true;
    }
    chart.render();
}

function createChartMonthly(list, name,title) {
    var chart = new CanvasJS.Chart(name, {
        title: {
            text: title,
            fontFamily: "tahoma",
            fontSize: 15
        },
        axisY: {
            title: "",
            lineColor: "#4F81BC",
            tickColor: "#4F81BC",
            labelFontColor: "#4F81BC",
            minimum: 0
        },
        axisY2: {
            title: "",
            suffix: "",
            lineColor: "#C0504E",
            tickColor: "#C0504E",
            labelFontColor: "#C0504E",
            minimum: 0
        },
        data: [{
            type: "column",
            dataPoints: list
        }]
    });
    chart.render();
    var dps = [];
    var yValue, yTotal = 0, y = 0;

    for (var i = 0; i < chart.data[0].dataPoints.length; i++)
        yTotal += chart.data[0].dataPoints[i].y;

    for (var i = 0; i < chart.data[0].dataPoints.length; i++) {
        yValue = chart.data[0].dataPoints[i].y;
        y += yValue
        dps.push({ label: chart.data[0].dataPoints[i].label, y: y });
    }

    chart.addTo("data", { type: "line", yValueFormatString: "#,###.00 VNĐ", dataPoints: dps });
    chart.data[1].set("axisYType", "secondary", false);
    chart.axisY[0].set("maximum", yTotal*1.1);
    chart.axisY2[0].set("maximum", yTotal*1.1);
}
