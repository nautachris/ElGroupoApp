$(document).ready(function () {
    $(".links").on("click", function () {
        $(".links").removeClass('bold');
        $(this).addClass('bold');
        $(".row.tab").hide();
        $(".row." + $(this).attr('data-link-type')).show();

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





    //new unregistered user

});