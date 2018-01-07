$(document).ready(function () {
    EditEvent.Init();
});

EditEvent = {
    Init: function () {
        EditEvent.EventId = Number($("#EventId").val());
        EditEvent.IsRecurring = $("#IsRecurring").val() === 'True';
        EditEvent.EventName = $("#lblEventName").text();

        $("#btnEditDetails").on("click", EditEvent.EventHandlers.HandleEditDetailsClick);
        $("#btnCancelEditDetails").on("click", EditEvent.EventHandlers.HandleCancelEditDetailsClick);
        $("#btnEditLocation").on("click", EditEvent.EventHandlers.HandleEditLocationClick);
        $("#btnCancelEditLocation").on("click", EditEvent.EventHandlers.HandleCancelLocationEditClick);
        $("#btnSaveEditLocation").on("click", EditEvent.EventHandlers.HandleSaveLocationEditClick);
        $("#btnSaveEditDetails").on("click", EditEvent.EventHandlers.HandleSaveDetailsClick);
        $("#btnSubmit").on("click", EditEvent.EventHandlers.HandleSubmitClick);
        $("html").on("click", "#btnDeleteEvent", EditEvent.EventHandlers.HandleDeleteEventClick);
        $("html").on("click", ".switch-container.event-status > span", EditEvent.EventHandlers.HandleEventStatusChanged);
        $("html").on("click", ".switch-container.checkin-type > span", EditEvent.EventHandlers.HandleCheckinTypeChanged);
    },

    DeleteEvent: function (recurring) {

    },
    EventHandlers: {
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
                    console.log(results);
                    $("#btnEditLocation").hide();
                    $("#divViewLocation").hide();
                    $("#btnSaveEditLocation").show();
                    $("#btnCancelEditLocation").show();
                    $("#divEditLocation").empty().html(results).show();

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
        HandleEditDetailsClick: function () {
            $.ajax({
                url: "/Events/" + EditEvent.EventId + "/EditDetails",
                type: 'GET',
                contentType: "application/json; charset=utf-8",
                async: true,
                cache: false,
                dataType: 'html',
                success: function success(results) {
                    $("#btnEditDetails").hide();
                    $("#divViewDetails").hide();
                    $("#btnSaveEditDetails").show();
                    $("#btnCancelEditDetails").show();
                    $("#divEditDetails").empty().html(results).show();
                    $("#EventDate").datepicker();
                    SwitchContainer.init("#divEditDetails");
                },
                error: function error(err) {
                    alert('error');
                }
            });

        },
        HandleSaveDetailsClick: function () {
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
            var $this = $(this);
            console.log('event status change');

            var currentVal = $("#" + $(this).closest('div[data-replace-element]').attr('data-replace-element')).val();
            if (currentVal === $(this).attr('data-replace-val')) {

                return false;
            }

            Confirm("Do you want to change the status of this event to " + $(this).attr('data-replace-val') + "?  This action cannot be undone!", function () {
                if ($("#IsRecurring").val() == "True") {
                    Confirm("Do you to update all recurring events?", function () {
                        $.ajax({
                            url: "/Events/UpdateEventStatus",
                            type: 'POST',
                            contentType: "application/json",
                            data: JSON.stringify({
                                eventId: Number($("#EventId").val()),
                                status: $this.attr('data-replace-val'),
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



                //reset selected container
                $this.closest('.switch-container').find('span').removeClass('switch-selected');
                var $replaceEl = $("#" + $this.closest('div.switch-container').attr('data-replace-element'));
                $this.closest('.switch-container').find('span[data-replace-val=' + $replaceEl.val() + ']').addClass('switch-selected');
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
            Confirm('Do you want to delete the event ' + EditEvent.EventName + '?', function () {
                if (EditEvent.IsRecurring === true) {
                    Confirm('Do you want to delete all associated recurring events?', function () { }, function () { });
                }
                else {

                }

            });
        },
    },
    SaveDetailsComplete: function (results) {
        
        console.log('in EditEvent.SaveDetailsComplete');
        console.log(results);
        $("#divEditDetails").empty().hide();
        $("#divViewDetails").html(results).show();
        $("#btnCancelEditDetails").hide();
        $("#btnSaveEditDetails").hide();
        $("#btnEditDetails").show();
        MessageDialog("Your event has been updated!");
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

