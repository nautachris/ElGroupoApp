$(document).ready(function () {
    Dashboard.Init();

});
Dashboard = {
    EventLinkClick: function (evt) {
        console.log('data-event-link click');
        console.log(evt);
        var url = $(this).attr('data-event-link');
        console.log('data-event-link click' + url);
        window.location.href = url;
    },
    QuickResponseClick: function (evt) {
        evt.stopPropagation();

        console.log(evt);
        console.log('quick response clicked');
        var eventName = $(this).closest('div.dashboard-event-item').find('#spanEventName').text();
        var eid = Number($(this).closest('div.dashboard-event-item').attr('data-event-id'));
        var responseType = null;
        switch ($(this).attr('data-response-type')) {
            case 'yes':
                responseType = 'Yes';
                break;
            case 'no':
                responseType = 'No';
                break;
            case 'maybe':
                responseType = 'Maybe';
                break;
        }


        Confirm('Do you want to change your response to ' + responseType + ' for ' + eventName + '?', function () {
            //Ajax.Post("/Events/EditDetails", saveObj).done(EditEvent.SaveDetailsComplete);
            var obj = { EventId: eid, Status: responseType, UpdateRecurring: false };


            $.ajax({
                url: "/Events/UpdateRSVPStatusFromDashboard",
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                data: JSON.stringify(obj),
                dataType: 'html',
                success: function success(results) {
                    $('div.dashboard-event-item[data-event-id=' + eid + ']').html(results);
                    //need to add one-off handlers b/c of the issue with event delegation and bubbling
                    $('div.dashboard-event-item[data-event-id=' + eid + '] a.quick-response').on("click", Dashboard.QuickResponseClick);
                    $('div.dashboard-event-item[data-event-id=' + eid + ']').on("click", Dashboard.EventLinkClick);
                },
                error: function error(err) {
                    alert('error');
                }
            });
            //Ajax.Post('/Events/UpdateRSVPStatusFromDashboard', obj).done(function (results) {
            //    $('div.dashboard-event-item[data-event-id=' + eid + ']').html(results);
            //});
        });
    },
    Init: function () {
        //link to event
        $("a.quick-response").on("click", Dashboard.QuickResponseClick);
        //$("ul").on("click", "a.quick-response", function (evt) {


        //});

        $("div [data-event-link]").on("click", Dashboard.EventLinkClick);

    },
    EventHandlers: {

    }
};