$(document).ready(function () {
    EventMessageBoard.Init();
});

EventMessageBoard = {
    EventId: - 1,
    Init: function () {
        console.log('evt msg board init');
        EventMessageBoard.EventId = Number($("#EventId").val());
        $("#btnCreateTopic").on("click", EventMessageBoard.EventHandlers.CreateTopicClicked);
        $("#divMessages").on("click", "a.topic-reply-show", EventMessageBoard.EventHandlers.ShowTopicReply);
        $("#divMessages").on("click", "a.topic-reply-post", EventMessageBoard.EventHandlers.PostTopicReply);
        $("#divMessages").on("click", "a.topic-reply-cancel", EventMessageBoard.EventHandlers.CancelTopicReply);
        $("#divMessages").on("click", "div.topic-header", EventMessageBoard.EventHandlers.TopicHeaderClicked);
        $("#btnPostMessage").on("click", EventMessageBoard.EventHandlers.PostMessageClicked);
        $("#divMessages").on("click", ".unread-message-subject", EventMessageBoard.EventHandlers.UnreadMessageClicked);
        $("#divMessages").on("click", ".delete-message", EventMessageBoard.EventHandlers.DeleteMessageClicked);
        $("#btnCancelMessage").on("click", EventMessageBoard.EventHandlers.CancelMessageClicked);
        $("#btnShowMessageDiv").on("click", EventMessageBoard.EventHandlers.ShowMessagesClicked);
    },
    EventHandlers: {
        CreateTopicClicked: function () {
            console.log('create topic clicked');
            var subject = $("#txtMessageSubject").val();
            var text = $("#txtMessageText").val();
            var eid = Number($("#EventId").val());
            var obj = {
                EventId: eid,
                Subject: subject,
                Text: text,
                ThreadId: -1
            };
            Loading.Start();
            $.ajax({
                url: "/Messages/Create",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: "html",
                data: JSON.stringify(obj),
                success: function success(results) {
                    Loading.Stop();
                    $("#divMessages").html(results);
                    $("#divCreateMessage").hide();
                    $("#btnShowMessageDiv").show();
                },
                error: function error(err) {
                    Loading.Stop();
                    alert('error');
                }
            });
        },
        TopicHeaderClicked: function () {
            console.log('topic header clicked');
            var $topicContainer = $(this).closest('div[data-topic-id]').find('div.topic-messages-container');
            if ($topicContainer.is(':visible')) $topicContainer.hide();
            else $topicContainer.show();

        },
        ShowTopicReply: function () {
            console.log('show topic reply clicked');
            var topicId = $(this).attr('data-topic-id');
            $(this).hide();
            $('div.topic-reply[data-topic-id=' + topicId + ']').show();
        },
        PostTopicReply: function () {
            console.log('post topic reply clicked');
            var txt = $(this).closest('div.topic-reply').find('textarea').val();
            var topicId = $(this).closest('div.topic-reply').attr('data-topic-id');


            //var subject = $("#txtMessageSubject").val();
            //var text = $("#txtMessageText").val();
            var eid = Number($("#EventId").val());
            var obj = {
                EventId: eid,
                ThreadId: topicId,
                Text: txt
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
                    Loading.Stop();
                    $("#divMessages").html(results);
                    $("#divCreateMessage").hide();
                    $("#btnShowMessageDiv").show();
                },
                error: function error(err) {
                    Loading.Stop();
                    alert('error');
                }
            });
        },
        CancelTopicReply: function () {
            console.log('cancel topic reply clicked');
            var topicId = $(this).closest('div.topic-reply').attr('data-topic-id');
            $(this).closest('div.topic-reply').hide();
            $('a.topic-reply-show[data-topic-id=' + topicId + ']').show();

        },
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
            Loading.Start();
            $.ajax({
                url: "/Messages/Create",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: "html",
                data: JSON.stringify(obj),
                success: function success(results) {
                    //Loading.Stop();
                    $("#divMessages").html(results);
                    $("#divCreateMessage").hide();
                    $("#btnShowMessageDiv").show();
                },
                error: function error(err) {
                    Loading.Stop();
                    alert('error');
                }
            });
        }
    }
};