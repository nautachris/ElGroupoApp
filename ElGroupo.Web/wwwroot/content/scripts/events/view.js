$(document).ready(function () {
});

ViewEvent = {
    IsAddress: false,
    MapLoaded: function () {
        var gpid = $("#GooglePlaceId").val();
        if (gpid === '') return false;
        Maps.PlaceSearch(gpid, function (place, status) {
            console.log(place);
            var isAddress = false;
            for (var x = 0; x < place.types.length; x++) {
                if (place.types[x] === 'street_address') {
                    $(".row.business-name").hide();
                    break;
                }
                if (place.types[x] === 'establishment') {
                    $(".row.business-name").show();
                    $("#spanBusinessName").val(place.name);
                    break;
                }
            }


            Maps.SetInfoWindow(place);

        });
    }
};