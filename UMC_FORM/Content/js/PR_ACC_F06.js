
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

var current_step = 0;
$(function () {
    $('#process').keyup(function (e) {
        if (e.keyCode == 13) {
            $.ajax({
                type: "GET",
                url: "/Process/CheckExist",
                data: {
                    processId: $(this).val()
                },
                success: function (msg) {
                    if (msg.result == 'error') {
                        alert(msg.message)
                        $('#process').focus()
                    }
                },
                error: function (msg) {
                    alert('error: ' + msg.d);
                }
            });
        }


    })
    $("#searchUser").on("keydown", function (e) {
        if (e.which == 13) {
            var value = $(this).val().toLowerCase();
            $("#listUser li").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
        }

    });
    $('#submitors').hide();
    $('#station_no').keydown(function () {
        var val = $('#station_no').val()
        var label = $('#list-station-no option').filter(function () {
            return this.value == val;
        }).attr('name');
        $('#station_name').val(label)
    })

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
    var processId = $('#process').val();
    if (processId != null && processId != '') {
        $.ajax({
            type: "GET",
            url: "/Process/LoadProcess",
            data: {
                processId: processId
            },
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
                        no: value.STATION_NO,
                        return: back,
                        returnTo: value.RETURN_INDEX,
                        rejectList: msg.reject.filter(function (m) {
                            return m.START_INDEX == (value.FORM_INDEX - 1)
                        }),
                        permission: msg.permission.filter(function (m) {
                            return m.ITEM_COLUMN_PERMISSION == (value.FORM_INDEX)
                        })
                    }
                    listStep.push(obj)
                });
                users = msg.stations;
                //$.each(msg.stations, function (index, value) {

                //    users.push(value);
                //});

                drawStation()


            },
            error: function (msg) {
                alert('error: ' + msg.d);
            }
        });


    } else {

        listStep = [

            {
                index: 0,
                key: 'step-1',
                name: 'Applicant',
                no: 'APPLICANT',
                return: {

                }

            }]
        drawStation()
    }

    $('#btn-add-reject-station').click(function (e) {

        var li = $('<li />', {
            text: $('#listStation option:selected').val(),
            class: 'list-group-item',
            name: $('#listStation option:selected').attr('name')
        })

        $('#listStationApproveAfterReject').append(li)
    })
    $('#btn-delete-reject-station').click(function (e) {
        $('#listStationApproveAfterReject').empty()
    })

    $('#btn-add-permission').click(function () {
        if ($('#permission').val().indexOf(" ") !== -1) {
            alert("PERMISSION không được có dấu cách")
            $('#permission').focus()
            return;
        }
        addPermissionRow($('#permission').val(), $('#dept-permission').val())
    })
});
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

function addPermissionRow(per, dept) {
    var tr = $('<tr />')
    var td = $('<td />', {
        text: per,
        class: "per"
    })
    var td1 = $('<td/>', {
        text: dept,
        class: "dept"
    })
    tr.append(td)
    tr.append(td1)
    var td2 = $('<td/>')
    var btnDelete = $('<button />', {
        class: "border-0 bg-white btn-delete-permission",
        click: function () {
            $(this).closest('tr').remove();
        }
    })
    var i = $('<i />', {
        class: "fas fa-minus-circle text-danger"
    })
    btnDelete.append(i)
    td2.append(btnDelete)
    tr.append(td2)
    $('#listPermissionSelected').append(tr)
}
function drawPermissionList(current_step) {
    $('#listPermissionSelected').empty()
    var step = listStep.find(m => m.index == current_step);
    if (step.permission != null) {
        $.each(step.permission, function (index, value) {
            addPermissionRow(value.ITEM_COLUMN, value.DEPT == '' ? 'None' : value.DEPT)
        })
    }
}
function drawStation() {
    var startX = 30;
    var startY = 100;
    var pi = 15;
    $('#listStation').empty()
  
    $.each(listStep, function (index, value) {
        var option = $('<option />', {
            name: value.index - 1,
            value: value.name,
            text: value.name
        })
        $('#listStation').append(option)
    
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
                    } else if (key == "Return") {
                        var step = listStep.find(x => x.key === key);

                        if (stepStart.returnTo != null) {
                            removeReturnLine(stepStart.index, stepStart.returnTo);
                        }
                        stepStart.returnTo = step.index;
                        drawReturnLine(index, step.index);
                    } else if (key == "nextStation") {
                        current_step = stepStart.index;
                        $('#newStationModal').modal('show')
                    } else if (key == "deleteStation") {
                        current_step = stepStart.index;
                        deleteStation();
                    } else if (key == "rejectStep") {
                        current_step = stepStart.index;
                        $('#listStationApproveAfterReject').empty()
                        drawRejectList(current_step);
                        $('#rejectStationModal').modal('show')
                    } else if (key == 'formPermission') {
                        current_step = stepStart.index;
                        drawPermissionList(value.index)
                        $('#formPermission').modal('show')
                    }

                },
                items: {

                    "Return": value.return,
                    "selectUser": { "name": "Select approval" },
                    "nextStation": { "name": "Add New Station" },
                    "deleteStation": { "name": "Delete Station" },
                    "rejectStep": { "name": "Select Step after Reject" },
                    "formPermission": { "name": "Form Permission" },
                    "exit": { "name": "Exit" }
                }
            });
        }

    })

  
}
function drawRejectList(current_step) {
    var step = listStep.find(m => m.index == current_step);
    if (step.rejectList != null) {
        $.each(step.rejectList, function (index, value) {
            var s = listStep.find(m => m.index == (parseInt(value.FORM_INDEX) + 1));
            var li = $('<li />', {
                text: s.name,
                class: 'list-group-item',
                name: value.FORM_INDEX
            })

            $('#listStationApproveAfterReject').append(li)
        })
    }
}
function savePermission() {
  
    var permission = []
    $('#listPermissionSelected tr').each(function (index, value) {
        var per = ''
        var dept = ''
        $(this).find('td').each(function (i,v) {
            if (i == 0) per = $(this).text()
            if (i == 1) dept = $(this).text()
        }); 
        
        var obj = {
            ITEM_COLUMN: per,
            ITEM_COLUMN_PERMISSION: current_step,
            DEPT:dept == 'None' ? '' : dept
        }
        permission.push(obj)
    })
    var tempListStep = [];
    $.each(listStep, function (index, value) {
        if (value.index == current_step) {
            value.permission = permission
        }
        tempListStep.push(value)

    })
    listStep = tempListStep
    $('#formPermission').modal('hide')
}
function saveRejectStep() {
    var reject = []

    $('#listStationApproveAfterReject li').each(function (index, value) {
        var obj = {
            FORM_INDEX: $(this).attr('name'),
            START_INDEX: current_step - 1
        }
        reject.push(obj)
    })
    var tempListStep = [];
    $.each(listStep, function (index, value) {
        if (value.index == current_step) {
            value.rejectList = reject
        }
        tempListStep.push(value)

    })
    listStep = tempListStep
    $('#rejectStationModal').modal('hide')
}
function deleteStation() {
    listStep = listStep.filter(function (obj) {
        return obj.index !== current_step;
    });
    users = users.filter(function (obj) {
        return obj.index !== current_step;
    })
    var tempListStep = [];
    $.each(listStep, function (index, value) {
        if (value.index > current_step) {
            value.index = value.index - 1;
            value.key = 'key-' + (value.index + 1)

        }
        tempListStep.push(value)

    })
    listStep = tempListStep;
    var tempListUser = []
    $.each(users, function (index, value) {
        if (value.index > current_step) {
            value.index = value.index - 1
        }
        tempListUser.push(value)
    })
    users = tempListUser
    $('.steps-form').empty()
    drawStation()
}
function saveChange() {
    var selectedForm = $("#process").val();
    if (selectedForm == '') {
        alert('Bạn cần nhập tên cho Process')
        return;
    }
    if (selectedForm.indexOf(" ") !== -1) {
        alert("Tên Process không được có dấu cách");
        return;
    }
    var data = JSON.stringify(listStep);
    var userJson = JSON.stringify(users);
    var base_url = window.location.origin;
    var state = ''
    if ($('#process').attr('name') == 'new') {
        state = 'new'
    } else {
        state = 'old'
    }
    $.ajax({
        type: "POST",
        url: "/Process/GetProcess",
        data: { process: data, selectedForm: selectedForm, user: userJson, state: state },      // NOTE CHANGE HERE
        success: function (msg) {
            if (msg.body == 'error') {
                alert(msg.message)
                return;
            }
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

    $('.steps-form').append(line1);
    $('.steps-form').append(line2);
    $('.steps-form').append(line3);
    $('.steps-form').append(line4);
    $('.steps-form').append(line5);

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

function saveNewStation() {
    var station_name = $('#station_name').val()
    var station_no = $('#station_no').val()
    if (station_name == '' || station_no == '') {
        alert('Bạn cần phải điền đầy đủ STATION NAME và STATION NO')
        return;
    }
    var station = listStep.find(x => x.name == station_name.trim() && x.no == station_no.trim());
    if (station != null) {
        alert('Đã có trạm này trong process rồi!')
        return;
    }
    if (station_no.indexOf(" ") !== -1) {
        alert("STATION NO không được có dấu cách");
        return;
    }
    var tempListStep = [];
    $.each(listStep, function (index, value) {
        if (index <= current_step) {
            tempListStep.push(value)
        }
    })

    var items = "{"
    $.each(listStep, function (index, value) {
        if (index <= current_step) {
            if (index == current_step) {
                items += '"step-' + (index + 1) + '":{"name":"' + value.name.trim() + '"}'
            } else {
                items += '"step-' + (index + 1) + '":{"name":"' + value.name.trim() + '"},'
            }
        }

    })

    items += "}"
    var itemsObj = JSON.parse(items)
    var newStep = {
        index: current_step + 1,
        key: 'step-' + (current_step + 2),
        name: station_name.trim(),
        no: station_no.trim(),
        return: {
            "name": "Back to",
            "items": itemsObj
        },
        returnTo: 0
    }
    tempListStep.push(newStep)

    $.each(listStep, function (index, value) {
        if (index > current_step) {
            var returnTo = value.returnTo
            if (value.RETURN_INDEX > current_step) {
                returnTo += 1;
            }
            var obj = {
                index: value.index + 1,
                key: 'step-' + (value.index + 2),
                name: value.name,
                no: value.no,
                return: value.return,
                returnTo: returnTo
            }
            tempListStep.push(obj)
        }
    })

    listStep = tempListStep;
    $('.steps-form').empty()
    drawStation()
    var option = $('<option />', {
        name: station_name.trim(),
        value: station_no.trim()
    })
    $('#list-station-no').append(option)
    $('#station_no').val('')
    $('#station_name').val('')
    $('#newStationModal').modal('hide')
}