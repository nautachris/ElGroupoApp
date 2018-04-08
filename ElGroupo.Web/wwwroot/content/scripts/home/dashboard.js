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
        
        $("html #btnDeleteEvent").on("click", function () {

            var $this = $(this);
            var tabType = $(this).closest('div[data-tab-type]').attr('data-tab-type');
            var prefix = 'div[data-tab-type=' + tabType + '] ';
            var $tab = $(prefix);
            var eventIds = [];
            $(prefix + ' :checkbox:checked').each(function () {
                eventIds.push($(this).closest('div.dashboard-event-item').attr('data-event-attendee-id'));
                //delete-recurrence-checkbox
                //delete-event-checkbox
                //eventIds.push(
            });
            console.log(eventIds);

            Confirm('Do you want to delete the ' + eventIds.length + ' selected event invitations?', function () {
                //Ajax.Post("/Events/EditDetails", saveObj).done(EditEvent.SaveDetailsComplete);
                var obj = { ids: eventIds };
               
                //var obj = { EventId: eid, Status: responseType, UpdateRecurring: false };


                $.ajax({
                    url: "/Events/SetEventInactive",
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    async: true,
                    cache: false,
                    data: JSON.stringify(obj),
                    dataType: 'html',
                    success: function success(results) {
                        console.log('SetEventInactive');
                        console.log(results);
                        //$('div.dashboard-event-item[data-event-id=' + eid + ']').html(results);
                        ////need to add one-off handlers b/c of the issue with event delegation and bubbling
                        //$('div.dashboard-event-item[data-event-id=' + eid + '] a.quick-response').on("click", Dashboard.QuickResponseClick);
                        //$('div.dashboard-event-item[data-event-id=' + eid + ']').on("click", Dashboard.EventLinkClick);
                        for (var x = 0; x < eventIds.length; x++) {
                            var $toRemove = $tab.find('div.dashboard-event-item[data-event-attendee-id=' + eventIds[x] + ']');

                            console.log($toRemove.length);
                            $toRemove.remove();
                        }
                    },
                    error: function error(err) {
                        alert('error');
                    }
                });

            });
        });
        //link to event
        $("a.quick-response").on("click", Dashboard.QuickResponseClick);
        //$("ul").on("click", "a.quick-response", function (evt) {


        //});

        $("div [data-event-link]").on("click", Dashboard.EventLinkClick);
        
        $(".delete-recurrence-checkbox").on("change", function (evt) {
            evt.stopPropagation();
            
        });
        $(".delete-event-checkbox[data-recurrence-id]").on("change", function (evt) {
            
            evt.stopPropagation();

            var recurId = Number($(this).attr('data-recurrence-id'));
            
            if ($(this).is(':checked')) {
                $(":checkbox.delete-recurrence-checkbox[data-recurrence-id=" + recurId + "]").prop('checked', true);
            }
            else {
                $(":checkbox.delete-recurrence-checkbox[data-recurrence-id=" + recurId + "]").prop('checked', false);
            }
        });
        $(".delete-event-checkbox[data-recurrence-id]").on("click", function (evt) {
            
            evt.stopPropagation();
        });

        $(":checkbox").on("click", function (evt) {
            evt.stopPropagation();
        });
        $(":checkbox").on("change", function (evt) {
            
            evt.stopPropagation();
            var tabType = $(this).closest('div[data-tab-type]').attr('data-tab-type');
            var prefix = 'div[data-tab-type=' + tabType + '] ';
            

            if ($(prefix + ' :checkbox:checked').length > 0) {

                $(prefix + ' #btnDeleteEvent').show();
            }
            else {
                $(prefix + ' #btnDeleteEvent').hide();
            }
        });
        $(".recurrence-link").on("click", function (evt) {

            var tabType = $(this).closest('div[data-tab-type]').attr('data-tab-type');
            var prefix = 'div[data-tab-type=' + tabType + '] ';

            evt.stopPropagation();
            var recurId = Number($(this).attr('data-recurrence-parent-item-id'));

            var $img = $(prefix + "#imgEventArrow[data-recurrence-id=" + recurId.toString() + "]");
            var $imgOpenDiv = $(this).closest('[data-event-id]').find('.open-event-arrow');
            var $imgClosedDiv = $(this).closest('[data-event-id]').find('.close-event-arrow');
            var $childDiv = $(prefix + "div[data-recurrence-item-id=" + recurId.toString() + "]");


            if ($(this).attr('data-children-visible') == 'false') {
                $(this).text('Hide Recurrences');
                $childDiv.css('display', 'flex');
                $(this).attr('data-children-visible', true);


                $imgClosedDiv.css('display', 'flex');
                $imgOpenDiv.hide();
                //~/content/images/open-event.png
                //var currentSrc = $img.attr('src').toString();
                //currentSrc = currentSrc.replace('open-event.png', 'close-event.png');
                //$img.attr('src', currentSrc);

            }
            else {
                $(this).text('View Recurrences');
                $childDiv.hide();
                $(this).attr('data-children-visible', false);
                $imgClosedDiv.hide();
                $imgOpenDiv.css('display', 'flex');
                //var currentSrc = $img.attr('src').toString();
                //currentSrc = currentSrc.replace('close-event.png', 'open-event.png');
                //$img.attr('src', currentSrc);
            }

        });

    },
    EventHandlers: {

    }
};