$(document).ready(function () {
    EventCheckIn.Init();
});


EventCheckIn = {
    Init: function () {
        EventCheckIn.EventId = Number($("#EventId").val());
        EventCheckIn.DistanceTolerance = Number($("#DistanceTolerance").val());
        EventCheckIn.AllowLocationCheckin = $("#CheckInMethod").val() === 'PasswordAndLocation';
        EventCheckIn.EventLocationX = Number($("#EventCoordX").val());
        EventCheckIn.EventLocationY = Number($("#EventCoordY").val());
        $("div.current-location").on("click", EventCheckIn.EventHandlers.LocationClicked);
        $("div.event-password").on("click", EventCheckIn.EventHandlers.PasswordClicked);
        $("#btnCheckIn").on("click", EventCheckIn.EventHandlers.SubmitCheckIn);
        if (EventCheckIn.AllowLocationCheckin) {
            EventCheckIn.InitGeolocation();
            EventCheckIn.EventHandlers.LocationClicked();
        }
        else {
            EventCheckIn.EventHandlers.PasswordClicked();
        }
    },
    EventId: -1,
    DistanceTolerance: -1,
    AllowLocationCheckin: false,
    EventLocationX: -1,
    EventLocationY: -1,
    LocationWatchId: -1,
    InitGeolocation: function () {
        if (navigator.geolocation) {
            EventCheckIn.LocationWatchId = navigator.geolocation.watchPosition(EventCheckIn.PositionChanged);
        }
        else {
            $("#divLocationMessage").text('Location is not working in your browser!');
        }
    },
    CheckInWithCoordinates: function (lat, lon) {
        var model = { EventId: EventCheckIn.EventId, CoordinateX: lon, CoordinateY: lat };
        $.ajax({
            url: "/Events/CheckInLocation",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            async: true,
            cache: false,
            success: function success(results) {
                if (results.success === true) {
                    alert('checkin succeeded');
                    $("#divCheckinStatus").show().text('You are now checked in!');
                }
                else {
                    alert('checkin failed - ' + results.message);
                    $("#divCheckinStatus").show().text('Check in failed - ' + results.message);
                }

            },
            error: function error(err) {
                alert('error');
            }
        });
    },
    CheckInWithPassword: function (pw) {
        var model = { EventId: EventCheckIn.EventId, Password: pw };
        $.ajax({
            url: "/Events/CheckInPassword",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            async: true,
            cache: false,
            //dataType: 'application/json',
            success: function success(results) {
                console.log(results);
                if (results.success === true) {
                    alert('checkin succeeded');
                    $("#divCheckinStatus").show().text('You are now checked in!');
                }
                else {
                    alert('checkin failed - ' + results.message);
                    $("#divCheckinStatus").show().text('Check in failed - ' + results.message);
                }

            },
            error: function error(err) {
                alert('error');
            }
        });
    },
    EventHandlers: {
        LocationClicked: function () {
            $(this).css('font-weight', 'bold');
            $("div.event-password").css('font-weight', 'normal');
            $("#btnCheckIn").attr('data-checkin-type', 'location');
            $("#divLocation").show();
            $("#divPassword").hide();
        },
        PasswordClicked: function () {
            $(this).css('font-weight', 'bold');
            $("div.current-location").css('font-weight', 'normal');
            $("#btnCheckIn").attr('data-checkin-type', 'password');
            $("#divLocation").hide();
            $("#divPassword").show();
        },
        SubmitCheckIn: function () {
            console.log('submit checkin - ' + $(this).attr('data-checkin-type'));
            if ($(this).attr('data-checkin-type') === 'location') {
                if (navigator.geolocation) {
                    console.log('navigator.geolocation');
                    navigator.geolocation.clearWatch(EventCheckIn.LocationWatchId);
                    navigator.geolocation.getCurrentPosition(function (position) {
                        EventCheckIn.LocationWatchId = navigator.geolocation.watchPosition(EventCheckIn.PositionChanged);
                        var ddist = EventCheckIn.GetDistance(position.coords.latitude, position.coords.longitude);
                        if (ddist > EventCheckIn.DistanceTolerance) {
                            alert('You are ' + ddist.toFixed(2) + ' feet away from the event location.  The maximum distance offset is ' + EventCheckIn.DistanceTolerance + ' feet.  Please check in using the event password.');
                            return false;
                        }
                        EventCheckIn.CheckInWithCoordinates(position.coords.latitude, position.coords.longitude);
                    }, function (error) {
                        console.log('geolocation error');
                        console.log(error);
                    }, function (options) {

                    });
                }
                else {
                    alert('location is not available - please check in using the event password');
                    return false;
                }
            }
            else {
                if ($("#UserPassword").val() === '') {
                    alert('password required');
                    return false;
                }
                EventCheckIn.CheckInWithPassword($("#UserPassword").val());
            }

        }
    },
    PositionChanged: function (position) {
        var ddist = EventCheckIn.GetDistance(position.coords.latitude, position.coords.longitude);
        $("#spanDistanceToEvent").text(ddist.toFixed(2));
        $("#spanLatHemisphere").text(position.coords.latitude > 0 ? 'N' : 'S');
        $("#spanLonHemisphere").text(position.coords.longitude > 0 ? 'E' : 'W');
        $("#spanLatDegree").text(position.coords.latitude.toFixed(5));
        $("#spanLonDegree").text(position.coords.longitude.toFixed(5));
    },
    GetDistance: function (lat1, lon1) {
        var lat2 = EventCheckIn.EventLocationY;
        var lon2 = EventCheckIn.EventLocationX;

        var radlat1 = Math.PI * lat1 / 180;
        var radlat2 = Math.PI * lat2 / 180;
        var radlon1 = Math.PI * lon1 / 180;
        var radlon2 = Math.PI * lon2 / 180;
        var theta = lon1 - lon2;
        var radtheta = Math.PI * theta / 180;
        var dist = Math.sin(radlat1) * Math.sin(radlat2) + Math.cos(radlat1) * Math.cos(radlat2) * Math.cos(radtheta);
        dist = Math.acos(dist);
        dist = dist * 180 / Math.PI;
        dist = dist * 60 * 1.1515;
        dist = dist * 5280;
        return dist;
    }
};