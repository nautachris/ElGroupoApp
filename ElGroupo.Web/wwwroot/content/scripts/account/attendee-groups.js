$(document).ready(function () {
    //divViewAttendeeGroups contains list of all groups
    //divEditAttendeeGroup contains active group edit view

    $("#divAttendeeGroups").on("click", "#btnAddAttendeeGroup", function () {
        $.ajax({
            url: "/Account/ViewAttendeeGroup/0",
            type: 'GET',
            //contentType: "application/json; charset=utf-8",
            async: true,
            cache: false,
            dataType: "html",
            success: function success(results) {
                $("#divEditAttendeeGroup").html(results);
                SetUserAutoComplete();
                $("#divViewAttendeeGroups").hide();
                $("#divEditAttendeeGroup").show();
            },
            error: function error(err) {
                alert('fuck me');
            }
        });

    });
    $("#divEditAttendeeGroup").on("click", "#btnReturnToGroups", function () {
        //check for changes
        $("#divViewAttendeeGroups").show();
        $("#divEditAttendeeGroup").hide();

    });
    $("#divEditAttendeeGroup").on("click","#btnSaveGroupChanges", function () {

        if ($("#txtAttendeeGroupName").val() === '') {
            alert('Group Name is Required');
            return false;
        }

        var model = {
            UserId: Number($("#Id").val()),
            Id: Number($("#iptGroupId").val()),
            Name: $("#txtAttendeeGroupName").val(),
            Users: GetGroupUsers()
        };


        $.ajax({
            url: "/Account/EditAttendeeGroup",
            type: 'POST',
            data: JSON.stringify(model),
            async: true,
            cache: false,
            contentType: 'application/json',
            dataType: "html",
            success: function success(results) {
                $("#divViewAttendeeGroups").empty().html(results);

            },
            error: function error(err) {
                alert('fuck me');
            }
        });



    });


    //user click on edit/remove group
    $("#divViewAttendeeGroups").on("click", "div.group-links a", function () {
        console.log('edit group links clicked');
        var groupId = $(this).closest('div[data-group-id]').attr('data-group-id');

        if ($(this).attr('data-action') === 'remove') {
            Confirm("Do you want to remove this attendee group?", function () {
                $.ajax({
                    url: "/Account/DeleteAttendeeGroup/" + groupId,
                    type: 'DELETE',
                    //contentType: "application/json; charset=utf-8",
                    async: true,
                    cache: false,
                    dataType: "html",
                    success: function success(results) {
                        $("#divViewAttendeeGroups").empty().html(results);
                    },
                    error: function error(err) {
                        alert('fuck me');
                    }
                });

            });
        }
        else {
            //edit
            $.ajax({
                url: "/Account/ViewAttendeeGroup/" + groupId,
                type: 'GET',
                //contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: "html",
                success: function success(results) {
                    $("#divEditAttendeeGroup").empty().html(results);
                    SetUserAutoComplete();
                    $("#divViewAttendeeGroups").hide();
                    $("#divEditAttendeeGroup").show();
                },
                error: function error(err) {
                    alert('fuck me');
                }
            });

        }

    });

    //click on user image to show the remove/view profile links
    $("#divEditAttendeeGroup").on("click", "div.pending-attendee-info", function () {
        console.log('group user clicked');
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
    $("#divEditAttendeeGroup").on("click", ".pending-attendee-links a", function () {
        console.log('link clicked');
        var $infoDiv = $(this).closest("div[data-user-id]").find("div.pending-attendee-info");
        if ($(this).attr('data-action') == 'profile') {
            //profile link
        }
        else {
            //remove
            $(this).closest(".pending-attendee-group-container").remove();
        }
        //$(this).closest("div.pending-attendee-links").hide();
        //$infoDiv.css('opacity', 1);


    });

    $("#divEditAttendeeGroup").on("input", "#txtSelectUser", function () {
        if ($(this).val() === '') {
            $(this).removeAttr('data-contact-id');
            $("#btnAddUser").attr('disabled', true);
            $("#txtSelectUser").removeClass('light-blue');
        }
    });

    //add user to group
    $("#divEditAttendeeGroup").on('click', "#btnAddUser", function () {
        var attendee = {
            Id: Number($("#txtSelectUser").attr("data-contact-id")),
            Name: $("#txtSelectUser").val(),
            Email: $("#txtSelectUser").attr("data-contact-email")
        };
        var otherAttendees = GetGroupUsers();
        otherAttendees.push(attendee);
        $.ajax({
            url: "/Account/AttendeeGroupUserList",
            type: 'POST',
            data: JSON.stringify(otherAttendees),
            async: true,
            cache: false,
            contentType: 'application/json',
            dataType: "html",
            success: function success(results) {
                $("#divAttendeeGroupUsersList").empty().html(results);
                $("#txtSelectUser").val('');
                $("#txtSelectUser").removeAttr('data-contact-id');
                $("#txtSelectUser").removeClass('light-blue');
                $("#btnAddUser").attr('disabled', true);
            },
            error: function error(err) {
                alert('fuck me');
            }
        });
    });





    GetGroupUserIds = function () {
        var ids = [];
        $("#divAttendeeGroupUsersList div.pending-attendee-group-container").each(function () {
            ids.push(Number($(this).attr('data-user-id')));
        });
        return ids;
    }
    GetGroupUsers = function () {
        var list = [];
        $("#divAttendeeGroupUsersList div.pending-attendee-group-container").each(function () {
            var obj = {
                Id: Number($(this).attr('data-user-id')),
                Name: $(this).attr('data-user-name'),
                Email: $(this).attr('data-user-email')
            };
            list.push(obj);
        });
        return list;
    }

    //$("#btnSearch").on("click", function () {
    //    var val = $("#txtSearch").val();
    //    if (val === '') return;

    //    $.ajax({
    //        url: "/Users/Search/" + val,
    //        type: 'GET',
    //        //contentType: "application/json; charset=utf-8",
    //        async: true,
    //        cache: false,
    //        dataType: "html",
    //        //data: JSON.stringify(obj),
    //        success: function success(results) {
    //            $("#divUserList").html(results);

    //        },
    //        error: function error(err) {
    //            alert('fuck me');
    //        }
    //    });

    //});

});


SetUserAutoComplete = function () {
    $("#txtSelectUser").autocomplete({
        minLength: 3,
        autoFocus: true,
        delay: 300,
        select: function (e, i) {
            console.log('selected autocomplete item');
            console.log(i);
            $("#txtSelectUser").attr('data-contact-id', i.item.id);
            $("#txtSelectUser").attr('data-contact-email', i.item.email);
            $("#txtSelectUser").addClass('light-blue');
            $("#btnAddUser").attr('disabled', false);
        },
        source: function (request, response) {
            //var userId = $("#Id").val();
            //var apiUrl = $("#rbContactsConnections").is(':checked') ? "/Users/SearchAllUsers/" + request.term : "/Users/SearchAutocomplete/" + request.term;
            var _response = response;
            var urlPrefix = null;
            if ($("div.search-method div.switch-selected").attr('data-action') === 'all') urlPrefix = "/Users/SearchAllUsers/";
            else urlPrefix = "/Users/SearchUserConnections/";



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
                            email: results[x].email,
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

}