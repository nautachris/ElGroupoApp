$(document).ready(function () {
    EditEventAttendees.Init();
});

EditEventAttendees = {
    EventId: -1,
    Init: function () {
        EditEventAttendees.EventId = Number($("#EventId").val());

        $("#btnSaveAttendeeChanges").on("click", function () {
            if ($("#IsRecurring").val() == "True") {
                Confirm('Do you want to update all recurring events?', function () {
                    EditEventAttendees.SaveChanges(true);
                }, function () {
                    EditEventAttendees.SaveChanges(false);
                });
            }
            else {
                EditEventAttendees.SaveChanges(false);
            }
        });
        $("html").on("click", ".pending-attendee-links a", function () {
            console.log('link clicked');
            var $infoDiv = $(this).closest("div[data-user-id]").find("div.pending-attendee-info");
            if ($(this).attr('data-action') == 'profile') {
                //profile link
            }
            else {
                //remove
                $(this).closest(".pending-attendee-container").remove();
            }
        });
        //pending users
        $("html").on("click", "div.pending-attendee-info", function () {
            var $links = $(this).closest("div[data-user-id]").find("div.pending-attendee-links");
            if ($(this).css('opacity') == 0.5) {
                $links.hide();
                $(this).css('opacity', 1);
            }

            else {
                //close all links?
                $("div.pending-attendee-links").hide();
                $("div.pending-attendee-info").css('opacity', 1);

                $(this).css('opacity', 0.5);
                $links.show();
            }
        });
        $("#txtAttendees").autocomplete({
            minLength: 3,
            autoFocus: true,
            delay: 300,
            select: function (e, i) {
                console.log(i);
                $("#txtAttendees").attr('data-contact-id', i.item.id);
                $("#txtAttendees").attr('data-group', i.item.group);
                $("#btnAddAttendee").attr('disabled', false);
            },
            source: function (request, response) {
                var _response = response;
                var urlPrefix = null;
                if ($("div.search-method div.switch-selected").attr('data-action') === 'all') urlPrefix = "/Users/SearchAllUsers/";
                else urlPrefix = "/Users/SearchUserConnections/";

                console.log(urlPrefix + request.term);
                $.ajax({
                    url: urlPrefix + request.term,
                    type: 'GET',
                    //dataType: "application/json; charset=utf-8",
                    async: true,
                    cache: false,
                    success: function success(results) {
                        var output = [];
                        console.log('autocomplete results');
                        console.log(results);
                        for (var x = 0; x < results.length; x++) {
                            if (results[x].hasOwnProperty('group') && Boolean(results[x].group) === true) {
                                var item = {
                                    value: results[x].name,
                                    label: results[x].name + ' (' + results[x].groupUserCount + ')',
                                    id: results[x].id,
                                    group: true
                                };
                                output.push(item);
                            }
                            else {
                                var item = {
                                    value: results[x].name,
                                    label: results[x].name + ' (' + results[x].email + ')',
                                    id: results[x].id,
                                    group: false
                                };
                                if (results[x].hasOwnProperty('registered')) item.isRegistered = results[x].registered;
                                output.push(item);
                            }

                        }
                        _response(output);
                    },
                    error: function error(err) {
                        alert('fuck me');
                    }
                });
            }
        });

        $("#btnCancelAddAttendee").on("click", function () {

            Confirm("Do you want to cancel your event attendee list edits?", function () {
                $("#divAddedAttendeeList").empty();
            });


        });

        //attendee remove/view profile
        $("html").on("click", "div.attendee-info", function () {
            console.log('attendee info click');
            var $links = $(this).closest("div[data-user-id]").find("div.attendee-links");
            if ($(this).css('opacity') == 0.5) {
                //links are showing
                console.log('links are showing');
                $("div.attendee-links").hide();
                $("div.attendee-info").css('opacity', 1);
            }
            else {
                //we do this to close any other open attendee links in the grid
                $("div.attendee-links").hide();
                $("div.attendee-info").css('opacity', 1);
                $(this).css('opacity', 0.5);
                $links.show();

            }
        });
        $(".attendee-links").on("click", "a", function () {
            console.log('attendee links click');
            var $infoDiv = $(this).closest("div[data-user-id]").find("div.attendee-info");
            if ($(this).attr('data-action') == 'profile') {
                //profile link
            }
            else {
                //remove
                Confirm("Do you want to remove this attendee from this event?", function () { }, function () { });

            }
            $(this).closest("div.attendee-links").hide();
            $infoDiv.css('opacity', 1);

        });
        $("#divEditEventAttendees div").on("click", ".switch-container > div", function () {
            if ($(this).attr('data-action') === 'manual') {
                console.log('hiding autocomplete');
                $('.row.select-existing-contacts').hide();
                $('.row.manual-search').show();
            }
            else {
                console.log('showing autocomplete');
                $('.row.select-existing-contacts').show();
                $('.row.manual-search').hide();
            }
        });
        $("#btnAddNewUser").on("click", function () {
            var nameVal = $("#txtNewUserName").val();
            var emailVal = $("#txtNewUserEmail").val()
            if (nameVal === '') {
                alert('name required');
                return false;
            }
            if (emailVal === '') {
                alert('email required');
                return false;
            }

            var attendee = {
                Id: -1,
                Name: $("#txtNewUserName").val(),
                Email: $("#txtNewUserEmail").val(),
                Owner: $(".unregistered-owner span.switch-selected").attr("data-action") === 'yes',
                Group: false
            };
            EditEventAttendees.AddAttendee(attendee);

        });
        $("#divAttendees").on("click", "a[data-contact-id]", function () {
            var eid = Number($("#Event_Id").val());
            var oid = Number($(this).attr('data-contact-id'));
            $.ajax({
                url: "/Events/Attendees/" + eid.toString() + '/Delete/' + oid.toString(),
                type: 'POST',
                //contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: "html",
                //data: JSON.stringify(obj),
                success: function success(results) {
                    $("#divAttendees").html(results);
                    $("#txtAttendees").val('');

                },
                error: function error(err) {
                    alert('fuck me');
                }
            });

        });
        $("#btnAddAttendee").on('click', function () {


            //we need to post the list of pending event attendees - or just refresh the pending event list client-side
            var attendee = {
                Id: Number($("#txtAttendees").attr("data-contact-id")),
                Name: $("#txtAttendees").val(),
                Owner: $(".registered-owner span.switch-selected").attr("data-action") === 'yes',
                Group: $("#txtAttendees").attr("data-group") === 'true',
                Email: null
            };
            EditEventAttendees.AddAttendee(attendee);



        });
        $("#txtAttendees").on("input", function () {
            if ($(this).val() === '') {
                $(this).removeAttr('data-contact-id');
                $("#btnAddAttendee").attr('disabled', true);
            }
        });
    },
    GetAddedAttendees: function () {

        var list = [];
        $("#divAddedAttendeeList div.pending-attendee-container").each(function () {
            var name = $(this).attr('data-user-name');
            var id = Number($(this).attr('data-user-id'));
            var isGroup = $(this).attr('data-group') === 'true';
            var email = null;
            if (id === -1 && !isGroup) {
                email = $(this).attr('data-user-email');
            }
            var isOwner = $(this).find(".switch-selected").attr('data-action') === 'yes';


            var attendee = {
                Id: id,
                Name: name,
                Email: email,
                Owner: isOwner,
                Group: isGroup
            }
            list.push(attendee);


        });

        return list;
    },
    AddAttendee: function (attendee) {
        var otherAttendees = EditEventAttendees.GetAddedAttendees();
        otherAttendees.push(attendee);
        $.ajax({
            url: "/Events/PendingAttendeeList",
            type: 'POST',
            data: JSON.stringify(otherAttendees),
            async: true,
            cache: false,
            contentType: 'application/json',
            dataType: "html",
            success: function success(results) {
                $("#divAddedAttendeeList").html(results);
                $("#txtAttendees").val('');
                $("#txtAttendees").removeAttr('data-contact-id')
            },
            error: function error(err) {
                alert('fuck me');
            }
        });
    },
    SaveChanges: function (updateRecurring) {
        console.log('in EditEventAttendees.SaveChanges ' + updateRecurring);
        var list = EditEventAttendees.GetAddedAttendees();
        var obj = {
            EventId: Number($("#EventId").val()),
            Attendees: list,
            UpdateRecurring: updateRecurring
        };
        $.ajax({
            url: "/Events/SavePendingAttendees",
            type: 'POST',
            data: JSON.stringify(obj),
            async: true,
            cache: false,
            contentType: 'application/json',
            dataType: "html",
            success: function success(results) {
                $("#divViewAttendees").html(results);
                $("#divAddedAttendeeList").empty();

                $(".links[data-link-type=attendees]").click();
            },
            error: function error(err) {
                console.log(err);
                alert('fuck me');
            }
        });
    }
};