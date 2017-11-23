﻿$(document).ready(function () {
    ViewEvent.Init();
});

ViewEvent = {
    Init: function () {
        $("div.rsvp-required span").on("click", ViewEvent.EventHandlers.RSVPClicked);
        //$("#divMessages").on("click", ".delete-message", ViewEvent.EventHandlers.DeleteMessageClicked);
        //$("#btnPostMessage").on("click", ViewEvent.EventHandlers.PostMessageClicked);
        //$("#divMessages").on("click", ".unread-message-subject", ViewEvent.EventHandlers.UnreadMessageClicked);
        //$("#btnCancelMessage").on("click", ViewEvent.EventHandlers.CancelMessageClicked);
        //$("#btnShowMessageDiv").on("click", ViewEvent.EventHandlers.ShowMessagesClicked);
    },
    EventHandlers: {
        RSVPClicked: function () {
            console.log('rsvp changed!');
            var newStatus = $(this).attr('data-replace-val');
            var oldStatus = $("#OriginalStatus").val();
            if (newStatus === oldStatus) return false;
            Confirm("Do you want to change your RSVP status from " + oldStatus + " to " + newStatus + "?", function () {
                var model = {
                    Status: $("#Status").val(),
                    EventId: $("#EventId").val()
                };
                if ($("#IsRecurring").val() == "True") {
                    Confirm("Do you want to change your RSVP status for all other associated events?", function () {
                        model.UpdateRecurring = true;
                        $.ajax({
                            url: "/Events/UpdateRSVPStatus",
                            type: 'POST',
                            contentType: "application/json; charset=utf-8",
                            async: true,
                            cache: false,
                            //dataType: "application/json",
                            data: JSON.stringify(model),
                            success: function success() {
                                console.log('success');
                                //$("#divMessages").html(results);
                            },
                            error: function error(err) {
                                console.log('error in saving response');
                                console.log(err);
                                alert('error');
                            }
                        });
                    }, function () {
                        model.UpdateRecurring = false;
                        $.ajax({
                            url: "/Events/UpdateRSVPStatus",
                            type: 'POST',
                            contentType: "application/json; charset=utf-8",
                            async: true,
                            cache: false,
                            //dataType: "application/json",
                            data: JSON.stringify(model),
                            success: function success() {
                                console.log('success');
                                //$("#divMessages").html(results);
                            },
                            error: function error(err) {
                                console.log('error in saving response');
                                console.log(err);
                                alert('error');
                            }
                        });
                    });
                }
                else {
                    model.UpdateRecurring = false;
                    $.ajax({
                        url: "/Events/UpdateRSVPStatus",
                        type: 'POST',
                        contentType: "application/json; charset=utf-8",
                        async: true,
                        cache: false,
                        //dataType: "application/json",
                        data: JSON.stringify(model),
                        success: function success() {
                            console.log('success');
                            //$("#divMessages").html(results);
                        },
                        error: function error(err) {
                            console.log('error in saving response');
                            console.log(err);
                            alert('error');
                        }
                    });
                }
                console.log(model);

            });
        },
        ShowMessagesClicked: function () {
            $("#divCreateMessage").show();
            $(this).hide();
        },
        CancelMessageClicked: function () {
            $("#divCreateMessage").hide();
            $("#btnShowMessageDiv").show();
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
    },
    IsAddress: false,
    MapLoaded: function () {
        var gpid = $("#GooglePlaceId").val();
        if (gpid === '') return false;
        Maps.PlaceSearch(gpid, function (place, status) {
            console.log(place);
            var isAddress = false;
            for (var x = 0; x < place.types.length; x++) {
                if (place.types[x] === 'street_address') {
                    $(".row.business-name").hide();
                    break;
                }
                if (place.types[x] === 'establishment') {
                    $(".row.business-name").show();
                    $("#spanBusinessName").val(place.name);
                    break;
                }
            }


            Maps.SetInfoWindow(place);

        });
    }
};