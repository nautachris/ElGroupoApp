$(document).ready(function () {
    EventMessageBoard.Init();
});

EventMessageBoard = {
    EventId: - 1,
    Init: function () {
        EventMessageBoard.EventId = Number($("#EventId").val());
        $("#btnPostMessage").on("click", EventMessageBoard.EventHandlers.PostMessageClicked);
        $("#divMessages").on("click", ".unread-message-subject", EventMessageBoard.EventHandlers.UnreadMessageClicked);
        $("#divMessages").on("click", ".delete-message", EventMessageBoard.EventHandlers.DeleteMessageClicked);
        $("#btnCancelMessage").on("click", EventMessageBoard.EventHandlers.CancelMessageClicked);
        $("#btnShowMessageDiv").on("click", EventMessageBoard.EventHandlers.ShowMessagesClicked);
    },
    EventHandlers: {
        ShowMessagesClicked: function () {
            $("#divCreateMessage").show();
            $(this).hide();
        },
        CancelMessageClicked: function () {
            $("#divCreateMessage").hide();
            $("#btnShowMessageDiv").show();
        },
        DeleteMessageClicked: function () {
            var messageId = $(this).attr('data-message-id');
            $.ajax({
                url: "/Messages/Delete/" + messageId,
                type: 'DELETE',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: "html",
                data: JSON.stringify(obj),
                success: function success(results) {

                    $("#divMessages").html(results);
                },
                error: function error(err) {
                    alert('error');
                }
            });
        },
        UnreadMessageClicked: function () {
            var messageId = Number($(this).attr('data-message-id'));
            $('.message-container[data-message-id=' + messageId.toString() + ']').show();

            $.ajax({
                url: "/Messages/SetAsViewed/" + messageId.toString(),
                type: 'GET',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: 'json',
                success: function success(results) {
                    $('.unread-message-subject[data-message-id=' + messageId.toString() + ']').removeClass('unread-message-subject');
                },
                error: function error(err) {
                    alert('error');
                }
            });

        },
        PostMessageClicked: function () {
            var subject = $("#txtMessageSubject").val();
            var text = $("#txtMessageText").val();
            var eid = Number($("#EventId").val());
            var obj = {
                EventId: eid,
                Subject: subject,
                Text: text
            };
            $.ajax({
                url: "/Messages/Create",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: "html",
                data: JSON.stringify(obj),
                success: function success(results) {
                    $("#divMessages").html(results);
                    $("#divCreateMessage").hide();
                    $("#btnShowMessageDiv").show();
                },
                error: function error(err) {
                    alert('error');
                }
            });
        }
    }
};