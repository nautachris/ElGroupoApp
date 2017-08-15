//gmapi key AIzaSyA8AObtWB7hUVmZkx7p6KIt2aXiKMZVXDk
Maps = {
    map: null,
    infoWindow: null,
    $mapDiv: null,
    Init: function (obj) {
        this.$mapDiv = $('#' + obj.mapDiv);
    },
    InitMap: function () {
        this.map = new google.maps.Map(this.$mapDiv[0], {
            zoom: 10,
            center: { lat: 35.11, lng: -106.62 }
        });

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                console.log(position);
                console.log(Maps);
                var pos = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };

                Maps.infoWindow = new google.maps.InfoWindow;
                Maps.infoWindow.setPosition(pos);
                Maps.infoWindow.setContent('Taz Lives Here');
                Maps.infoWindow.open(Maps.map);
                Maps.map.setCenter(pos);
            });



        }
        else {
            alert('your browser sucks balls');
        }
    }
}