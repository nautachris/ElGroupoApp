$(document).ready(function(){
    ViewEventNotifications.Init();
});

ViewEventNotifications = {
    Init: function () {
        $("#divNotifications").on("click", ".unread-notification-subject", ViewEventNotifications.EventHandlers.UnreadNotificationClicked);
    },
    EventHandlers: {
        UnreadNotificationClicked: function () {
            var notificationId = Number($(this).attr('data-notification-id'));
            $('.notification-container[data-notification-id=' + notificationId.toString() + ']').show();

            $.ajax({
                url: "/Notifications/SetAsViewed/" + notificationId.toString(),
                type: 'GET',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: 'json',
                success: function success(results) {
                    $('.unread-notification-subject[data-notification-id=' + notificationId.toString() + ']').removeClass('unread-notification-subject');
                },
                error: function error(err) {
                    alert('error');
                }
            });
        }
    }
};