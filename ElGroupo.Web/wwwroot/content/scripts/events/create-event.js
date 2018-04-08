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
                if ($("#idRecurringYes").is(":checked")) {
                    $('#create-step').text('Step 1 of 5');
                }
                else {
                    $('#create-step').text('Step 1 of 4');
                }
                
                $('#create-step-title').text('Name & Description');
                $('#create-step-description').text("Give your event a name and a brief description about what it's about.");
                $("#btnPreviousStep").hide();
                $("#btnNextStep").show();
                $("#btnNextStep").css('margin-left', '0px');
                $("#divSubmit").hide();
                break;
            case 2:
                $("#btnNextStep").css('margin-left', '45px');
                $("#btnPreviousStep").css('margin-right', '45px');
                if ($("#idRecurringYes").is(":checked")) {
                    $('#create-step').text('Step 2 of 5');
                }
                else {
                    $('#create-step').text('Step 2 of 4');
                }
                $('#create-step-title').text('Date & Time');
                $('#create-step-description').text('Let your invitees know when to attend.');
                $("#btnPreviousStep").show();
                $("#btnNextStep").show();
                $("#divSubmit").hide();
                $("#StartDate").datepicker();
                $("#EndDate").datepicker();
                break;
            case 3:
                $("#btnNextStep").css('margin-left', '45px');
                $("#btnPreviousStep").css('margin-right', '45px');
                $('#create-step').text('Step 3 of 5');
                $('#create-step-title').text('Recurrence');
                $('#create-step-description').text('When and how often is the event occurring?');
                $("#btnPreviousStep").show();
                $("#btnNextStep").show();
                $("#divSubmit").hide();
                break;
            case 4:
                $("#btnNextStep").css('margin-left', '45px');
                $("#btnPreviousStep").css('margin-right', '45px');
                if ($("#idRecurringYes").is(":checked")) {
                    $('#create-step').text('Step 4 of 5');
                }
                else {
                    $('#create-step').text('Step 3 of 4');
                }

                $('#create-step-title').text('Location');
                $('#create-step-description').text('Tell your invitees where they need to be.');
                $("#btnPreviousStep").show();
                $("#btnNextStep").show();
                $("#divSubmit").hide();
                if (!CreateEvent.MapLoaded) {
                    Maps.InitMapManual();
                    CreateEvent.MapLoaded = true;
                }


                break;
            case 5:
                if ($("#idRecurringYes").is(":checked")) {
                    $('#create-step').text('Step 5 of 5');
                }
                else {
                    $('#create-step').text('Step 4 of 4');
                }
                $('#create-step').text('Step 4 of 4');
                $('#create-step-title').text('Check-In');
                $('#create-step-description').text('Tell your invitees how they can check in.');
                $("#btnPreviousStep").show();
                $("#btnPreviousStep").css('margin-right', '0px');
                $("#btnNextStep").hide();
                $("#divSubmit").show();
                break;
        }
    },
    GetStartDate: function () {
        var startDate = new Date($("#StartDate").val());
        var startHour = Number($("#StartHour").val());
        var startMinute = Number($("#StartMinute").val());
        var startAMPM = $("#StartAMPM").val();
        var isPM = startAMPM === 'PM';
        var hour = 0;
        if (isPM) {
            
            if (startHour === 12) {
                hour = 12;
            }
            else {
                hour = startHour + 12;
            }

        }
        else {
            if (startHour === 12) {
                hour = 0;
            }
            else {
                hour = startHour;
            }
        }

        startDate.setHours(hour);
        startDate.setMinutes(startMinute);
        console.log('startdate');
        console.log(startDate);
        return startDate;
    },
    GetEndDate: function () {
        var startDate = new Date($("#EndDate").val());
        var startHour = Number($("#EndHour").val());
        console.log(startHour);
        var startMinute = Number($("#EndMinute").val());
        console.log(startMinute);
        var startAMPM = $("#EndAMPM").val();
        var isPM = startAMPM === 'PM';
        var hour = 0;
        if (isPM) {

            if (startHour === 12) {
                hour = 12;
            }
            else {
                hour = startHour + 12;
            }

        }
        else {
            if (startHour === 12) {
                hour = 0;
            }
            else {
                hour = startHour;
            }
        }

        startDate.setHours(hour);
        startDate.setMinutes(startMinute);
        console.log('enddate');
        console.log(startDate);
        return startDate;

    },
    ValidateStep: function (step) {
        console.log('validate step: ' + step);
        var errors = [];
        if (step === 1) {
            if ($("#Name").val() === '') {
                errors.push('Event Name is Required');
            }
            else if ($("#Name").val().toString().trimLeft().trimRight() === '') {
                errors.push('Event Name is Required');
            }

        }
        else if (step === 2) {
            var sd = CreateEvent.GetStartDate();
            var ed = CreateEvent.GetEndDate();
            if (ed <= sd) errors.push('The event end date must be after the start date');
        }
        else if (step === 3) {
            //recurrance
            if ($("#rbRecurrenceDaily").is(":checked")) {
                if ($("#Recurrence_RecurrenceInterval").val() === '') errors.push('Daily recurrence interval is required');
                if ($("#Recurrence_RecurrenceLimit").val() === '') errors.push('Recurrence limit is required');
                else if (Number($("#Recurrence_RecurrenceLimit").val()) > 25) errors.push('The maximum number of event recurrences is 25');


            }
            else if ($("#rbRecurrenceWeekly").is(":checked")) {
                if ($("#Recurrence_RecurrenceInterval").val() === '') errors.push('Weekly recurrence interval is required');
                if ($("#Recurrence_RecurrenceLimit").val() === '') errors.push('Recurrence limit is required');
                else if (Number($("#Recurrence_RecurrenceLimit").val()) > 25) errors.push('The maximum number of event recurrences is 25');
                console.log('days selected');
                console.log($(".days-of-week :checkbox:checked").length);
                if ($(".days-of-week :checkbox:checked").length === 0) {
                    errors.push('At least one day of the week must be selected for weekly recurring events');

                }
            }
            else {
                if ($("#Recurrence_RecurrenceInterval").val() === '') errors.push('Monthly recurrence interval is required');
                if ($("#Recurrence_RecurrenceLimit").val() === '') errors.push('Recurrence limit is required');
                else if (Number($("#Recurrence_RecurrenceLimit").val()) > 25) errors.push('The maximum number of event recurrences is 25');

                if ($("#Recurrence_DaysOfMonth").val() === '') errors.push('There must be at least one day of the month selected for a monthly recurring event');
                var daysOfMonth = $("#Recurrence_DaysOfMonth").val();
                console.log(daysOfMonth);
                var splt = daysOfMonth.toString().split(',');
                console.log(splt);

                for (var x = 0; x < splt.length; x++) {
                    console.log(splt[x]);
                    var testInt = Number(splt[x]);
                    if (isNaN(testInt)) errors.push('There are invalid days of the month entered: ' + splt[x]);
                    else if (testInt > 31) errors.push('An invalid day of the month has been entered: ' + splt[x]);
                }

            }
        }
        else if (step === 4) {
            if ($("#GooglePlaceId").val() === '') errors.push('A Google Place reference is required');

        }
        else if (step === 5) {
            if ($("#rbVerifyPasswordOrLocation").is(":checked")) {
                if ($("#VerificationCode").val() === '') errors.push('Password is required');
            }
            else if ($("#rbVerifyPassword").is(":checked")) {
                if ($("#VerificationCode").val() === '') errors.push('Password is required');

                //loc-tolerance-custom
                if ($("#loc-tolerance-custom").is(":checked")) {
                    if ($("#LocationTolerance").val() === '') errors.push('Custom location tolerance is required');
                }
            }

        }
        console.log(errors);
        if (errors.length > 0) {
            $("#divValidationSummary ul li").remove();
            for (var x = 0; x < errors.length; x++) {
                $("#divValidationSummary ul").append('<li>' + errors[x] +'</li>');
            }
            $("#divValidationSummary").show();
            return false;
        }
        else {
            $("#divValidationSummary").hide();
            return true;
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
        $("#btnNextStep").css('margin-left', '0px');
        $("#btnNextStep").on("click", function () {
            if (!CreateEvent.ValidateStep(CreateEvent.ActiveStep)) return;
            if (CreateEvent.ActiveStep === 5) return;
            if (CreateEvent.ActiveStep === 2) {
                //if recur, set to 3
                //if not, set to 2
                if ($("#idRecurringYes").is(":checked")) {
                    console.log('recur yes');
                    CreateEvent.ActiveStep = 3;
                }
                else {
                    console.log('recur no');
                    CreateEvent.ActiveStep = 4;
                }
            }
            else {
                CreateEvent.ActiveStep++;
            }
            
            CreateEvent.LoadStep();

        });
        $("#btnPreviousStep").on("click", function () {
            if (CreateEvent.ActiveStep === 1) return;
            if (CreateEvent.ActiveStep === 4) {
                //if recur, set to 3
                //if not, set to 2
                if ($("#idRecurringYes").is(":checked")) {
                    CreateEvent.ActiveStep = 3;
                }
                else {
                    CreateEvent.ActiveStep = 2;
                }
            }
            else {
                CreateEvent.ActiveStep--;
            }
            
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
            if (!CreateEvent.ValidateStep(5)) return;


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