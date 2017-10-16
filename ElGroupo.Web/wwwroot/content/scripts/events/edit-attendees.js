$(document).ready(function () {
    console.log('edit-attendees.js loaded');
    $("#divEditEventAttendees div").on("click", ".switch-container > div", function () {
        console.log('divEditattendees switch container click');
        console.log($(this).attr('data-action'));
        //$(this).closest("div.switch-container").find(".switch-selected").removeClass("switch-selected");
        //$(this).addClass("switch-selected");

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
            Owner: $(".unregistered-owner span.switch-selected").attr("data-action") === 'yes'
        };

        var otherAttendees = GetAddedAttendees();
        otherAttendees.push(attendee);

        $.ajax({
            url: "/Events/PendingAttendeeList",
            type: 'POST',
            async: true,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify(otherAttendees),
            success: function success(results) {
                $("#divAddedAttendeeList").html(results);
                $("#txtNewUserName").val('');
                $("#txtNewUserEmail").val('');
            },
            error: function error(err) {
                alert('fuck me');
            }
        });


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
            Id : Number($("#txtAttendees").attr("data-contact-id")),
            Name : $("#txtAttendees").val(),
            Owner: $(".registered-owner span.switch-selected").attr("data-action") === 'yes',
            Email: null
        };

        console.log('attendee to add');
        console.log(attendee);

        var otherAttendees = GetAddedAttendees();
        otherAttendees.push(attendee);

        console.log('all pending attendees');
        console.log(otherAttendees);
        //console.log('btnaddatendee post object');
        //console.log(obj);
        $.ajax({
            url: "/Events/PendingAttendeeList",
            type: 'POST',
            data : JSON.stringify(otherAttendees),
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
    });
    $("#txtAttendees").on("input", function () {
        if ($(this).val() === '') {
            $(this).removeAttr('data-contact-id');
            $("#btnAddAttendee").attr('disabled', true);
        }
    });

    $("#txtAttendees").autocomplete({
        minLength: 3,
        autoFocus: true,
        delay: 300,
        select: function (e, i) {
            console.log(i);
            $("#txtAttendees").attr('data-contact-id', i.item.id);
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
                    for (var x = 0; x < results.length; x++) {
                        var item = {
                            value: results[x].name,
                            label: results[x].name + ' (' + results[x].email + ')',
                            id: results[x].id                         
                        };
                        if (results[x].hasOwnProperty('registered')) item.isRegistered = results[x].registered;
                        output.push(item);
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
        //$(this).closest("div.pending-attendee-links").hide();
        //$infoDiv.css('opacity', 1);


    });


    $("#btnSaveAttendeeChanges").on("click", function () {
        var list = GetAddedAttendees();
        var obj = {
            EventId: Number($("#EventId").val()),
            Attendees: list
        };
        console.log('adding these attendees');
        console.log(obj);
        $.ajax({
            url: "/Events/SavePendingAttendees",
            type: 'POST',
            data: JSON.stringify(obj),
            async: true,
            cache: false,
            contentType: 'application/json',
            dataType: "html",
            success: function success(results) {
                //this will redirect to _ViewEventAttendees
                console.log('results from savependingattendees');
                console.log(results);
                $("#divViewAttendees").html(results);
                $("#divAddedAttendeeList").empty();

                $(".links[data-link-type=attendees]").click();
            },
            error: function error(err) {
                console.log(err);
                alert('fuck me');
            }
        });

    });

    function GetAddedAttendees() {
        var list = [];
        $("#divAddedAttendeeList div.pending-attendee-container").each(function () {
            var name = $(this).attr('data-user-name');
            var id = Number($(this).attr('data-user-id'));
            var email = null;
            if (id === -1) {
                email = $(this).attr('data-user-email');
            }
            var isOwner = $(this).find(".switch-selected").attr('data-action') === 'yes';


            var attendee = {
                Id : id,
                Name: name,
                Email: email,
                Owner: isOwner
            }
            list.push(attendee);


        });

        return list;

    }
});