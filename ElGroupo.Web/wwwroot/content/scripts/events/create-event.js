$(document).ready(function () {
    CreateEvent.Init();
});

CreateEvent = {
    Init: function () {

        $("#EventDate").datepicker({
            //dateFormat: "mm/dd/yyyy",
            //defaultDate: new Date(Date.now())
        });
        $("html").on("click", ".switch-container.map-select > span", CreateEvent.EventHandlers.MapSelectionModeChanged);
        $(".switch-container.checkin-type > span").on("click", CreateEvent.EventHandlers.CheckInModeChanged);
        $(".row.days-of-week span[data-day-index]").on("click", CreateEvent.EventHandlers.DayOfWeekClicked);
        $("div.switch-container[data-replace-element='Recurrence_Pattern'] span").on("click", CreateEvent.EventHandlers.RecurrencePatternChanged);
        $("div.switch-container.is-recurring span").on("click", CreateEvent.EventHandlers.RecurrenceChanged);
        $("#btnSubmit").on("click", CreateEvent.EventHandlers.SubmitClicked);
        $("#btnSearchAddress").on("click", CreateEvent.EventHandlers.AddressSearchClicked);
    },
    EventHandlers: {
        AddressSearchClicked: function () {
            var addressText = [];
            if ($("#Address1").val() !== '') addressText.push($("#Address1").val());
            if ($("#Address2").val() !== '') addressText.push($("#Address2").val());
            if ($("#City").val() !== '') addressText.push($("#City").val());
            if ($("#State").val() !== '') addressText.push($("#State").val());
            if ($("#ZipCode").val() !== '') addressText.push($("#ZipCode").val());
            if (addressText.length === 0) {
                alert('Please enter address info');
            }
            Maps.GeocodeAddress(addressText.join(' '));
        },
        SubmitClicked: function () {
            //var frm = $("#frmCreateEvent").serialize();
            //console.log(frm);
            //var frm2 = $("#frmCreateEvent").serialize();
            //console.log(frm2);

            $("#frmCreateEvent").submit();
        },
        RecurrenceChanged: function () {
            if ($(this).attr('data-replace-val') === 'True') {
                $("#divRecurrence").show();
                $("#lblEventDate").text('Starting Date:');
            }
            else {
                $("#divRecurrence").hide();
                $("#lblEventDate").text('Event Date:');
            }
        },
        RecurrencePatternChanged: function () {
            console.log('recur pattern click');
            console.log($(this).attr('data-replace-val'));
            var pattern = $(this).attr('data-replace-val');
            switch (pattern) {
                case 'Daily':
                    $("#lblInterval").text('Days');
                    $(".row.days-of-week").hide();
                    $(".row.monthly-recurrence").hide();
                    break;
                case 'Weekly':
                    $("#lblInterval").text('Weeks');
                    $(".row.days-of-week").show();
                    $(".row.monthly-recurrence").hide();
                    break;
                case 'Monthly':
                    $("#lblInterval").text('Months');
                    $(".row.days-of-week").hide();
                    $(".row.monthly-recurrence").show();
                    break;

            }
        },
        DayOfWeekClicked: function () {
            if ($(this).attr('data-selected') === 'false') $(this).attr('data-selected', 'true');
            else $(this).attr('data-selected', 'false');

            var $chk = $(".row.days-of-week :checkbox[data-day-index=" + $(this).attr('data-day-index') + "]");

            if ($(this).attr('data-selected') === 'true') $chk.prop('checked', true);
            else $chk.prop('checked', false);
        },
        CheckInModeChanged: function () {
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
        MapSelectionModeChanged: function () {
            $("#txtAutocompleteSearch").val('');
            $(".row.manual-search input[type=text]").val('');
            Maps.RemoveMarker();
            switch ($(this).attr('data-map-select')) {
                case 'address':
                    Maps.UpdateDrawingVisibility(false);
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
        }
    },
    PlaceChange: function (place) {
        console.log('in CreateEvent.PlaceChange');
        console.log(place);
        if (place.name !== undefined && place.name !== null && place.name !== '') {
            $("#LocationName").val(place.name);
        }

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