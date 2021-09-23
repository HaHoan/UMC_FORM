
var users = [];
var listStep = [];
//var listStep = [

//    {
//        index: 0,
//        key: 'step-1',
//        name: 'Applicant',
//        return: {

//        }

//    },
//    {
//        index: 1,
//        key: 'step-2',
//        name: 'Dept Manager',
//        return: {
//            "name": "Back to",
//            "items": {
//                "step-1": { "name": "Applicant" }

//            }
//        },
//        returnTo: 0

//    },
//    {
//        index: 2,
//        key: 'step-3',
//        name: 'Assets',
//        return: {
//            "name": "Back to",
//            "items": {
//                "step-1": { "name": "Applicant" },
//                "step-2": { "name": "Dept Manager" }

//            }
//        },
//        returnTo: 0
//    },
//    {
//        index: 3,
//        key: 'step-4',
//        name: 'Factory Manager',
//        return: {
//            "name": "Back to",
//            "items": {
//                "step-1": { "name": "Applicant" },
//                "step-2": { "name": "Dept Manager" },
//                "step-3": { "name": "Assets" }
//            }
//        },
//        returnTo: 0
//    },

//    {
//        index: 4,
//        key: 'step-5',
//        name: 'General Director',
//        return: {
//            "name": "Back to",
//            "items": {
//                "step-1": { "name": "Applicant" },
//                "step-2": { "name": "Dept Manager" },
//                "step-3": { "name": "Assets" },
//                "step-4": { "name": "Factory Manager" }
//            }
//        },
//        returnTo: 0
//    },

//    {
//        index: 5,
//        key: 'step-6',
//        name: 'Purchasing Dept',
//        return: {
//            "name": "Back to",
//            "items": {
//                "step-1": { "name": "Applicant" },
//                "step-2": { "name": "Dept Manager" },
//                "step-3": { "name": "Assets" },
//                "step-4": { "name": "Factory Manager" },
//                "step-5": { "name": "General Director" }

//            }
//        },
//        returnTo: 0
//    },

//    {
//        index: 6,
//        key: 'step-7',
//        name: 'Applicant',
//        return: {
//            "name": "Back to",
//            "items": {
//                "step-1": { "name": "Applicant" },
//                "step-2": { "name": "Dept Manager" },
//                "step-3": { "name": "Assets" },
//                "step-4": { "name": "Factory Manager" },
//                "step-5": { "name": "General Director" },
//                "step-6": { "name": "Purchasing Dept" }

//            }

//        },
//        returnTo: 0
//    },

//    {
//        index: 7,
//        key: 'step-8',
//        name: 'Asset Center',
//        return: {
//            "name": "Back to",
//            "items": {
//                "step-1": { "name": "Applicant" },
//                "step-2": { "name": "Dept Manager" },
//                "step-3": { "name": "Assets" },
//                "step-4": { "name": "Factory Manager" },
//                "step-5": { "name": "General Director" },
//                "step-6": { "name": "Purchasing Dept" },
//                "step-7": { "name": "Applicant" }
//            }
//        },
//        returnTo: 0
//    }
//];
$(function () {
    var current_step = 0;
    $("#searchUser").on("keydown", function (e) {
        if (e.which == 13) {
            var value = $(this).val().toLowerCase();
            $("#listUser li").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
        }

    });
    $('#submitors').hide();

    function resetList() {
        $('#listUser .form-check-input').prop('checked', false);
        var step = users.findIndex(obj => obj.index == current_step);
        if (step >= 0) {
            var member_in_step = users[step].member;
            $("#listUser li label input").each(function (index) {
                var member = member_in_step.findIndex(obj => obj.code == this.name);
                if (member >= 0)
                    $(this).prop('checked', true);
            });
        }

    }
    $('#listUser .form-check-input').change(function () {
        var step = users.findIndex((obj => obj.index == current_step));
        var user = {
            code: this.name,
            name: $(this).attr('value')
        };
        if (this.checked) {
            if (step >= 0) {
                users[step].member.push(user);
            } else {

                step = {
                    index: current_step,
                    member: [
                        user
                    ]
                }
                users.push(step);
            }
            var memberApprove = $('<p/>', {
                text: user.name,
                id: current_step + '-' + user.code
            })
            $('#step-' + current_step).append(memberApprove)
        } else {
            if (step >= 0) {
                var new_member = $.grep(users[step].member, function (item) {
                    return item.code !== user.code;
                })
                users[step].member = new_member;
                $('#' + current_step + '-' + user.code).remove();
            }
        }
    });

    $.ajax({
        type: "GET",
        url: "/Process/LoadProcess",
        data: {
            processId: $('#process').val() },
        success: function (msg) {

            $.each(msg.data, function (index, value) {
                var items = "{"
                for (var i = 0; i < index; i++) {
                    if (i == index - 1) {
                        items += '"step-' + (i + 1) + '":{"name":"' + msg.data[i].STATION_NAME.trim() + '"}'
                    } else {
                        items += '"step-' + (i + 1) + '":{"name":"' + msg.data[i].STATION_NAME.trim() + '"},'
                    }
                }
                items += "}"
                var itemsObj = JSON.parse(items)
                var back = {
                    "name": "Back to",
                    "items": itemsObj
                }
                var obj = {
                    index: value.FORM_INDEX,
                    key: 'step-' + (value.FORM_INDEX + 1),
                    name: value.STATION_NAME,
                    return: back,
                    returnTo: value.RETURN_INDEX
                }
                listStep.push(obj)
            });
            users =  msg.stations;
            //$.each(msg.stations, function (index, value) {
                
            //    users.push(value);
            //});
            var startX = 30;
            var startY = 100;
            var pi = 15;

            $.each(listStep, function (index, value) {

                var step = makeSVG('circle', { cx: startX, cy: startY, r: 15, fill: '#43a047', stroke: 'white', 'stroke-width': 2, class: "steps-step context-menu-" + (index + 1) });
                var step_text = makeSVG('text', { x: startX - 4, y: startY + 5, fill: 'white', class: "context-menu-" + (index + 1) })
                step_text.appendChild(document.createTextNode(index + 1));
                if (index < listStep.length - 1) {
                    var line = makeSVG('line', { x1: startX + pi, y1: startY, x2: 100 + startX + pi, y2: startY, style: "stroke:rgb(141, 132, 132);stroke-width:0.5" })
                    $('svg').append(line);
                }
                var step_name = makeSVG('text', { x: startX - pi * 2, y: startY + 30, fill: 'gray' })
                step_name.appendChild(document.createTextNode(value.name));
                $('svg').append(step);
                $('svg').append(step_text);
                $('svg').append(step_name);

                startX = startX + pi + 100;
                if (value.returnTo != null)
                    drawReturnLine(index, value.returnTo);
                if (value.return != null) {
                    $.contextMenu({
                        selector: '.context-menu-' + (index + 1),
                        callback: function (key, options) {
                            var stepStart = listStep.find(x => x.index === index);
                            if (key == "exit") {
                                for (var i = 0; i < 7; i++) {
                                    $('.context-menu-' + (i + 1)).css({ stroke: "white" });
                                }
                                $('#submitors').hide();

                            } else if (key == "selectUser") {
                                //show list user
                                current_step = stepStart.index + 1;
                                resetList();
                                for (var i = 0; i < 7; i++) {
                                    $('.context-menu-' + (i + 1)).css({ stroke: "white" });
                                }
                                $('.context-menu-' + (index + 1)).css({ stroke: "#003d00" });

                                $("#submitors").fadeOut(1);
                                $("#submitors").fadeIn();
                            } else {
                                var step = listStep.find(x => x.key === key);

                                if (stepStart.returnTo != null) {
                                    removeReturnLine(stepStart.index, stepStart.returnTo);
                                }
                                stepStart.returnTo = step.index;
                                drawReturnLine(index, step.index);
                            }

                        },
                        items: {

                            "Return": value.return,
                            "selectUser": { "name": "Select approval" },
                            "exit": { "name": "Exit" }
                        }
                    });
                }

            })
        },
        error: function (msg) {
            alert('error: ' + msg.d);
        }
    });



});
function saveChange() {
    var selectedForm = $("#process").val();
    var data = JSON.stringify(listStep);
    var userJson = JSON.stringify(users);
    var base_url = window.location.origin;
    $.ajax({
        type: "POST",
        url: "/Process/GetProcess",
        data: { process: data, selectedForm: selectedForm, user: userJson },      // NOTE CHANGE HERE
        success: function (msg) {
            //alert(msg.d);
            window.location.href = base_url + '/Process';
        },
        error: function (msg) {
            alert('error: ' + msg.d);
        }

    });
}
function GoBack() {
    window.history.back();
}
function GetProcess(process) {
    alert(process.value);
}
function drawReturnLine(currentStation, returnStation) {
    var pi = 15;
    var startX = 30 + 115 * currentStation;
    var returnX = 30 + 115 * returnStation;
    var startY = 100;
    var initY = 1 + returnStation * 10;
    var color = '#111';

    var line1 = makeSVG('line', { x1: startX, y1: startY - pi, x2: startX, y2: initY, style: "stroke:" + color + ";stroke-width:0.5", class: "line-" + currentStation + "-" + returnStation })
    var line2 = makeSVG('line', { x1: startX, y1: initY, x2: returnX, y2: initY, style: "stroke:" + color + ";stroke-width:0.5", class: "line-" + currentStation + "-" + returnStation })
    var line3 = makeSVG('line', { x1: returnX, y1: initY, x2: returnX, y2: startY - pi, style: "stroke:" + color + ";stroke-width:0.5", class: "line-" + currentStation + "-" + returnStation })
    var line4 = makeSVG('line', { x1: returnX, y1: startY - pi, x2: returnX - 7, y2: startY - pi - 4, style: "stroke:" + color + ";stroke-width:0.5", class: "line-" + currentStation + "-" + returnStation })
    var line5 = makeSVG('line', { x1: returnX, y1: startY - pi, x2: returnX + 7, y2: startY - pi - 4, style: "stroke:" + color + ";stroke-width:0.5", class: "line-" + currentStation + "-" + returnStation })

    $('svg').append(line1);
    $('svg').append(line2);
    $('svg').append(line3);
    $('svg').append(line4);
    $('svg').append(line5);

}
function removeReturnLine(currentStation, returnStation) {
    $(".line-" + currentStation + "-" + returnStation).remove();
}
function makeSVG(tag, attrs) {
    var el = document.createElementNS('http://www.w3.org/2000/svg', tag);
    for (var k in attrs)
        el.setAttribute(k, attrs[k]);
    return el;
}