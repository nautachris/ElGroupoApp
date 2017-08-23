$(document).ready(function () {
    $("#EventDate").datepicker();

    $("#btnSubmit").on("click", function () {
        $("#frmEditEvent").submit();
    });

    $("#rbMapSelectionAddress").on("change", function () {
        if ($(this).is(':checked')) {
            console.log('address checked');
            $(".row.map-search").show();
            $(".row.manual-search").hide();
            Maps.autocomplete.setTypes(['geocode']);
            //$("#LocationName").val('');
        }
        else {
            //console.log('place checked');
            //$(".row.map-search").hide();
            //$(".row.manual-search").show();
        }
    });

    $("#rbMapSelectionPlace").on("change", function () {
        if ($(this).is(':checked')) {
            $(".row.map-search").show();
            $(".row.manual-search").hide();
            Maps.autocomplete.setTypes(['establishment']);
            $("#LocationName").val('');
        }
        else {

            //$(".row.map-search").hide();
            //$(".row.manual-search").show();
        }
    });



    var gpid = $("#GooglePlaceId").val();



});

EditEvent = {
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
    PlaceChange : function (place) {
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