//gmapi key AIzaSyA8AObtWB7hUVmZkx7p6KIt2aXiKMZVXDk
Maps = {
    placesService: null,
    map: null,
    autocomplete: null,
    infoWindow: null,
    infoWindowContent: null,
    activePlace: null,
    marker : null,
    $mapDiv: null,
    drawingManager: null,
    $txtAutocomplete: null,
    placeChangeCallback: null,
    mapLoadedCallback: null,
    showDrawTools: false,
    Init: function (obj) {
        this.$mapDiv = $('#' + obj.mapDiv);
        if (obj.txtAutocomplete) this.$txtAutocomplete = $("#" + obj.txtAutocomplete);     
        if (obj.placeChangeCallback) this.placeChangeCallback = obj.placeChangeCallback;
        if (obj.mapLoadedCallback) this.mapLoadedCallback = obj.mapLoadedCallback;
        if (obj.showDrawTools) this.showDrawTools = obj.showDrawTools;
        //$("#divContactList").on("click", " #tblContacts a", function () {
        $("#" + obj.mapDiv).on("click", "a.select-link", function () {
            alert('place selected!');
        });
    },
    InitMap: function () {
        this.map = new google.maps.Map(this.$mapDiv[0], {
            zoom: 10,
            center: { lat: 35.11, lng: -106.62 }
        });
        if (this.showDrawTools) {
            this.drawingManager = new google.maps.drawing.DrawingManager({
                drawingMode: google.maps.drawing.OverlayType.MARKER,
                drawingControl: true,
                drawingControlOptions: {
                    position: google.maps.ControlPosition.TOP_CENTER,
                    drawingModes: ['marker', 'circle']
                },
                markerOptions: { icon: 'https://developers.google.com/maps/documentation/javascript/examples/full/images/beachflag.png' },
                circleOptions: {
                    fillColor: '#ffff00',
                    fillOpacity: 1,
                    strokeWeight: 5,
                    clickable: false,
                    editable: true,
                    zIndex: 1
                }
            });
            this.drawingManager.setMap(this.map);
            google.maps.event.addListener(this.drawingManager, 'markercomplete', function (marker) {
                console.log('marker complete');
                console.log(marker);
                console.log(marker.getPosition().lat());
            });
            google.maps.event.addListener(this.drawingManager, 'circlecomplete', function (circle) {
                console.log('circle complete');
                console.log(circle);
                console.log(circle.getRadius());
            });
        }


        this.marker = new google.maps.Marker({
            map: this.map,
            anchorPoint: new google.maps.Point(0, -29)
        });
        this.infoWindow = new google.maps.InfoWindow;
        this.infoWindowContent = document.getElementById('infowindow-content');
        this.infoWindow.setContent(this.infoWindowContent);
        if (navigator.geolocation) {
            //navigator.geolocation.getCurrentPosition(function (position) {
            //    var pos = {
            //        lat: position.coords.latitude,
            //        lng: position.coords.longitude
            //    };

               
            //    Maps.infoWindow.setPosition(pos);
            //    Maps.infoWindow.setContent('Taz Lives Here');
            //    Maps.infoWindow.open(Maps.map);
            //    Maps.map.setCenter(pos);
            //});



        }
        else {
            alert('your browser sucks balls');
        }

        if (this.$txtAutocomplete) Maps.InitAutocomplete(this.$txtAutocomplete);

        console.log('calling mapLoadedCallback');
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
            Maps.marker.setVisible(true);
            var address = '';
            if (Maps.activePlace.address_components) {
                address = [
                    (Maps.activePlace.address_components[0] && Maps.activePlace.address_components[0].short_name || ''),
                    (Maps.activePlace.address_components[1] && Maps.activePlace.address_components[1].short_name || ''),
                    (Maps.activePlace.address_components[2] && Maps.activePlace.address_components[2].short_name || '')
                ].join(' ');
            }

            Maps.infoWindowContent.children['place-icon'].src = Maps.activePlace.icon;
            Maps.infoWindowContent.children['place-name'].textContent = Maps.activePlace.name;
            Maps.infoWindowContent.children['place-address'].textContent = address;
            Maps.infoWindow.open(Maps.map, Maps.marker);

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
            //console.log(Maps.activePlace);
            Maps.placeChangeCallback(Maps.activePlace);
            Maps.SetInfoWindow();
            //if (Maps.activePlace.geometry) {
            //    Maps.infoWindow.close();
            //    if (Maps.activePlace.geometry.viewport) {
            //        Maps.map.fitBounds(Maps.activePlace.geometry.viewport);
            //    }
            //    else {
            //        Maps.map.setCenter(Maps.activePlace.geometry.location);
            //        Maps.map.setZoom(13);
            //    }
            //    Maps.marker.setPosition(Maps.activePlace.geometry.location);
            //    Maps.marker.setVisible(true);
            //    var address = '';
            //    if (Maps.activePlace.address_components) {
            //        address = [
            //            (Maps.activePlace.address_components[0] && Maps.activePlace.address_components[0].short_name || ''),
            //            (Maps.activePlace.address_components[1] && Maps.activePlace.address_components[1].short_name || ''),
            //            (Maps.activePlace.address_components[2] && Maps.activePlace.address_components[2].short_name || '')
            //        ].join(' ');
            //    }
                
            //    Maps.infoWindowContent.children['place-icon'].src = Maps.activePlace.icon;
            //    Maps.infoWindowContent.children['place-name'].textContent = Maps.activePlace.name;
            //    Maps.infoWindowContent.children['place-address'].textContent = address;
            //    Maps.infoWindow.open(Maps.map, Maps.marker);

            //}
            //console.log(place);
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