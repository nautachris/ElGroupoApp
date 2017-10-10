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

        var eid = Number($("#Event_Id").val());
        var obj = {
            name: nameVal,
            email: emailVal,
            eventId: eid
        };
        $.ajax({
            url: "/Events/Attendees/" + eid.toString() + '/AddUnregistered',
            type: 'POST',
            async: true,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            data: JSON.stringify(obj),
            success: function success(results) {
                $("#divAttendees").html(results);
                $("#txtAttendees").val('');
                $("#txtAttendees").removeAttr('data-contact-id')
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
        if (!$("#txtAttendees").has('[data-contact-id]')) return;
        var cid = Number($("#txtAttendees").attr('data-contact-id'));
        var eid = Number($("#Event_Id").val());
        var obj = { userId: cid, eventId: eid };

        $.ajax({
            url: "/Events/Attendees/" + eid.toString() + '/Add/' + cid.toString(),
            type: 'POST',
            async: true,
            cache: false,
            dataType: "html",
            success: function success(results) {
                $("#divAttendees").html(results);
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
});