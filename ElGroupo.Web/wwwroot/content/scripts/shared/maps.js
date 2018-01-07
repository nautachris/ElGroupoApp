//gmapi key AIzaSyA8AObtWB7hUVmZkx7p6KIt2aXiKMZVXDk
Maps = {
    initOnLoad: true,
    placesService: null,
    map: null,
    autocomplete: null,
    infoWindow: null,
    infoWindowContent: null,
    activePlace: null,
    marker: null,
    $mapDiv: null,
    drawingManager: null,
    $txtAutocomplete: null,
    placeChangeCallback: null,
    mapLoadedCallback: null,
    showDrawTools: false,
    drawMarker: null,
    createDrawTools: false,
    Geocoder: null,
    RemoveMarker: function () {
        if (Maps.marker !== null) Maps.marker.setMap(null);
        if (Maps.drawMarker !== null) Maps.drawMarker.setMap(null);
    },
    UpdateDrawingVisibility: function (show) {
        if (Maps.drawingManager === null) Maps.InitDrawingManager();
        if (Maps.drawMarker !== null) {
            //console.log('removing marker');
            Maps.drawMarker.setMap(null);
        }
        if (show === true) {
            Maps.drawingManager.setMap(Maps.map);
        }
        else {
            Maps.drawingManager.setMap(null);
        }
        Maps.drawingManager.setOptions({
            drawingControl: show
        });
    },
    GeocodeCoordinate: function (latitude, longitude) {
        if (Maps.Geocoder === null) Maps.Geocoder = new google.maps.Geocoder();
        var ll = { lat: latitude, lng: longitude };
        Maps.Geocoder.geocode({ location: ll }, function (results, status) {
            if (results.length === 0) {
                alert('No Geocode Results Found!');
                return;
            }
            Maps.activePlace = results[0];
            Maps.placeChangeCallback(Maps.activePlace)
            //console.log('geocode results');
            //console.log(status);
            //console.log(results);

        });
    },
    GeocodeAddress: function (addressText) {
        if (Maps.Geocoder === null) Maps.Geocoder = new google.maps.Geocoder();
        console.log('geocode address: ' + addressText);
        Maps.Geocoder.geocode({ address: addressText }, function (results, status) {
            if (results.length === 0) {
                alert('No Geocode Results Found!');
                return;
            }
            Maps.activePlace = results[0];
            Maps.SetInfoWindow();
            Maps.placeChangeCallback(Maps.activePlace)


        });
    },
    Init: function (obj) {
        this.$mapDiv = $('#' + obj.mapDiv);
        if (obj.txtAutocomplete) this.$txtAutocomplete = $("#" + obj.txtAutocomplete);
        if (obj.placeChangeCallback) this.placeChangeCallback = obj.placeChangeCallback;
        if (obj.mapLoadedCallback) this.mapLoadedCallback = obj.mapLoadedCallback;
        if (obj.showDrawTools) this.showDrawTools = obj.showDrawTools;
        if (obj.createDrawTools) this.createDrawTools = obj.createDrawTools;
        //$("#divContactList").on("click", " #tblContacts a", function () {
        $("#" + obj.mapDiv).on("click", "a.select-link", function () {
            alert('place selected!');
        });
    },
    InitDrawingManager: function () {
        Maps.drawingManager = new google.maps.drawing.DrawingManager({
            drawingMode: google.maps.drawing.OverlayType.MARKER,
            drawingControl: Maps.showDrawTools,
            drawingControlOptions: {
                position: google.maps.ControlPosition.TOP_CENTER,
                drawingModes: ['marker']
            },
            //markerOptions: { icon: 'https://developers.google.com/maps/documentation/javascript/examples/full/images/beachflag.png' }
        });
        //this.drawingManager.setMap(this.map);
        google.maps.event.addListener(Maps.drawingManager, 'markercomplete', function (marker) {
            if (Maps.drawMarker !== null) {
                Maps.drawMarker.setMap(null);
            }
            if (Maps.marker !== null) {
                Maps.marker.setMap(null);
            }
            Maps.drawMarker = marker;
            Maps.GeocodeCoordinate(marker.getPosition().lat(), marker.getPosition().lng());
        });
    },
    InitMap: function () {
        if (this.initOnLoad) {
            this.map = new google.maps.Map(this.$mapDiv[0], {
                zoom: 10,
                center: { lat: 35.11, lng: -106.62 }
            });
            if (this.createDrawTools) {
                this.InitDrawingManager();
            }


            this.marker = new google.maps.Marker({
                map: this.map,
                anchorPoint: new google.maps.Point(0, -29)
            });
            this.infoWindow = new google.maps.InfoWindow;
            this.infoWindowContent = document.getElementById('infowindow-content');
            this.infoWindow.setContent(this.infoWindowContent);
            if (this.$txtAutocomplete) Maps.InitAutocomplete(this.$txtAutocomplete);
            if (this.mapLoadedCallback) this.mapLoadedCallback();
        }

    },
    InitMapManual: function () {
        this.map = new google.maps.Map(this.$mapDiv[0], {
            zoom: 10,
            center: { lat: 35.11, lng: -106.62 }
        });
        if (this.createDrawTools) {
            this.InitDrawingManager();
        }


        this.marker = new google.maps.Marker({
            map: this.map,
            anchorPoint: new google.maps.Point(0, -29)
        });
        this.infoWindow = new google.maps.InfoWindow;
        this.infoWindowContent = document.getElementById('infowindow-content');
        this.infoWindow.setContent(this.infoWindowContent);
        if (this.$txtAutocomplete) Maps.InitAutocomplete(this.$txtAutocomplete);
        if (this.mapLoadedCallback) this.mapLoadedCallback();
    },
    SetInfoWindow: function (place) {
        if (place) Maps.activePlace = place;
        if (Maps.activePlace.geometry) {
            Maps.infoWindow.close();
            if (Maps.activePlace.geometry.viewport) {
                Maps.map.fitBounds(Maps.activePlace.geometry.viewport);
            }
            else {
                Maps.map.setCenter(Maps.activePlace.geometry.location);
                Maps.map.setZoom(13);
            }
            
            Maps.marker.setPosition(Maps.activePlace.geometry.location);
            Maps.marker.setMap(Maps.map);
            Maps.marker.setVisible(true);
            //console.log('marker');
            //console.log(Maps.marker);
            //var address = '';
            //if (Maps.activePlace.address_components) {
            //    address = [
            //        (Maps.activePlace.address_components[0] && Maps.activePlace.address_components[0].short_name || ''),
            //        (Maps.activePlace.address_components[1] && Maps.activePlace.address_components[1].short_name || ''),
            //        (Maps.activePlace.address_components[2] && Maps.activePlace.address_components[2].short_name || '')
            //    ].join(' ');
            //}

            //Maps.infoWindowContent.children['place-icon'].src = Maps.activePlace.icon;
            //Maps.infoWindowContent.children['place-name'].textContent = Maps.activePlace.name;
            //Maps.infoWindowContent.children['place-address'].textContent = address;
            //Maps.infoWindow.open(Maps.map, Maps.marker);

        }
    },
    InitAutocomplete: function () {
        this.autocomplete = new google.maps.places.Autocomplete(
            this.$txtAutocomplete[0],
            {
                types: ['geocode']
            }
        );
        this.autocomplete.bindTo('bounds', this.map);
        this.autocomplete.addListener('place_changed', fillInAddress);

        function fillInAddress() {
            Maps.activePlace = Maps.autocomplete.getPlace();
            Maps.placeChangeCallback(Maps.activePlace);
            Maps.SetInfoWindow();

        }

        //function geolocate() {
        //    if (navigator.geolocation) {
        //        navigator.geolocation.getCurrentPosition(function (position) {
        //            var loc = {
        //                lat: position.coords.latitude,
        //                lng: position.coords.longitude
        //            };

        //            var c = new google.maps.Circle({
        //                center : loc,
        //                radius: position.coords.accuracy
        //            });

        //            if (Maps.autocomplete) {
        //                Maps.autocomplete.setBounds(c.getBounds());
        //            }
        //        });
        //    }
        //}
    },
    PlaceSearch: function (pid, callback) {
        var _callback = callback;
        if (this.placesService === null) {
            this.placesService = new google.maps.places.PlacesService(this.map);
        }

        this.placesService.getDetails({
            placeId: pid
        }, function (place, status) {
            _callback(place, status);
        });
    }
}