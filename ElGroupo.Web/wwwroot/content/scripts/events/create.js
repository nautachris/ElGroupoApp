$(document).ready(function () {
    $("#EventDate").datepicker({
        //dateFormat: "mm/dd/yyyy",
        //defaultDate: new Date(Date.now())
    });

    $("#btnSubmit").on("click", function () {
        console.log('btn submit click');
        $("#frmCreateEvent").submit();
    });

    $("div.switch-container.is-recurring span").on("click", function () {
        if ($(this).attr('data-replace-val') === 'True') {
            $("#divRecurrence").show();
            $("#lblEventDate").text('Starting Date:');
        }
        else {
            $("#divRecurrence").hide();
            $("#lblEventDate").text('Event Date:');
        }

    });

    //shit related to event recurrence
    $("div.switch-container[data-replace-element='Recurrence_Pattern'] span").on("click", function () {
        console.log('recur pattern click');
        console.log($(this).attr('data-replace-val'));
        var pattern = $(this).attr('data-replace-val');
        switch (pattern) {
            case 'Daily':
                $("#lblInterval").text('Days');
                $(".row.days-of-week").hide();
                break;
            case 'Weekly':
                $("#lblInterval").text('Weeks');
                $(".row.days-of-week").show();
                break;
            case 'Monthly':
                $("#lblInterval").text('Months');
                $(".row.days-of-week").hide();
                break;

        }

    });

    $(".row.days-of-week span[data-day-index]").on("click", function () {
        if ($(this).attr('data-selected') === 'false') $(this).attr('data-selected', 'true');
        else $(this).attr('data-selected', 'false');

        var $chk = $(".row.days-of-week :checkbox[data-day-index=" + $(this).attr('data-day-index') + "]");

        if ($(this).attr('data-selected') === 'true') $chk.prop('checked', true);
        else $chk.prop('checked', false);




    });


    $(".switch-container.checkin-type > span").on("click", function () {

        switch ($(this).attr('data-replace-val')){
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
    });


    $("html").on("click", ".switch-container.map-select > span", function () {
        switch ($(this).attr('data-map-select')) {
            case 'address':
                $(".row.map-search").show();
                $(".row.manual-search").hide();
                Maps.autocomplete.setTypes(['geocode']);
                $("#LocationName").val('');
                $(".row.map-search").show();
                $(".row.manual-search").hide();
                break;
            case 'business':
                $(".row.map-search").show();
                $(".row.manual-search").hide();
                Maps.autocomplete.setTypes(['establishment']);
                $("#LocationName").val('');
                $(".row.map-search").show();
                $(".row.manual-search").hide();
                break;
            case 'manual':
                $(".row.map-search").hide();
                $(".row.manual-search").show();
                break;
        }
    });




});

CreateEvent = {
    PlaceChange: function (place) {
        console.log('in CreateEvent.PlaceChange');
        console.log(place);

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