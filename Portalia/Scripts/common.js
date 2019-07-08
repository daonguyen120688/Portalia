//Get browser name and version
function GetBrowserInfor() {
    var ua = navigator.userAgent, tem,
        M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
    if (/trident/i.test(M[1])) {
        tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
        return { name: 'ie', version: (tem[1] || '') };
    }
    if (M[1] === 'Chrome') {
        tem = ua.match(/\b(OPR|Edge)\/(\d+)/);
        if (tem != null) return tem.slice(1).join(' ').replace('OPR', 'Opera');
    }
    M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
    if ((tem = ua.match(/version\/(\d+)/i)) != null) M.splice(1, 1, tem[1]);
    return { name: M[0].toLowerCase(), version: M[1].toLowerCase() };
}

//
function ShowWarningIfBrowserIsOutDate(listOfBrowserInfors) {
    var browserInfor = GetBrowserInfor();

    if (browserInfor.name === "ie")
        return true;

    var arrSupport = listOfBrowserInfors.split(",");
    for (i = 0; i < arrSupport.length; i++) {
        var browserDefinition = arrSupport[i].split("-");
        //If browser is detected and version is not set, return false
        if (browserDefinition[0].toLowerCase() === browserInfor.name) {
            if (browserDefinition[1] === "")
                return false;
            var curVersion = parseInt(browserDefinition[1]);
            if (parseInt(browserInfor.version) < curVersion)
                return true;
        }
    }
    return false;
}

function formatNumber(n) {
    // format number 1000000 to 1,234,567
    return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".")
}


function formatCurrency(input, noOfDecimal, blur) {
    //Initilize noOfDecimal if undefined
    if (noOfDecimal === undefined)
        noOfDecimal = 0;
    // validates decimal side
    // and puts cursor back in right position.

    // get input value
    var input_val = input.val();

    // don't validate empty input
    if (input_val === "") { return; }

    // original length
    var original_len = input_val.length;

    // initial caret position 
    var caret_pos = input.prop("selectionStart");

    var decimalNo = "";

    //Initilize default decimal numbers
    for (i = 0; i < noOfDecimal; i++) {
        decimalNo += "0";
    }

    // check for decimal
    if (input_val.indexOf(",") >= 0) {

        // get position of first decimal
        // this prevents multiple decimals from
        // being entered
        var decimal_pos = input_val.indexOf(",");

        // split number by decimal point
        var left_side = input_val.substring(0, decimal_pos);
        var right_side = input_val.substring(decimal_pos);

        // add commas to left side of number
        left_side = formatNumber(left_side);

        // validate right side
        right_side = formatNumber(right_side);

        // On blur make sure {noOfDecimal} numbers after decimal
        if (blur === "blur" && noOfDecimal>0) {
            right_side += decimalNo.replace(",","");
        }

        // Limit decimal to {noOfDecimal} digits
        right_side = right_side.replace(".", "").substring(0, noOfDecimal);

        // join number by .
        if (noOfDecimal > 0)
            input_val = left_side + "," + right_side;
        else
            input_val = left_side;

    } else {
        // no decimal entered
        // add commas to number
        // remove all non-digits
        input_val = formatNumber(input_val);
        input_val = input_val;

        // final formatting
        if (blur === "blur" && noOfDecimal > 0) {
            input_val += ","+decimalNo;
        }
    }

    // send updated string to input
    input.val(input_val);

    // put caret back in the right position
    var updated_len = input_val.length;
    caret_pos = updated_len - original_len + caret_pos;
    input[0].setSelectionRange(caret_pos, caret_pos);
}

var common = (function () {
    function showPageIndicator() {
        $('#ajax-loading').removeClass('ajax-loading-hidden');
    };
    function hidePageIndicator() {
        $('#ajax-loading').addClass('ajax-loading-hidden');
    };

    var downloadWorkContract = function (downloadFileUrl) {
        window.open(downloadFileUrl, "_blank");
    };

    // Work Contract button
    var initializeEventsForWorkContractButton = function () {
        $(".wc__sticky-button-wrapper").on("click", ".download-work-contract-btn", function () {

            var $stickyButton = $(".wc__sticky-button-wrapper"),
                $workContractBtn = $(this),
                downloadFileUrl = $workContractBtn.data("download-wc-url"),
                wcAcknowledgeUrl = $workContractBtn.data("wc-acknowledge-url"),
                redirectedUrl = $workContractBtn.data("url"),
                workContractId = $workContractBtn.data("wc-id");

            $.ajax({
                type: "POST",
                url: wcAcknowledgeUrl,
                data: {
                    workContractId: workContractId
                },
                success: function (data) {
                    console.log(data);
                },
                error: function () {
                    console.log("error");
                }
            });

            modalHelpers.popupModalWithoutCloseButton("Work Contract", "Télécharger", "Cliquez sur le bouton 'Télécharger' pour télécharger votre Work Contract. Vous pouvez également trouver ce document dans Mon activité > <a target='_blank' href='" + redirectedUrl + "'>Contrats</a>.", 'common.downloadWorkContract("' + downloadFileUrl + '")');

            $("#popup-without-close-button").on('hidden.bs.modal',
                function() {
                    $stickyButton.remove();
                });
        });
    };


    return {
        showPageIndicator,
        hidePageIndicator,
        initializeEventsForWorkContractButton: initializeEventsForWorkContractButton,
        downloadWorkContract: downloadWorkContract
    };
})();

(function ($) {
    $.fn.inputFilter = function (inputFilter) {
        return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            }
        });
    };
}(jQuery));

var modalHelpers = (function() {
    // Generate uniqueID (with 5 characters) for modal
    var generateId = function () {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        for (var i = 0; i < 5; i++)
            text += possible.charAt(Math.floor(Math.random() * possible.length));

        return text;
    };

    // Because modal HTML Template has 3 parts, this function removes redundant parts after used.
    var removeUnusedTags = function (html) {

        // If passing empty or undefined HTML, return empty string
        if (html === null || html === undefined) {
            return "";
        }

        // Replace tag {heading} with empty string
        html = html.replace("{heading}", "");
        // Replace tag {closeButtonText} with empty string
        html = html.replace("{closeButtonText}", "");
        // Replace tag {formContent} with empty string
        html = html.replace("{formContent}", "");
        // Replace tag {confirmButton} with empty string
        html = html.replace("{confirmButton}", "");
        // Replace tag {extraButton} with empty string
        html = html.replace("{extraButton}", "");

        return html;
    };

    // Generate confirmation button HTML by passing displayed text and onclick function
    var getConfirmButtonHtml = function (text, func) {
        // Get confirmation button HTML template
        var confirmBtnHtml = '<button class="modal__button" onclick="{func}">{text}</button>';

        // If the passing text is not empty, replace tag {text} with passing text
        if (text !== null && text !== undefined) {
            confirmBtnHtml = confirmBtnHtml.replace("{text}", text);
        }

        // If the passing text is not empty, replace tag {text} with passing text
        if (func) {
            // If user pass the function which use "", we replace "" with '' to correct synctax
            func = func.replace(/"/g, "'");
            confirmBtnHtml = confirmBtnHtml.replace("{func}", func);
        }

        return confirmBtnHtml;
    };

    // Append heading to html template
    var appendHeading = function (html, heading) {
        // Replace the tag {heading} with passing heading
        html = html.replace("{heading}", heading);
        return html;
    };

    // Append closeButtonText to html template
    var appendCloseButtonText = function (html, closeButtonText) {
        // Replace the tag {heading} with passing closeButtonText
        html = html.replace("{closeButtonText}", closeButtonText);
        return html;
    };

    // Append form content (HTML or just a string) to html template
    var appendFormContent = function (html, formContent) {
        // Replace the tag {formContent} with passing formContent
        html = html.replace("{formContent}", formContent);
        return html;
    };

    // Append confirmation button HTML to html template by passing displayed text and onclick Func
    var appendConfirmBtn = function (html, btnConfirmText, btnConfirmFunc) {
        // Get confirmation button HTML template
        var btnConfirmHtml = getConfirmButtonHtml(btnConfirmText, btnConfirmFunc);
        // Replace the tag {confirmButton} with the template above
        html = html.replace("{confirmButton}", btnConfirmHtml);
        return html;
    };


    // Append extra button HTML to html template by passing displayed text and onclick Func
    var appendExtraButton = function (html, extraButton) {
        // Replace the tag {extraButton} with the template above
        html = html.replace("{extraButton}", extraButton);
        return html;
    };

    // Return modal HTML template with id = dynamicModal
    // HOW TO USE: after getting template, you have to replace {heading}, {formContent}, {confirmButton} with your custom value
    // Remember, it's just a HTML template
    var getModalHtmlTemplate = function (modalId) {
        var html = '<div id="' + modalId + '" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="confirm-modal" aria-hidden="true">';
        html += '<div class="modal-dialog">';
        html += '<div class="modal__content">';
        html += '<div class="modal__header">';
        html += '<a class="modal__close" data-dismiss="modal">×</a>';
        html += '<h4>{heading}</h4>';
        html += '</div>';
        html += '<div class="modal__body">';
        html += '{formContent}';
        html += '</div>';
        html += '<div class="modal__footer">';
        html += '{extraButton}'; // extra button to re-upload file
        html += '<button class="modal__button modal__button--alt" data-dismiss="modal">{closeButtonText}</button>';
        html += '{confirmButton}';
        html += '</div>';  // content
        html += '</div>';  // dialog
        html += '</div>';  // footer
        html += '</div>';  // modalWindow

        return html;
    };

    // Make modal visible to users by passing HTML, 
    // This also initialize the event on hidden of modal. After closing, it automatically removes itself from DOM
    var showModal = function (html, modalId) {
        // Append HTML to body
        $("body").append(html);

        // Initialize modal
        $("#" + modalId).modal();
        // Show modal
        $("#" + modalId).modal('show');

        // Initialize event that make modal automatically destroy and remove itself from DOM after closing
        $("#" + modalId).on('hidden.bs.modal',
            function (e) {
                $(this).remove();
            });
    };

    var popupModalWithoutCloseButton = function (heading, funcButtonText, message, callbackFunc) {
        //var modalId = generateId();
        var modalId = "popup-without-close-button";
        var simpleModalHtml = getModalHtmlTemplate(modalId);

        // Append heading to HTML template
        simpleModalHtml = appendHeading(simpleModalHtml, heading);
        // Append form content to HTML template
        simpleModalHtml = appendFormContent(simpleModalHtml, message);
        if (callbackFunc !== undefined) {
            simpleModalHtml = appendConfirmBtn(simpleModalHtml, funcButtonText, callbackFunc);
        }
        // Remove close button from HTMl template
        simpleModalHtml =
            simpleModalHtml.replace(
                '<button class="modal__button modal__button--alt" data-dismiss="modal">{closeButtonText}</button>',
                '');
        // Remove unused tags from HTML template
        simpleModalHtml = removeUnusedTags(simpleModalHtml);
        // Show modal to make it visible to users
        showModal(simpleModalHtml, modalId);
    };

    // Make Info modal visible to users by passing heading and message, after closing, it automatically removes itself from DOM
    // Make sure you pass the unique modalId
    var popupInfoModal = function (heading, closeButtonText, message, callbackFunc, extraButton) {
        //var modalId = generateId();
        var modalId = "modalId";
        var simpleModalHtml = getModalHtmlTemplate(modalId);

        // Append heading to HTML template
        simpleModalHtml = appendHeading(simpleModalHtml, heading);

        //// Append extra button if exist
        if (extraButton !== undefined) {
            simpleModalHtml = appendExtraButton(simpleModalHtml, extraButton);
        }

        // Append close button text to HTML template
        simpleModalHtml = appendCloseButtonText(simpleModalHtml, closeButtonText);
        // Append form content to HTML template
        simpleModalHtml = appendFormContent(simpleModalHtml, message);
        if (callbackFunc !== undefined) {
            simpleModalHtml = appendConfirmBtn(simpleModalHtml, closeButtonText, callbackFunc);
        }
        // Remove unused tags from HTML template
        simpleModalHtml = removeUnusedTags(simpleModalHtml);
        // Show modal to make it visible to users
        showModal(simpleModalHtml, modalId);
    };

    // Make Conformation Modal visible to users by passing heading, firmContent (HTML or Non-HTML), confirmation button text, confirmation button onclick function
    // After closing, it automatically removes it own from DOM
    // Make sure you pass the unique modalId
    var popupConfirmationModal = function (heading, formContent, btnCloseText, btnConfirmText, btnConfirmFunc) {
        var modalId = generateId();
        var modalHtml = getModalHtmlTemplate(modalId);

        // Append heading to HTML template
        modalHtml = appendHeading(modalHtml, heading);
        // Append close button text to HTML template
        modalHtml = appendCloseButtonText(modalHtml, btnCloseText);
        // Append form content to HTML template
        modalHtml = appendFormContent(modalHtml, formContent);
        // Append confirmation button to HTML template
        modalHtml = appendConfirmBtn(modalHtml, btnConfirmText, btnConfirmFunc);
        // Remove unused tags from HTML template
        modalHtml = removeUnusedTags(modalHtml);
        // Show modal to make it visible to users
        showModal(modalHtml, modalId);
    };

    // Close modal
    var closeModal = function () {
        var modal = $(".modal"); // Select modal

        // If modal found, hide it
        if (modal) {
            modal.modal("hide"); // Hide it
        }
    };

    var freezeButtons = function() {
        var $buttons = $(".modal__button");

        $buttons.attr("disabled", true);
        $buttons.addClass("disabled");
        $buttons.html("");
        $buttons.append("<i class='fa fa-spinner fa-spin'></i>");
    };
    
    return {
        showModal: showModal,
        popupInfoModal: popupInfoModal,
        popupConfirmationModal: popupConfirmationModal,
        closeModal: closeModal,
        freezeButtons: freezeButtons,
        popupModalWithoutCloseButton: popupModalWithoutCloseButton
    };    
})();