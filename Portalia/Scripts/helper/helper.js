var helper = {};

$('[data-toggle="popover"]').popover();

(function (helper) {
    helper.showAjaxLoading = function () {
        $('#ajax-loading').show();
    };
    helper.hideAjaxLoading = function () {
        $('#ajax-loading').hide();
    };
    helper.AutoCompleteGoogleAddress = function (selector, isShowcity) {
        var placeSearch, autocomplete;

        function initAutocomplete() {
            // Create the autocomplete object, restricting the search to geographical
            // location types.
            autocomplete = null;

            if (isShowcity) {
                autocomplete = new google.maps.places.Autocomplete(
                /** @type {!HTMLInputElement} */(document.querySelector(selector)),
                    { types: ['(cities)'] });
            } else {
                autocomplete = new google.maps.places.Autocomplete(
                    /** @type {!HTMLInputElement} */(document.querySelector(selector)),
                    { types: ['geocode'] });
            }
            // When the user selects an address from the dropdown, populate the address
            // fields in the form.
            google.maps.event.addListener(autocomplete, 'place_changed', function () {
                fillInPlace();
            });
        }

        function fillInPlace() {
            try {
                place = autocomplete.getPlace();
                //var $address = $(place['adr_address']);
                $(selector).val(place.formatted_address);
                $(selector).data("is-valid-address", true);
            } catch (e) {

            } 
        }

        $(selector).on('focusout', function (e) {
            setTimeout(function () {
                if ($(selector).data("is-valid-address") === false || $(selector).data("is-valid-address") === undefined || $(selector).data("is-valid-address") === null) {
                    console.log($(selector).val());
                    $(selector).val('');
                }
                $(selector).data("is-valid-address", false);
            }, 0);
        });

        // Bias the autocomplete object to the user's geographical location,
        // as supplied by the browser's 'navigator.geolocation' object.
        function geolocate() {
            initAutocomplete();
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    var geolocation = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };
                    var circle = new google.maps.Circle({
                        center: geolocation,
                        radius: position.coords.accuracy
                    });
                    autocomplete.setBounds(circle.getBounds());
                });
            }
        }

        //Init
        geolocate();
    };

    helper.ShowModal = function (options) {
        //default options value
        options = options || {};
        options.onInit = options.hasOwnProperty('onInit') ? options.onInit : function () {
            return true;
        };
        options.modalWidth = options.hasOwnProperty('modalWidth') ? options.modalWidth : options.modalWidth = 650;

        var isShow = options.onInit();
        $("#myModal > div").css("width", options.modalWidth);
        if (isShow) {
            helper.showAjaxLoading();

            $.get(options.url, function (html) {
                $('#myModalContent').html(html);
                $('#myModal').modal();
                $('#myModal').modal('show');
                helper.hideAjaxLoading();
            });
            //Onclose event modal
            if (options.hasOwnProperty("onClosed") && typeof options.onClosed == "function") {
                $('#myModal').on('hidden.bs.modal', function () {
                    options.onClosed();
                });
            }
        }
    };
    helper.InitDatePicker = function () {
        $('.datepicker').datepicker({
            format: 'dd/mm/yyyy'
        });
    };
    helper.ShowAlert = function (style, message) {

        var alertStyle = { success: "#alert-success", fail: "#alert-fail" };
        var messageTemplate = { success: "<strong>Success!</strong>" + message, fail: "<strong>Fail!</strong>" + message };
        $(alertStyle[style]).html(messageTemplate[style]);
        $(alertStyle[style]).show();
        $(alertStyle[style]).fadeTo(2000, 500).slideUp(500, function () {
            $(alertStyle[style]).slideUp(500);
        });
    };

    helper.HideModal = function () {
        $('#myModal').modal('toggle');
    };

})(helper || (helper = {}));


$('.datepicker').datepicker({
    format: 'dd/mm/yyyy'
});

