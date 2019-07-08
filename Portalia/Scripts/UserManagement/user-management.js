var userManagement = (function() {

    var keyUpTimeout = null;
    var pageSize = 10; // Page size

    var showIndicator = function() {
        $(".indicator").removeClass("hidden");
        $("#paging-users-container").addClass("hidden");
    };

    var hideIndicator = function() {
        $(".indicator").addClass("hidden");
        $("#paging-users-container").removeClass("hidden");
    };

    // Generate url to get user data based on filter
    var getQueryUrl = function (pageNumber) {
        // Declare variables, get all filters
        var isEmployee = $(".table__select--employee-filter").find("option:selected").val(), // Get filtered IsEmployee value
            workContractStatus =
                $(".table__select--status-filter").find("option:selected").val(), // Get filtered Work Contract Status value
            searchQuery = $(".table__input-filter").val(), // Get filtered Search value
            url = $("#url-search-user").val() + // Get Url to make a GET to server to get data
                "?IsEmployee=" + isEmployee +
                "&Status=" + workContractStatus +
                "&SearchUserNameQuery=" + searchQuery +
                "&PageNumber=" + (pageNumber === null || pageNumber === undefined ? 1 : pageNumber) +
                "&PageSize=" + pageSize;

        return url;
    };

    // Call ajax to get user data based on filter and page number
    var getFilteredData = function (pageNumber) {
        // Declare variables
        var url = getQueryUrl(pageNumber);

        // Perform ajax to get data
        $.ajax({
            type: "GET",
            url: url,
            beforeSend: function () {
                showIndicator();
            },
            success: function (response) {
                $("#paging-users-container").html(response);
            },
            complete: function () {
                hideIndicator();
            }
        });
    };

    var initializeEvents = function () {

        // Change event for Employee type filter
        $("body").on("change", ".table__select--employee-filter", function () {
            getFilteredData();
        });

        // Keyup event for Search input
        $("body").on("keyup", ".table__input-filter", function () {
            // If user is typing, stop making request
            if (keyUpTimeout) {
                clearTimeout(keyUpTimeout);
            }

            // If user stop typing, delay for 500ms and perform ajax to get data
            keyUpTimeout = setTimeout(function () {
                getFilteredData();
            }, 500);
        });

        // Change event for Work Contract Status filter
        $("body").on("change", ".table__select--status-filter", function () {
            // Get filtered data from server
            getFilteredData();
        });

        // Click event to select page in user management
        $("body").on("click", ".pagination__page-number", function () {
            // Declare variables
            var $pageItem = $(this), // Get clicked a tag
                pageNumber = $pageItem.data("page-number"); // Get clicked page number

            // If user clicks arrow icon and current page is the first page or the last page, do nothing
            if ($pageItem.hasClass("disabled")) {
                return;
            }

            // Get filtered data from server
            getFilteredData(pageNumber);
        });

        // Click event for Close Contract button
        $("body").on("click", ".wc_btn--close-contract", function () {
            var $closeBtn = $(this), // Get clicked Close button
                affectedRowId = $closeBtn.data("affected-row-id"),
                closeContractUrl = $closeBtn.data("url"), // Get Url to close contract
                employeeFullName = $closeBtn.data("employee-full-name"), // Get employee full name
                formContentHtml = "<p>Do you want to close Work Contract form for " +
                    employeeFullName +
                    "?</p><p>This user will not be able to access to form anymore.</p>", // Declare form content HTML of Modal
                closeWorkContractFunc = "userManagement.handleOpenAndCloseWorkContract('" + closeContractUrl + "', '" + affectedRowId + "')"; // Declare function for confirm button of Modal
            // Show confirmation popup.
            // Click No to close popup.
            // Click Yes to perform the passed function.
            modalHelpers.popupConfirmationModal("Close Work Contract form", formContentHtml, "No", "Yes", closeWorkContractFunc);
        });

        // Click event for Open Contract button
        $("body").on("click", ".wc_btn--open-contract", function() {
            var $openBtn = $(this), // Get clicked Close button
                affectedRowId = $openBtn.data("affected-row-id"),
                openContractUrl = $openBtn.data("url"), // Get Url to close contract
                employeeFullName = $openBtn.data("employee-full-name"), // Get employee full name
                formContentHtml = "<p>Do you want to open Work Contract form to " +
                    employeeFullName +
                    "?</p><p>This user will receive a notification email with the link to access it.</p>", // Declare form content HTML of Modal
                openWorkContractFunc =
                    "userManagement.handleOpenAndCloseWorkContract('" + openContractUrl + "', '" + affectedRowId + "')"; // Declare function for confirm button of Modal
            // Show confirmation popup.
            // Click No to close popup.
            // Click Yes to perform the passed function.
            modalHelpers.popupConfirmationModal("Open Work Contract form", formContentHtml, "No", "Yes", openWorkContractFunc);
        });


        var openFileBrowser = function () {           
            var $uploadContractBtn = $(this);
            var $file = $($uploadContractBtn.data("trigger-target"));

            $file.click();
        };

        // Click event for Upload Contract button, this will trigger the input file to appear
        $("body").on("click", ".wc_btn--upload-contract", openFileBrowser);


        // On change event for the input file, select contract to upload
        $("body").on("change", ".wc__file--contract", function () {
            var $fileInput = $(this),
                $file = $fileInput[0],
                fileInputId = $fileInput.attr("id"),
                fileName = $file.files[0].name,
                extension = fileName.split(".").pop().toLowerCase(),
                username = $fileInput.data("fullname"),
                fileSize = $file.files[0].size,
                validExtensions = ["pdf", "doc", "docx"],
                validFileSize = 2097152, // 2MB in bytes
                dataId = $fileInput.data("unique-id");

            var selectAnotherFileBtn = '<button id="btn-re-upload" class="modal__button" data-dismiss="modal">Select another file</button>';

            // Validate extension: .pdf, .doc, .docx
            if (validExtensions.indexOf(extension) === -1) {
                modalHelpers.popupInfoModal("Cannot upload file", "Close", "File extension must be .pdf | .doc | .docx", undefined, selectAnotherFileBtn);
                $('.wc__file--contract').val(null);
                $('#btn-re-upload').on("click", openFileBrowser.bind($("[data-trigger-target='#contract-file-" + dataId + "']")));
                return;
            }

            // File size must be less than 2MB
            if (fileSize > validFileSize) {
                modalHelpers.popupInfoModal("Work Contract", "Close", "File size must be less than 2MB", undefined, selectAnotherFileBtn);
                $('.wc__file--contract').val(null);
                $('#btn-re-upload').on("click", openFileBrowser.bind($("[data-trigger-target='#contract-file-" + dataId + "']")));
                return;
            }

            // Show confirmation popup.
            // Click No to close popup.
            // Click Yes to perform the passed function.
            var formContentHtml = "<p>Please confirm to upload this file to user " +
                    username +
                    ": <strong>" +
                    fileName +
                    "</strong></p>" +
                    "<p>After uploading successfully, this user will be notified via email to download the file in Portal</p>",
                uploadContractFunc = "userManagement.handleUploadContractConfirmation('" + fileInputId + "')";
            modalHelpers.popupConfirmationModal("Upload Work Contract Confirmation", formContentHtml, "Cancel", "Upload file", uploadContractFunc);
        });
    };

    // This function will handle the GET action of passed Url.
    // After receiving the response from server, we replace the content of the affected row
    var handleOpenAndCloseWorkContract = function (url, affectedRowId) {
        // Perform ajax with GET method
        $.ajax({
            type: "GET",
            url: url,
            beforeSend: function () {
                modalHelpers.freezeButtons();
            },
            success: function (response) {
                // First, close the confirmation popup
                modalHelpers.closeModal();

                // If success
                if (response.IsSuccess === true) {
                    // Replace the content of the affected row with data from server
                    $(affectedRowId).replaceWith(response.Data);
                }

                // Message to notify user
                modalHelpers.popupInfoModal("Work Contract", "Close", response.Message);
            },
            complete: function () {

            }
        });
    };

    var handleUploadContractConfirmation = function (fileInputId) {
        // Prepare data to upload contract
        var $file = $("#" + fileInputId),
            $fileInput = $file[0],
            formData = new FormData(),
            userId = $file.data("user-id"),
            uploadUrl = $file.data("upload-url");

        // Append Key and Value to map view model in Controller
        formData.append("UserId", userId);
        formData.append("FolderType", 1);
        formData.append("Contract", $fileInput.files[0]);

        $.ajax({
            type: "POST",
            url: uploadUrl,
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            beforeSend: function () {
                modalHelpers.freezeButtons();
            },
            success: function (response) {
                //Re-load table
                getFilteredData();

                modalHelpers.closeModal();
                modalHelpers.popupInfoModal("Upload Work Contract", "Close", response.Message);

                $('#modalId').on('hidden.bs.modal',
                    function() {
                        var $tr = $("#tr-" + userId);

                        $tr.replaceWith(response.Data);
                    });
            },
            error: function (e) {
                console.log(e);
            },
            complete: function () {
            }
        });
    };

    return {
        initializeEvents: initializeEvents,
        handleOpenAndCloseWorkContract: handleOpenAndCloseWorkContract,
        handleUploadContractConfirmation: handleUploadContractConfirmation
    };

})();
