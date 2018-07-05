$(document).ready(function () {
    EditEvent.Init();
});

EditEvent = {
    Init: function () {
        EditEvent.EventId = Number($("#EventId").val());
        EditEvent.IsRecurring = $("#IsRecurring").val() === 'True';
        EditEvent.EventName = $("#lblEventName").text();

        //for the "add new" button in attendees
        $("#divAttendees").on("click", ".show-edit-attendees", function () {
            console.log('add new');
            $("#divAttendees").hide();
            $("#divEditEventAttendees").show();
        });

        $(".edit-links").on("click", function () {
            console.log('edit links clicked: ' + $(this).attr('data-edit-type'));
            switch ($(this).attr('data-edit-type')) {
                case 'time':
                    EditEvent.LoadEventDates();
                    break;
                case 'details':
                    EditEvent.LoadEventDetails();
                    break;
                case 'location':
                    EditEvent.LoadEventLocation();
                    break;
                case 'notifications':
                    EditEvent.LoadEventNotifications();
                    break;
                case 'messages':
                    EditEvent.LoadEventMessages();
                    break;
            }

        });

        $(".close-links").on("click", EditEvent.ClearDetailsView);

        $(".save-links").on("click", function () {
            switch ($("#details-view-container").attr('data-view-type')) {
                case 'details':
                    break;
                case 'location':
                    break;
                case 'time':
                    break;
            }
        });


        $("html").on("change", "#frmEditEventLocation .location-search-method input[type=radio]", EditEvent.MapSelectionModeChanged);

        $("#btnSubmit").on("click", EditEvent.EventHandlers.HandleSubmitClick);

        //change to delegates
        //$("#frmEditEventDetails div.verification-method input[type=radio]").on("change", EditEvent.EventHandlers.VerificationMethodChanged);
        $("html").on("change", "#frmEditEventDetails div.verification-method input[type=radio]", EditEvent.EventHandlers.VerificationMethodChanged);
        $("html").on("change", "#frmEditEventDetails div.location-tolerance input[type=radio]", EditEvent.EventHandlers.LocationToleranceChanged);

        $("html").on("click", "#btnDeleteEvent", EditEvent.EventHandlers.HandleDeleteEventClick);
        $("html").on("change", ".event-status input[type=radio]", EditEvent.EventHandlers.HandleEventStatusChanged);
        $("html").on("click", ".switch-container.checkin-type > span", EditEvent.EventHandlers.HandleCheckinTypeChanged);

        $("#edit-attendees").on("click", function () {
            
            $.ajax({
                url: "/Events/EditEventAttendees/" + $("#EventId").val(),
                type: 'GET',
                async: true,
                cache: false,
                success: function success(results) {
                    //$("#edit-attendees").hide();
                    //$("#cancel-attendee-changes").show();
                    //$("#save-attendee-changes").show();
                    $(".edit-links").hide();
                    $(".save-links").show();
                    $(".close-links").show();
                    $("#divEditAttendees").empty().html(results).show();
                    EditEventAttendees.Init();
                    $("#divViewAttendees").hide();
                },
                error: function error(err) {
                    alert('error');
                }
            });



           

            
        });

        $("#save-attendee-changes").on("click", function () {
            EditEventAttendees.ConfirmSave();
        });
        $("#cancel-attendee-changes").on("click", function () {

            Confirm('Do you want to cancel any attendee edits you made?', function () {
                $("#divViewAttendees").show();
                $("#divEditAttendees").hide();
                $(".edit-links").show();
                $(".save-links").hide();

                $("#cancel-attendee-changes").hide();
                $("#edit-attendees").show();
                $("#save-attendee-changes").hide();
            });
            
        });
    },

    MapSelectionModeChanged: function () {
        $("#txtAutocompleteSearch").val('');
        $(".row.manual-search input[type=text]").val('');
        Maps.RemoveMarker();
        console.log($(this).attr('data-map-select'));
        //switch ($(this).attr('data-map-select')) {
        switch ($(this).val()) {
            case 'address':
                Maps.UpdateDrawingVisibility(false);
                $("#lblSearchType").text('Search by Street Address');
                //$(".row.map-search").show();
                $(".row.location-search").show();
                $(".row.manual-search-button").hide();
                //$(".row.manual-search input[type=text]").prop('disabled', true);
                Maps.autocomplete.setTypes(['geocode']);
                $("#LocationName").val('');
                break;
            case 'draw':
                Maps.UpdateDrawingVisibility(true);
                //$(".row.map-search").show();
                $(".row.location-search").hide();
                $(".row.manual-search-button").hide();
                //$(".row.manual-search input[type=text]").prop('disabled', true);
                break;
            case 'business':
                Maps.UpdateDrawingVisibility(false);
                $("#lblSearchType").text('Search by Business Name');
                //$(".row.map-search").show();
                $(".row.location-search").show();
                //$(".row.manual-search").hide();
                //$(".row.manual-search input[type=text]").prop('disabled', true);
                $(".row.manual-search-button").hide();
                Maps.autocomplete.setTypes(['establishment']);
                $("#LocationName").val('');
                break;
            case 'manual':
                Maps.UpdateDrawingVisibility(false);
                //$(".row.map-search").show();
                $(".row.location-search").hide();
                //$(".row.manual-search input[type=text]").prop('disabled', false);
                $(".row.manual-search-button").show();
                break;
        }
    },
    ClearDetailsView: function () {
        $("#details-view").empty();
        $("#details-view-container").hide();
        $(".edit-links").show();
        $(".save-links").hide();
        $(".close-links").hide();
        $("#divAttendees").show();
        //data-summary-type
        $("div[data-summary-type]").removeClass('summary-selected');
    },
    DeleteEvent: function (recurring) {




        if (EditEvent.IsRecurring === true) {
            var saveObj = obj;
            Confirm('Do you want to update all recurring events?', function () {
                saveObj.updateRecurring = true;
                Ajax.Post("/Events/EditDetails", saveObj).done(EditEvent.SaveDetailsComplete);
            }, function () {
                Ajax.Post({ url: "/Events/EditDetails", data: saveObj }).done(EditEvent.SaveDetailsComplete);
            });
        }
        else {
            Ajax.Post({ url: "/Events/EditDetails", data: obj }).done(EditEvent.SaveDetailsComplete);
        }
    },
    PopulateDetails: function (html, type) {
        $("#divAttendees").hide();
        $("#details-view-container").attr('data-view-type', type).show();
        $("#details-view").empty().html(html);
        $(".edit-links").hide();
        $(".save-links").show();
        $(".close-links").show();

        $("div[data-summary-type='"+ type + "']").addClass('summary-selected');
    },
    LoadEventDetails() {
        Ajax.Get("/Events/" + EditEvent.EventId + "/EditDetails").done(function (results) {
            EditEvent.PopulateDetails(results, 'details');
            SwitchContainer.init("#divEditDetails");

            //trigger the verification mode change event
            $("#frmEditEventDetails div.verification-method :checked").change();
            //500, 3000, 10000
            switch ($("#LocationTolerance").val()) {
                case '500':
                    $("#loc-tolerance-high").prop('checked', true);
                    break;
                case '3000':
                    $("#loc-tolerance-medium").prop('checked', true);
                    break;
                case '10000':
                    $("#loc-tolerance-low").prop('checked', true);
                    break;
                default:
                    $("#loc-tolerance-custom").prop('checked', true);
                    break;
            }
            $("#frmEditEventDetails div.location-tolerance input[type=radio] :checked").change();
        });
    },
    LoadEventLocation: function() {
        Ajax.Get("/Events/" + EditEvent.EventId + "/EditLocationNew").done(function (results) {
            EditEvent.PopulateDetails(results, 'location');
            Maps.InitMapManual({ mapDiv: 'divMap', txtAutocomplete: 'txtAutocompleteSearch', placeChangeCallback: EditEvent.PlaceChange });
            //init map here
        });
    },
    LoadEventDates: function () {
        Ajax.Get("/Events/" + EditEvent.EventId + "/EditDates").done(function (results) {
            EditEvent.PopulateDetails(results, 'time');
            $("#StartDate").datepicker();
            $("#EndDate").datepicker();
            //SwitchContainer.init("#frmEditEventDates");
            //init map here
        });
    },
    LoadEventMessages: function () {
        Ajax.Get("/Events/" + EditEvent.EventId + "/LoadMessages").done(function (results) {
            EditEvent.PopulateDetails(results, 'messages');
            //SwitchContainer.init("#frmEditEventDates");
            //init map here
        });
    },
    LoadEventNotifications: function () {
        Ajax.Get("/Events/" + EditEvent.EventId + "/LoadNotifications").done(function (results) {
            EditEvent.PopulateDetails(results, 'news');
            //SwitchContainer.init("#frmEditEventDates");
            //init map here
        });
    },
    EventHandlers: {
        VerificationMethodChanged: function () {
            console.log('verification method changed');
            switch ($(this).val()) {
                case "None":
                    $(".verification-code").hide();
                    $(".location-tolerance").hide();
                    break;
                case "PasswordOrLocation":
                    $(".verification-code").show();
                    $(".location-tolerance").show();
                    break;
                case "PasswordOnly":
                    $(".verification-code").show();
                    $(".location-tolerance").hide();
                    break;
            }
        },
        LocationToleranceChanged: function () {
            console.log('location tolerance changed');
            console.log($(this).val());
            console.log($("div.location-tolerance-custom input").length);
            switch ($(this).val()) {
                case 'high':
                    $("div.location-tolerance-custom").hide();
                    $("div.location-tolerance-custom input").val('500');
                    break;
                case 'medium':
                    $("div.location-tolerance-custom").hide();
                    $("div.location-tolerance-custom input").val('3000');
                    break;
                case 'low':
                    $("div.location-tolerance-custom").hide();
                    $("div.location-tolerance-custom input").val('10000');
                    break;
                default:
                    $("div.location-tolerance-custom").show();
                    break;
            }
        },
        HandleSaveLocationEditClick: function () { },
        HandleCancelLocationEditClick: function () { },
        HandleEditLocationClick: function () {
            $.ajax({
                url: "/Events/" + EditEvent.EventId + "/EditLocation",
                type: 'GET',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: 'html',
                success: function success(results) {
                    EditEvent.PopulateDetails(results);
                    //we need to init maps here!!
                    //Maps.Init({
                    //    mapDiv: 'divMap',
                    //    mapLoadedCallback: ViewEvent.MapLoaded
                    //});

                },
                error: function error(err) {
                    alert('error');
                }
            });
        },
        HandleCancelEditDetailsClick: function () {
            $("#divEditDetails").empty();
            $(this).hide();
            $("#btnSaveEditDetails").hide();
            $("#btnEditDetails").show();
            $("#divViewDetails").show();
        },
        HandleCloseDetailsClick: function () {
            $("#details-view").empty().hide();

        },
        HandleSaveDetailsClick: function () {

            //depending on what details view is active, this can do one of many things
            var obj = $("#frmEditEventDetails").serializeArray();
            var newObj = {};
            for (var x = 0; x < obj.length; x++) {
                newObj[obj[x].name] = obj[x].value;
            }
            if ($("#Status").val() !== $("#OriginalStatus").val()) {
                var saveObject = newObj;
                console.log('calling confirm');
                Confirm("Do you want to change the status of your event from " + $("#OriginalStatus").val() + " to " + $("#Status").val() + "?",
                    function () {
                        EditEvent.SaveDetails(saveObject);
                    },
                    function () {
                        //do nothing
                    });
            }
            else {
                console.log('status not changed');
                EditEvent.SaveDetails(newObj);
            }

        },
        HandleSubmitClick: function () {
            $("#frmEditEvent").submit();
        },
        HandleEventStatusChanged: function () {

            //radio button
            var oldStatus = $("#spanStatus").text();
            var newStatus = $(this).val();
            console.log('status change');
            console.log('old: ' + oldStatus);
            console.log('new: ' + newStatus);
            if (oldStatus === newStatus) return;


            Confirm("Do you want to change the status of this event to " + newStatus + "?  This action cannot be undone!", function () {
                if ($("#IsRecurring").val() == "True") {
                    Confirm("Do you to update all recurring events?", function () {
                        $.ajax({
                            url: "/Events/UpdateEventStatus",
                            type: 'POST',
                            contentType: "application/json",
                            data: JSON.stringify({
                                eventId: Number($("#EventId").val()),
                                status: newStatus,
                                updateRecurring: true
                            }),
                            async: true,
                            cache: false,
                            dataType: 'html',
                            success: function success(results) {
                                location.reload(true);
                            },
                            error: function error(err) {
                                alert('error');
                            }
                        });
                    }, function () {
                        $.ajax({
                            url: "/Events/UpdateEventStatus",
                            type: 'POST',
                            contentType: "application/json",
                            data: JSON.stringify({
                                eventId: Number($("#EventId").val()),
                                status: newStatus,
                                updateRecurring: false
                            }),
                            async: true,
                            cache: false,
                            success: function success(results) {
                                location.reload(true);
                            },
                            error: function error(err) {
                                alert('error');
                            }
                        });
                    });

                }
                else {
                    $.ajax({
                        url: "/Events/UpdateEventStatus",
                        type: 'POST',
                        contentType: "application/json",
                        data: JSON.stringify({
                            eventId: Number($("#EventId").val()),
                            status: $this.attr('data-replace-val'),
                            updateRecurring: false
                        }),
                        async: true,
                        cache: false,
                        //dataType: 'html',
                        success: function success(results) {
                            location.reload(true);
                        },
                        error: function error(err) {
                            alert('error');
                        }
                    });
                }
            }, function () {

                $(".event-status input[type=radio][value=" + oldStatus + "]").prop('checked', true);
            });
        },
        HandleCheckinTypeChanged: function () {
            switch ($(this).attr('data-replace-val')) {
                case "None":
                    $(".verification-code").hide();
                    $(".location-tolerance").hide();
                    break;
                case "PasswordAndLocation":
                    $(".verification-code").show();
                    $(".location-tolerance").show();
                    break;
                case "PasswordOnly":
                    $(".verification-code").show();
                    $(".location-tolerance").hide();
                    break;
            }
        },

        HandleDeleteEventClick: function () {
            var saveObj = { EventId: EditEvent.EventId, UpdateRecurring: false };
            Confirm('Do you want to delete the event ' + EditEvent.EventName + '?', function () {
                if (EditEvent.IsRecurring === true) {
                    Confirm('Do you want to delete all associated recurring events?', function () {
                        saveObj.UpdateRecurring = true;
                        Ajax.Delete({ url: "/Events/Delete", data: saveObj, dataType: 'json'  }).done(function (results) {
                            console.log('delete results');
                            console.log(results);
                            window.location.href = results.url;
                        });
                    }, function () {
                        Ajax.Delete({ url: "/Events/Delete", data: saveObj, dataType: 'json'  }).done(function (results) {
                            console.log('delete results');
                            console.log(results);
                            window.location.href = results.url;
                        });
                    });
                }
                else {
                    Ajax.Delete({ url: "/Events/Delete", data: saveObj, dataType: 'json' }).done(function (results) {
                        console.log('delete results');
                        console.log(results);
                        window.location.href = results.url;
                    });
                }

            });
        },
    },
    SaveDetailsComplete: function (results) {
        EditEvent.ClearDetailsView();
        MessageDialog("Your event has been updated!");

        //problem - we need to update the "banner" area with the updated information
    },
    SaveLocation: function () {
        var obj = $("#frmEditEventLocation").serializeArray();
        var saveObj = {};
        for (var x = 0; x < obj.length; x++) {
            saveObj[obj[x].name] = obj[x].value;
        }

        if (EditEvent.IsRecurring === true) {
            Confirm('Do you want to update all recurring events?', function () {
                saveObj.updateRecurring = true;
                Ajax.Post("/Events/EditLocationNew", saveObj).done(EditEvent.SaveDetailsComplete);
            }, function () {
                Ajax.Post({ url: "/Events/EditLocationNew", data: saveObj }).done(EditEvent.SaveDetailsComplete);
            });
        }
        else {
            Ajax.Post({ url: "/Events/EditLocationNew", data: obj }).done(EditEvent.SaveDetailsComplete);
        }
    },
    SaveDetails: function (obj) {
        if (EditEvent.IsRecurring === true) {
            var saveObj = obj;
            Confirm('Do you want to update all recurring events?', function () {
                saveObj.updateRecurring = true;
                Ajax.Post("/Events/EditDetails", saveObj).done(EditEvent.SaveDetailsComplete);
            }, function () {
                Ajax.Post({ url: "/Events/EditDetails", data: saveObj }).done(EditEvent.SaveDetailsComplete);
            });
        }
        else {
            Ajax.Post({ url: "/Events/EditDetails", data: obj }).done(EditEvent.SaveDetailsComplete);
        }
    },
    EventId: -1,
    EventName: null,
    IsRecurring: false,
    SaveEventChanges: function () { },
    IsAddress: false,
    MapLoaded: function () {
        var gpid = $("#GooglePlaceId").val();
        if (gpid === '') return false;
        Maps.PlaceSearch(gpid, function (place, status) {
            console.log(place);
            var isAddress = false;
            for (var x = 0; x < place.types.length; x++) {
                if (place.types[x] === 'street_address') {
                    EditEvent.IsAddress = true;
                    $(".row.business-name").hide();
                    break;
                }
                if (place.types[x] === 'establishment') {
                    EditEvent.IsAddress = false;
                    $(".row.business-name").show();
                    $("#txtBusinessName").val(place.name);
                    break;
                }
            }

            $(".row.manual-search input").prop('disabled', true);

            if (!EditEvent.IsAddress) {

            }

            //if (isAddress === true) {
            //    $("#rbMapSelectionAddress").prop('checked', true);
            //    $("#rbMapSelectionPlace").prop('checked', false);
            //    $("#rbMapSelectionAddress").change();
            //}
            //else {
            //    $("#rbMapSelectionAddress").prop('checked', false);
            //    $("#rbMapSelectionPlace").prop('checked', true);
            //    $("#rbMapSelectionPlace").change();
            //}

            Maps.SetInfoWindow(place);

        });
    },
    PlaceChange: function (place) {
        console.log('placechange');

        $("#Address1").val('');
        $("#City").val('');
        $("#ZipCode").val('');
        $("#State").val('');

        var streetNumber = '';
        var streetName = '';
        var city = '';
        var state = '';
        var zip = '';;

        if (!place.address_components) return;
        for (var x = 0; x < place.address_components.length; x++) {
            for (var y = 0; y < place.address_components[x].types.length; y++) {
                switch (place.address_components[x].types[y].toString()) {
                    case 'street_number':
                        streetNumber = place.address_components[x].short_name;
                        break;
                    case 'route':
                        streetName = place.address_components[x].short_name;
                        break;
                    case 'locality':
                        city = place.address_components[x].short_name;
                        break;
                    case 'postal_code':
                        zip = place.address_components[x].short_name;
                        break;
                    case 'administrative_area_level_1':
                        state = place.address_components[x].short_name;
                        break;

                }
            }
        }

        $("#Address1").val(streetNumber + ' ' + streetName);
        $("#City").val(city);
        $("#ZipCode").val(zip);
        $("#State").val(state);

        $("#YCoord").val(place.geometry.location.lat());
        $("#XCoord").val(place.geometry.location.lng());
        $("#GooglePlaceId").val(place.place_id);

    }
};

