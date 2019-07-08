(function () {
    $.fn.editable.defaults.mode = 'inline';
    $(document).ready(function () {
        var euList = ["AT", "BE", "BG", "CY", "CZ", "DE", "DK", "EE", "ES", "FI", "FN", "FR", "GB", "GR", "HR", "HU", "IE", "IT", "LT", "LU", "LV", "MT", "NL", "PL", "PT", "RO", "SE", "SI", "SK"];

        var nationalitiesValue = $("#Nationality > a").text();
        var socialsecuritynumberSelector = $("#Socialsecuritynumber");
        var idCardSelector = $("#IDCard");
        var workpermitSelector = $("#Workpermit");
        var swiftSelector = $("#Swift");
        var systemSelector = $("#System > a").text();
        if (nationalitiesValue && (nationalitiesValue === "FR" || nationalitiesValue === "FRANCE")) {
            //socialsecuritynumberSelector.show();
            idCardSelector.show();
        } else if (nationalitiesValue && (nationalitiesValue !== "FR" || nationalitiesValue !== "FRANCE")) {
            //socialsecuritynumberSelector.hide();
            idCardSelector.hide();
        } else {
            //socialsecuritynumberSelector.hide();
            idCardSelector.hide();
        }



        if (nationalitiesValue && ($.inArray(nationalitiesValue, euList) === -1)) {
            workpermitSelector.show();
        } else if (nationalitiesValue && ($.inArray(nationalitiesValue, euList) > 0)) {
            workpermitSelector.hide();
        } else {
            workpermitSelector.hide();
        }
        var american = "3";
        var asian = "4";
        if (systemSelector === american || systemSelector === asian) {
            swiftSelector.show();
        } else {
            swiftSelector.hide();
        }

        var updateTotalMissingField = function () {
            $.get("/Proposal/CountMissingField", function (result) {
                $("#total-mission-fields").text(result);
            });
        };

        var updateTotalMissingSection = function (attributeType) {
            var userId = $("#UserId").val();
            var url = "/portalia/proposal/CountMissingFieldByAttributeType?userId=" + userId + "&attributeType=" + attributeType;
            $.get(url, function (result) {
                $(".missing-fields-" + attributeType).text(result);
            });
        };

        $('#Address > a').on('shown', function (e, editable) {
            helper.AutoCompleteGoogleAddress('#Address > span > div > form > div > div:nth-child(1) > div > input');
        });
        $('#BirthPlace > a').on('shown', function (e, editable) {
            helper.AutoCompleteGoogleAddress('#BirthPlace > span > div > form > div > div:nth-child(1) > div > input', true);
        });
        $('#Birthplaceaddress > a').on('shown', function (e, editable) {
            helper.AutoCompleteGoogleAddress('#Birthplaceaddress > span > div > form > div > div:nth-child(1) > div > input');
        });
        $('.editable-click').editable({
            validate: function (value) {
                if ($.trim(value) === '') {
                    return 'Ce champ est requis';
                }

            },
            success: function (response, newValue) {
                var currentSelector = $(this).parent().closest('div').attr('id');
                var isRequired = $(this).parent().closest('div').attr('data-is-required') === "True";
                var attributeType = $(this).parent().closest('fieldset').attr('id');
                if (newValue === "FR" && currentSelector === "Nationality") {
                    //socialsecuritynumberSelector.show();
                    idCardSelector.show();
                } else if (currentSelector === "Nationality" && newValue !== "FR") {
                    //socialsecuritynumberSelector.hide();
                    idCardSelector.hide();
                }
                if (currentSelector === "Nationality" && $.inArray(newValue, euList) > 0) {
                    workpermitSelector.hide();
                } else if (currentSelector === "Nationality" && $.inArray(newValue, euList) < 0) {
                    workpermitSelector.show();
                }
                if ((newValue === american || newValue === asian) && currentSelector === "System") {
                    swiftSelector.show();
                } else if (currentSelector === "System" && (newValue !== american || newValue !== asian)) {
                    swiftSelector.hide();
                }
                if (isRequired) {
                    updateTotalMissingField();
                    updateTotalMissingSection(attributeType);
                }
                if (currentSelector === "LastName" || currentSelector === "FirstName") {
                    $("#profileUpdatedNotificationModal").modal();
                }
            },
            showbuttons: false,
            inputclass: "input-large",
            onblur: "submit",
            display: function (value, sourceData) {
                var currentSelector = $.fn.editableutils.itemsByValue(value, sourceData);
                var seft = $(this);
                var selectorData = $(this).data();
                if (currentSelector.length > 0 && typeof selectorData.type !== typeof undefined && selectorData.type !== false && selectorData.type == "select") {
                    $.each(sourceData, function (i, v) {
                        if (v.value == value) {
                            seft.html(v.text);
                            return;
                        }
                    });
                } else if (selectorData.type == "date") {
                    if (value) {
                        var date = new Date(value);
                        return seft.html(date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear());
                    }
                } else {
                    return seft.html(value);
                }
            },
        });
        $('.file').click(function (e) {
            e.preventDefault();
            var url = $(this).data('url');
            var isRequiredField = $(this).parent().closest("div").attr("data-is-required") === "True";
            $.get(url, function (htmlContent) {
                $('#myModalContent').html(htmlContent);
                $('#myModal').modal();
                $('#myModal').modal('show');
                $('#myModal').on('hidden.bs.modal', function () {
                    if (isRequiredField) {
                        updateTotalMissingField();
                        updateTotalMissingSection("Document");
                    }
                });
            });
        });
        function checkMissingFields() {
            var personalMissvalue = ($("#Personal > div [data-is-required='True']").length - $("#Personal > div[data-is-required='True'] > a").filter(function () { return $(this).html() != "Empty" && $(this).html() != "" }).length).toString();

            $(".missing-fields-Personal").html(personalMissvalue == 0 ? "" : personalMissvalue);

            var documentHasValueCount = $("#Document > div [data-is-required='True'] > a").filter(function () {
                return $(this).attr("href") != "";
            });
            var documentMissValue = $("#Document > div [data-is-required='True']").length - documentHasValueCount.length;
            $(".missing-fields-Document").html(documentMissValue == 0 ? "" : documentMissValue);
            var bankMissValues = $("#Banks > div [data-is-required='True']").length - $("#Banks > div[data-is-required='True'] > a").filter(function () { return $(this).html() != "Empty" }).length;

            $(".missing-fields-Banks").html(bankMissValues == 0 ? "" : bankMissValues);
        };
        checkMissingFields();
        $(".remove-user-document").click(function () {
            var data = $(this).data();
            var documentElement = $("#" + data.id);
            $.post(data.url, function (response, statusText, xhr) {
                if (statusText == "success") {
                    documentElement.remove();
                }
            });
        });

        // Reload page after uploading profile picture
        $("#myModal").on("hidden.bs.modal", function () {
            location.reload();
        });
    });
})()