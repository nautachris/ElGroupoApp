$(document).ready(function () {

    //navigation
    $(".links").on("click", function () {
        $(".links").removeClass('bold');
        $(this).addClass('bold');
        $(".row.tab").hide();
        $(".row." + $(this).attr('data-link-type')).show();

    });

    $("html").on("click", ".switch-container > div", function () {
        $(this).closest("div.switch-container").find(".switch-selected").removeClass("switch-selected");
        $(this).addClass("switch-selected");
    });
    $("html").on("click", ".switch-container > span", function () {
        $(this).closest("div.switch-container").find(".switch-selected").removeClass("switch-selected");
        $(this).addClass("switch-selected");
    });

    //details



    //attendees



    //notifications



    //messages
    $("#btnShowMessageDiv").on("click", function () {
        $("#divCreateMessage").show();
        $(this).hide();

    });
    $("#btnCancelMessage").on("click", function () {
        $("#divCreateMessage").hide();
        $("#btnShowMessageDiv").show();
    });
    $("#divMessages").on("click", ".delete-message", function () {
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
                alert('fuck me');
            }
        });
    });
    $("#divMessages").on("click", ".unread-message-subject", function () {
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
                alert('fuck me');
            }
        });
        //update "read" property of message
    });
    $("#btnPostMessage").on("click", function () {
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
                alert('fuck me');
            }
        });
    });

    //notifications




    //messages


    //toggle display add message div
    //handle submit message button
    //handle message grid functions
    //delete (admin), view for first time, hide?
});

ViewEvent = {
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