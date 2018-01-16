$(document).ready(function () {
    EditEventNotifications.Init();
});

EditEventNotifications = {
    Init: function () {
        $("#divNotifications").on("click", ".delete-notification", EditEventNotifications.EventHandlers.DeleteNotificationClicked);
        $("#btnPostNotifcation").on("click", EditEventNotifications.EventHandlers.PostNotificationClicked);
        $("#btnCancelNotifcation").on("click", EditEventNotifications.EventHandlers.CancelNotificationClicked);
        $("#btnShowNotificationDiv").on("click", EditEventNotifications.EventHandlers.ShowNotifications);
    },
    EventHandlers: {
        ShowNotifications: function () {
            console.log('showing notifications');
            $("#divCreateNotification").show();
            $("#txtNotificationSubject").val('');
            $("#txtNotificationText").val('');
            $(this).hide();
        },
        CancelNotificationClicked: function () {
            $("#divCreateNotification").hide();
            $("#btnShowNotificationDiv").show();
        },
        PostNotificationClicked: function () {
            var subject = $("#txtNotificationSubject").val();
            var text = $("#txtNotificationText").val();
            var eid = Number($("#EventId").val());
            var obj = {
                EventId: eid,
                Subject: subject,
                Text: text
            };
            Loading.Start();
            $.ajax({
                url: "/Notifications/Create",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: "html",
                data: JSON.stringify(obj),
                success: function success(results) {
                    Loading.Stop();
                    $("#divNotifications").html(results);
                    $("#divCreateNotification").hide();
                    $("#btnShowNotificationDiv").show();
                },
                error: function error(err) {
                    alert('error');
                    Loading.Stop();
                }
            });
        },
        DeleteNotificationClicked: function () {
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
                    alert('error');
                }
            });
        }
    }
};