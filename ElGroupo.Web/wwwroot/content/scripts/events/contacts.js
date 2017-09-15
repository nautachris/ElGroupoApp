$(document).ready(function () {
    $(".links").on("click", function () {
        $(".links").removeClass('bold');
        $(this).addClass('bold');
        $(".row.tab").hide();
        $(".row." + $(this).attr('data-link-type')).show();

    });

    //organizers
    $("#btnAddOrganizer").on('click', function () {
        if (!$("#txtOrganizers").has('[data-contact-id]')) return;
        var cid = Number($("#txtOrganizers").attr('data-contact-id'));
        var isOwner = $("#chkOwner").is(':checked');
        var eid = Number($("#Event_Id").val());
        var obj = { userId: cid, owner: isOwner, eventId: eid };

        $.ajax({
            url: "/Events/Organizers/" + eid.toString() + '/Add',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            async: true,
            cache: false,
            dataType: "html",
            data: JSON.stringify(obj),
            success: function success(results) {
                $("#divOrganizers").html(results);
                $("#txtOrganizers").val('');
                $("#txtOrganizers").removeAttr('data-contact-id')
            },
            error: function error(err) {
                alert('fuck me');
            }
        });
    });
    $("#txtOrganizers").on("input", function () {
        if ($(this).val() === '') {
            $(this).removeAttr('data-contact-id');
            $("#btnAddOrganizer").attr('disabled', true);
        }
    });
    $("#txtOrganizers").autocomplete({
        minLength: 3,
        autoFocus: true,
        delay: 300,
        select: function (e, i) {
            $("#txtOrganizers").attr('data-contact-id', i.item.id);
            $("#btnAddOrganizer").attr('disabled', false);
        },
        source: function (request, response) {
            var _response = response;
            $.ajax({
                url: "/Users/Search/" + request.term,
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

    $("#divOrganizers").on("click", "a[data-contact-id]", function () {
        var eid = Number($("#Event_Id").val());
        var oid = Number($(this).attr('data-contact-id'));
        $.ajax({
            url: "/Events/Organizers/" + eid.toString() + '/Delete/' + oid.toString(),
            type: 'POST',
            //contentType: "application/json; charset=utf-8",
            async: true,
            cache: false,
            dataType: "html",
            //data: JSON.stringify(obj),
            success: function success(results) {
                $("#divOrganizers").html(results);
                $("#txtOrganizers").val('');

            },
            error: function error(err) {
                alert('fuck me');
            }
        });

    });


    $(".contact-search-rb").on("change", function () {
        if ($("#rbExistingContacts").is(':checked')) {
            $(".row.select-existing-contacts").show();
            $(".row.manual-search").hide();
        }
        else {
            $(".row.select-existing-contacts").hide();
            $(".row.manual-search").show();
        }
    });

    //attendees
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
            $("#txtAttendees").attr('data-contact-id', i.item.id);
            $("#btnAddAttendee").attr('disabled', false);
        },
        source: function (request, response) {
            var _response = response;
            $.ajax({
                url: "/Users/SearchAutocomplete/" + request.term,
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



    //new unregistered user
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
});