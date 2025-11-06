var routeURL = location.protocol + "//" + location.host;
$(document).ready(function () {
    InitializeCanlendar();
});
function InitializeCanlendar() {
    try {
        let date=new Date();
        $('#calendar').fullCalendar({
            timezone: false,
            header:
            {
                left: 'prev,next,today',
                center: 'title',
                right: 'month,agendaWeek,agendaDay'
            },
            views: {
                week: {
                    titleFormat: 'D MMMM YYYY',
                    titleRangeSeparator: ' - ',
                }
            },
            selectable: true,
            editable: false,
            select: function (event) {
                onShowModal(event, null);
            },
            events: function (start, end, timezone, callback) {
                debugger;
                var doctorId = $('#doctorCId').val();//document.getElementById("doctorId").selectedValue;
                $.ajax({
                    url: '/Appointments/GetCalendar?doctorId=' + doctorId,
                    dataType: 'JSON',
                    type:'GET',
                    contentType: 'application/json;',
                    success: function (res) {
                        var events = [];
                        $.each(res, function (i, data) {
                            events.push({
                                title: data.patientName,
                                doctorId: data.doctorId,
                                patientId: data.patientId,
                                start: data.startDate,
                                end: data.endDate,
                                startDate: data.startDate,
                                endDate: data.endDate,
                                notes: data.notes,
                                status: data.status,
                                appointmentTypeId: data.appointmentTypeId,
                                id:data.id
                               
                            });
                        });
                        callback(events);
                    }
                    
                });
            },
            eventClick: function (info) {
                getEventDetailByEventId(info);
            }
            
        });
       
    }
    catch (e) {
        alert(e);
    }
}

function onShowModal(obj, isEventDetail) {
    if (isEventDetail != null) {

        $("#doctorId").select2("val", String(obj.doctorId));
        $("#patientId").select2("val", String(obj.patientId));
        $("#appointmentTypeId").select2("val", String(obj.appointmentTypeId));
        $("#statusId").select2("val", String(obj.status));
        document.getElementById("startDate").value = obj.startDate;
        document.getElementById("endDate").value = obj.endDate;
        var note = document.getElementById('note');
        note.value = obj.notes;
        //document.getElementById("note").innerHTML = obj.notes;
        document.getElementById("id").value = obj.id;
        document.getElementById("branchId").value = obj.branchId;
      
    }
    if (isEventDetail == null) {
        var currentDate = new Date();
        $("#doctorId").select2("val", "0");
        $("#patientId").select2("val", "0");
        $("#appointmentTypeId").select2("val", "0");
        $("#statusId").select2("val", "0");
      //  var sd = String(currentDate.getFullYear()) + "-" + String(currentDate.getMonth()) + "-" + String(currentDate.getDay()) + " :" + String(currentDate.getHours()) + ":" + String(currentDate.getMinutes()) + ":" + String(currentDate.getSeconds()); 
        var note = document.getElementById('note');
        note.value = "";
        document.getElementById("id").value = 0;
        document.getElementById("branchId").value = 0;
        
    }
    $("#appointmentInput").modal("show");
}
function onCloseModal() {
   
    $("#appointmentInput").modal("hide");
}
function getEventDetailByEventId(x) {
    $.ajax({
        url: '/Appointments/GetEventById?id=' + x.id,
        type: 'GET',
        dataType:'JSON',
        contentType: 'application/json;',
        success: function (res) {
            if (res != undefined) {
                onShowModal(res, true);
            }
            
           
        }

    });
}
function onChangeDoctor() {
    $('#calendar').fullCalendar('refetchEvents');
}
function ValidTime() {
    
   var startDate= document.getElementById("startDate").value;
    var endDate = document.getElementById("endDate").value;
    $.ajax({
        url: '/Appointments/GetAppointmentByDateTime?startDate=' + startDate + '&endDate=' + endDate,
        type: 'GET',
        dataType: 'JSON',
        contentType: 'application/json;',
        success: function (res) {
            if (res.length != 0) {
                document.getElementById("btnSave").disabled = true;
                alert("You can't appointment this time");

            }
            else {
                document.getElementById("btnSave").disabled = false;
            }


        }
    });
}
