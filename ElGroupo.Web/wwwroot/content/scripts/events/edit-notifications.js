$(document).ready(function () {
    $("#btnShowNotificationDiv").on("click", function () {
        $("#divCreateNotification").show();
        $("#txtNotificationSubject").val('');
        $("#txtNotificationText").val('');
        $(this).hide();

    });
    $("#btnCancelNotifcation").on("click", function () {
        $("#divCreateNotification").hide();
        $("#btnShowNotificationDiv").show();
    });

    $("#btnPostNotifcation").on("click", function () {
        var subject = $("#txtNotificationSubject").val();
        var text = $("#txtNotificationText").val();
        var eid = Number($("#EventId").val());
        var obj = {
            EventId: eid,
            Subject: subject,
            Text: text
        };
        $.ajax({
            url: "/Notifications/Create",
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            async: true,
            cache: false,
            dataType: "html",
            data: JSON.stringify(obj),
            success: function success(results) {
                $("#divNotifications").html(results);
                $("#divCreateNotification").hide();
                $("#btnShowNotificationDiv").show();
            },
            error: function error(err) {
                alert('fuck me');
            }
        });
    });

    //this is for deleting from everybody
    $("#divNotifications").on("click", ".delete-notification", function () {
        var notificationId = $(this).attr('data-notification-id');
        $.ajax({
            url: "/Notifications/Delete/" + messageId,
            type: 'DELETE',
            contentType: "application/json; charset=utf-8",
            async: true,
            cache: false,
            dataType: "html",
            data: JSON.stringify(obj),
            success: function success(results) {

                $("#divNotifications").html(results);
            },
            error: function error(err) {
                alert('fuck me');
            }
        });
    });
});