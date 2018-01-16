$(document).ready(function () {
    CreateEvent.Init();
});

CreateEvent = {
    MapLoaded: false,
    ActiveStep: 1,
    LoadStep: function () {
        $("div[data-create-step]").hide();
        $("div[data-create-step=" + CreateEvent.ActiveStep + "]").show();

        switch (CreateEvent.ActiveStep) {
            case 1:
                $('#create-step').text('Step 1 of 4');
                $('#create-step-title').text('Name & Description');
                $('#create-step-description').text("Give your event a name and a brief description about what it's about.");
                $("#btnPreviousStep").hide();
                $("#btnNextStep").show();
                $("#divSubmit").hide();
                break;
            case 2:
                $('#create-step').text('Step 2 of 4');
                $('#create-step-title').text('Date & Time');
                $('#create-step-description').text('Let your invitees when to attend and whether a RSVP is required.');
                $("#btnPreviousStep").show();
                $("#btnNextStep").show();
                $("#divSubmit").hide();
                $("#StartDate").datepicker();
                $("#EndDate").datepicker();
                break;
            case 3:
                $('#create-step').text('Step 3 of 4');
                $('#create-step-title').text('Location');
                $('#create-step-description').text('Tell your invitees where they need to be.');
                $("#btnPreviousStep").show();
                $("#btnNextStep").show();
                $("#divSubmit").hide();
                if (!CreateEvent.MapLoaded) {
                    Maps.InitMapManual();
                    CreateEvent.MapLoaded = true;
                    //setTimeout(function () {
                    //    Maps.Init({
                    //        mapDiv: 'divMap',
                    //        txtAutocomplete: 'txtAutocompleteSearch',
                    //        placeChangeCallback: CreateEvent.PlaceChange,
                    //        createDrawTools: true,
                    //        showDrawTools: false
                    //    });
                    //}, 400);

                    //CreateEvent.MapLoaded = true;
                }


                break;
            case 4:
                $('#create-step').text('Step 4 of 4');
                $('#create-step-title').text('Check-In');
                $('#create-step-description').text('Tell your invitees how they can check in.');
                $("#btnPreviousStep").show();
                $("#btnNextStep").hide();
                $("#divSubmit").show();
                break;
        }
    },
    Init: function () {

        $("#StartDate").datepicker();
        $("#EndDate").datepicker();

        //location-search-method
        //$("html").on("click", ".switch-container.map-select > span", CreateEvent.EventHandlers.MapSelectionModeChanged);
        $("html").on("change", "div.location-search-method input[type = radio]", CreateEvent.EventHandlers.MapSelectionModeChanged);
        //$(".switch-container.checkin-type > span").on("click", CreateEvent.EventHandlers.CheckInModeChanged);
        $("div.checkin-type input[type=radio]").on("change", CreateEvent.EventHandlers.CheckInModeChanged);
        $(".row.days-of-week span[data-day-index]").on("click", CreateEvent.EventHandlers.DayOfWeekClicked);
        //$("div.switch-container[data-replace-element='Recurrence_Pattern'] span").on("click", CreateEvent.EventHandlers.RecurrencePatternChanged);
        //$("div.switch-container.is-recurring span").on("click", CreateEvent.EventHandlers.RecurrenceChanged);

        $("div.location-tolerance input[type=radio]").on("change", CreateEvent.EventHandlers.LocationToleranceChanged);

        $("div.recurrence-pattern input[type=radio]").on("change", CreateEvent.EventHandlers.RecurrencePatternChanged);
        $("div.is-recurring input[type=radio]").on("change", CreateEvent.EventHandlers.RecurrenceChanged);
        $("#btnSubmit").on("click", CreateEvent.EventHandlers.SubmitClicked);
        $("#btnSearchAddress").on("click", CreateEvent.EventHandlers.AddressSearchClicked);

        $("#btnNextStep").on("click", function () {
            if (CreateEvent.ActiveStep === 4) return;
            CreateEvent.ActiveStep++;
            CreateEvent.LoadStep();

        });
        $("#btnPreviousStep").on("click", function () {
            if (CreateEvent.ActiveStep === 1) return;
            CreateEvent.ActiveStep--;
            CreateEvent.LoadStep();
        });
    },
    EventHandlers: {
        LocationToleranceChanged: function () {
            console.log('location tolerance changed');
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


            console.log($("#frmCreateEvent").serialize());
            console.log($("#frmCreateEvent").serializeArray());
            $("#frmCreateEvent").submit();
        },
        RecurrenceChanged: function () {

            console.log($(this).val());
            if ($(this).val() == true) {
                console.log('== true');
            }
            if ($(this).val() === 'True') {
                $("#divRecurrence").show();
                $("#lblEventDate").text('Starting Date:');
            }
            else {
                $("#divRecurrence").hide();
                $("#lblEventDate").text('Event Date:');
            }
            //if ($(this).attr('data-replace-val') === 'True') {
            //    $("#divRecurrence").show();
            //    $("#lblEventDate").text('Starting Date:');
            //}
            //else {
            //    $("#divRecurrence").hide();
            //    $("#lblEventDate").text('Event Date:');
            //}
        },
        RecurrencePatternChanged: function () {

            var pattern = $(this).val();
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