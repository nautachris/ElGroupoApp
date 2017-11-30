$(document).ready(function () {
    AttendeeGroups.Init();
});




AttendeeGroups = {
    Init: function () {
        $("#divEditAttendeeGroup").on('click', "#btnAddUser", AttendeeGroups.EventHandlers.AddUserToGroupClicked);
        $("#divEditAttendeeGroup").on("input", "#txtSelectUser", AttendeeGroups.EventHandlers.SelectUserTextChanged);
        $("#divEditAttendeeGroup").on("click", ".pending-attendee-links a", AttendeeGroups.EventHandlers.PendingUserLinkClicked);
        $("#divEditAttendeeGroup").on("click", "div.pending-attendee-info", AttendeeGroups.EventHandlers.PendingUserDivClicked);
        $("#divViewAttendeeGroups").on("click", "div.group-links a", AttendeeGroups.EventHandlers.EditGroupLinkClicked);
        $("#divEditAttendeeGroup").on("click", "#btnSaveGroupChanges", AttendeeGroups.EventHandlers.SaveGroupChangesClicked);
        $("#divEditAttendeeGroup").on("click", "#btnReturnToGroups", AttendeeGroups.EventHandlers.ReturnToGroupListClicked);
        $("#divAttendeeGroups").on("click", "#btnAddAttendeeGroup", AttendeeGroups.EventHandlers.AddGroupClicked);
    },
    EventHandlers: {
        AddUserToGroupClicked: function () {
            var attendee = {
                Id: Number($("#txtSelectUser").attr("data-contact-id")),
                Name: $("#txtSelectUser").val(),
                Email: $("#txtSelectUser").attr("data-contact-email")
            };
            var otherAttendees = AttendeeGroups.GetGroupUsers();
            otherAttendees.push(attendee);
            Loading.Start();
            $.ajax({
                url: "/Account/AttendeeGroupUserList",
                type: 'POST',
                data: JSON.stringify(otherAttendees),
                async: true,
                cache: false,
                contentType: 'application/json',
                dataType: "html",
                success: function success(results) {
                    Loading.Stop();
                    $("#divAttendeeGroupUsersList").empty().html(results);
                    $("#txtSelectUser").val('');
                    $("#txtSelectUser").removeAttr('data-contact-id');
                    $("#txtSelectUser").removeClass('light-blue');
                    $("#btnAddUser").attr('disabled', true);
                },
                error: function error(err) {
                    Loading.Stop();
                    alert('error');
                }
            });
        },
        SelectUserTextChanged: function () {
            if ($(this).val() === '') {
                $(this).removeAttr('data-contact-id');
                $("#btnAddUser").attr('disabled', true);
                $("#txtSelectUser").removeClass('light-blue');
            }
        },
        PendingUserLinkClicked: function () {
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
        },
        PendingUserDivClicked: function () {
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
        },
        EditGroupLinkClicked: function () {
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
                            alert('error');
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
                        AttendeeGroups.SetUserAutoComplete();
                        $("#divViewAttendeeGroups").hide();
                        $("#divEditAttendeeGroup").show();
                    },
                    error: function error(err) {
                        alert('error');
                    }
                });

            }
        },
        SaveGroupChangesClicked: function () {
            if ($("#txtAttendeeGroupName").val() === '') {
                alert('Group Name is Required');
                return false;
            }

            var model = {
                UserId: Number($("#Id").val()),
                Id: Number($("#iptGroupId").val()),
                Name: $("#txtAttendeeGroupName").val(),
                Users: AttendeeGroups.GetGroupUsers()
            };

            Loading.Start();
            $.ajax({
                url: "/Account/EditAttendeeGroup",
                type: 'POST',
                data: JSON.stringify(model),
                async: true,
                cache: false,
                contentType: 'application/json',
                dataType: "html",
                success: function success(results) {
                    Loading.Stop();
                    $("#divViewAttendeeGroups").empty().html(results);

                },
                error: function error(err) {
                    Loading.Stop();
                    alert('error');
                }
            });
        },
        ReturnToGroupListClicked: function () {
            $("#divViewAttendeeGroups").show();
            $("#divEditAttendeeGroup").hide();
        },
        AddGroupClicked: function () {
            $.ajax({
                url: "/Account/ViewAttendeeGroup/0",
                type: 'GET',
                //contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: "html",
                success: function success(results) {
                    $("#divEditAttendeeGroup").html(results);
                    AttendeeGroups.SetUserAutoComplete();
                    $("#divViewAttendeeGroups").hide();
                    $("#divEditAttendeeGroup").show();
                },
                error: function error(err) {
                    alert('error');
                }
            });
        }
    },
    SetUserAutoComplete : function () {
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
                        alert('error');
                    }
                });
            }
        });

    },
    GetGroupUserIds : function () {
        var ids = [];
        $("#divAttendeeGroupUsersList div.pending-attendee-group-container").each(function () {
            ids.push(Number($(this).attr('data-user-id')));
        });
        return ids;
    },
    GetGroupUsers : function () {
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
};