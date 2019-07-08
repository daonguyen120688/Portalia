!(function () {
    var finalUrl = null;
    function RedirectToRoot() {
        var isSubmit = $('#IsSubmit').val() === "True" ? true : false;
        if (isSubmit)
            window.location = finalUrl;
    }

    function initDatePicker() {
        $('#DateOfBirth').datepicker({
            autoclose: true,
            clearBtn: true,
            timepicker: false,
            format: 'dd/mm/yyyy'
        });
        $('#ContractStartDate').datepicker({
            autoclose: true,
            clearBtn: true,
            timepicker: false,
            format: 'dd/mm/yyyy',
        });
        $('#ContractEndDate').datepicker({
            autoclose: true,
            clearBtn: true,
            timepicker: false,
            format: 'dd/mm/yyyy'
        });
    }

    function preventKeyBoardInputDatePicker() {
        $('.wc__form-control-datepicker').on('keydown', function (event) {
            event.preventDefault();
        });
    }

    function initSelect2() {

        //Initialize Title
        $("#Title").select2({
            minimumResultsForSearch: Infinity,
            placeholder: 'Veuillez choisir'
        });

        //Initialize CountryOfBirth
        $("#CountryOfBirth").select2({
            placeholder: 'Veuillez choisir'
        });

        //Initialize Nationality
        $("#Nationality").select2({
            placeholder: 'Veuillez choisir'
        });

        //Initialize Country
        $("#Country").select2({
            placeholder: 'Veuillez choisir'
        });

        //Initialize Currency
        $("#Currency").select2({
            placeholder: 'Veuillez choisir'
        });

        //Initialize Basic
        $("#Basic").select2({
            minimumResultsForSearch: Infinity,
            placeholder: 'Veuillez choisir'
        });

        //Initialize Skill
        $('#Skills').select2({
            multiple: true,
            tags: true,
            ajax: {
                url: skillUrl,
                delay: 500,
                dataType: 'json',
                data: function(params) {
                    var query = {
                        name: params.term
                    };

                    return query;
                },
                processResults: function(data) {
                    // Tranforms the top-level key of the response object from 'items' to 'results'
                    return {
                        results: data
                    };
                }
                // Additional AJAX parameters go here; see the end of this chapter for the full code of this example
            },
            placeholder: 'Veuillez choisir'
        });

        //Initialize City
        $('#City').select2({
            ajax: {
                url: cityUrl,
                delay: 500,
                dataType: 'json',
                data: function(params) {
                    var query = {
                        name: params.term,
                        countryCode: $('#Country').val()
                    };

                    return query;
                },
                processResults: function(data) {
                    // Tranforms the top-level key of the response object from 'items' to 'results'
                    return {
                        results: data
                    };
                }
                // Additional AJAX parameters go here; see the end of this chapter for the full code of this example
            },
            placeholder: 'Veuillez choisir'
        });

        // Initialize ClientName
        if (wcStatus === "3") {
            $("#ClientName").select2({
                ajax: {
                    url: clientNameUrl,
                    delay: 500,
                    dataType: 'json',
                    data: function (params) {
                        var query = {
                            name: params.term
                        };

                        return query;
                    },
                    processResults: function (data) {
                        // Tranforms the top-level key of the response object from 'items' to 'results'
                        console.log(data);
                        return {
                            results: data
                        };
                    }
                    // Additional AJAX parameters go here; see the end of this chapter for the full code of this example
                },
                placeholder: 'Veuillez choisir'
            });

            // Initialize onChange of ClientName
            $("#ClientName").on("change", function () {
                var $select = $(this),
                    $clientAddress = $("#AdminAddress");
                var address = null;

                try {
                    address = $select.select2("data")[0].address;

                    if (address) {
                        $clientAddress.val(address);
                    }
                }
                catch(err)
                {
                    address = "";
                }

                
            });
        }
    }

    function initEvents() {
        $('#frmWC').submit(function () {
            return false;
        });

        $('#btnSave').click(function () {
            $('#IsSubmit').val('False');
            $('#HighlightedFields').val(highlightFields);
            SaveOrSubmitForm(saveUrl);
            return false;
        });

        $('#btnSubmit').click(function () {
            $('#IsSubmit').val('True');
            $('#HighlightedFields').val("");
            SaveOrSubmitForm(validationUrl);
            return false;
        });

        $('#ContractEndDate').change(function () {
            $('#frmWC').valid();
        });

        $('#Country').change(function () {
            if ($(this).val().toLowerCase() === "fr") {
                $('#divSocialNumber').addClass('wc__form-item--w-100');
                $('#divSocialNumber').removeClass('wc__form-item--w-50');
                $('#divVisaPermit').addClass('hidden');
                $('#VisaPermitNo').prop('disabled', true);
            }
            else {
                $('#divSocialNumber').removeClass('wc__form-item--w-100');
                $('#divSocialNumber').addClass('wc__form-item--w-50');
                $('#divVisaPermit').removeClass('hidden');
                $('#VisaPermitNo').prop('disabled', false);
            }

            $("#City").val("").trigger('change');
        });

        $("input[data-type='currency']").on({
            keydown: function (evt) {
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                console.log(charCode);
                if (charCode !== 46 && charCode !== 188 && charCode > 31
                    && (charCode < 48 || charCode > 57))
                    return false;

                return true;
            },
            keyup: function () {
                formatCurrency($(this), parseInt($(this).attr('data-decimal')));
            },
            blur: function () {
                formatCurrency($(this), parseInt($(this).attr('data-decimal')), "blur");
            }
        });

        $("#PostCode,#SSN,#VisaPermitNo").inputFilter(function (value) {
            return /^\d*$/.test(value);
        });

        $(document).on('click', '.clsCity .select2', function (e) {
            $('#select2-City-results').html('');
        });
    }

    function SaveOrSubmitForm(serverUrl) {

        var message = "";
        switch (serverUrl) {
            case submitUrl: message = "Merci!<br/>Le formulaire a été envoyé à l'administrateur et est en attente de validation."; finalUrl = redirectUrl; break;
            case validationUrl: message = "Merci!<br/>Le formulaire a été envoyé à l'administrateur et est en attente de validation."; finalUrl = redirectUrl; break;
            case revisionUrl: message = "The request has been sent to user."; finalUrl = redirectUrlAdmin; break;
            case saveUrl:
            default:
                finalUrl = redirectUrl;
                message = "Enregistrez le formulaire avec succès."; break;
        }

        if ($('#frmWC').valid()) {
            var isSubmit = $('#IsSubmit').val() === "True" ? true : false;
            var skills = "",city="";
            if ($('#Skills').val() != null)
                skills = "&Skills=" + $('#Skills').val().join();

            common.showPageIndicator();
            $.ajax({
                type: "POST",
                url: serverUrl,//isSubmit ? submitUrl : saveUrl,
                data: $('[name!=Skills]', $('#frmWC')).serialize() + skills,
                success: function (data) {
                    if (!data.HasError) {

                        common.hidePageIndicator();

                        //Clear tracking change
                        $('#frmWC').clearTrackChanges();

                        modalHelpers.popupInfoModal('Succès', 'Fermer', message);

                        $('#modalId').on('hide.bs.modal', function () {
                            RedirectToRoot();
                        });

                    }
                },
                error: function () {
                    alert("Something went wrong!");
                }
            });
        }
    }

    function onLoad() {
        if (isClearCountry) {
            $("#Country").val("").trigger('change');
        }
        else
            $('#Country').trigger('change');

        if (isClearCountryOfBirth) {
            $("#CountryOfBirth").val("").trigger('change');
        }

        if (isClearNationality) {
            $("#Nationality").val("").trigger('change');
        }
        $('#Skills').val($('#tempSkill').val().split(",")).trigger('change');
        $('#City').val($('#tempCity').val()).trigger('change');
        if (wcStatus === "3") {
            $('#ClientName').val($('#tempClientName').val()).trigger('change');
        }
        if ($('#AllowancesDisplay').val() !== "")
            $('#AllowancesDisplay').trigger('blur');
        $('#Title').val(title).trigger('change');

        if ($('#IsInputDisabled').val() === 'True') {
            // Make form readonly
            $('.wc__form input, .wc__form select, .wc__form textarea').prop('disabled', true);
            $('.wc__form-button').addClass('wc__form-button--disabled');
        } else {
            // Track changes to show warning when reload
            $(".wc__form").initTrackingChanges();
        }

        if (wcStatus === "4") {
            $(highlightFields).parent().addClass("wc__form-control--highlight");
        }
    }

    initDatePicker();
    initSelect2();
    preventKeyBoardInputDatePicker();
    initEvents();
    onLoad();
})();