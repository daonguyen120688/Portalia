// 3.1.0

//Begin Drag and Drop
var FileUploader = (function ($, window, document, undefined) {
    // feature detection for drag&drop upload
    'use strict';
    var init = function () {
        var html = $("html");
        if (!html.hasClass("js")) {
            html.addClass("js");
        }

        var isAdvancedUpload = function () {
            var div = document.createElement('div');
            return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'FileReader' in window;
        }();

        // applying the effect for every form
        $('.box:not(.has-advanced-upload)').each(function () {
            var $form = $(this),
				$input = $form.find('input[type="file"]'),
				$label = $form.find('label'),
				$labelOld = $form.find('label').html(),
				$errorMsg = $form.find('.box-error span'),
				$restart = $form.find('.box-restart'),
				autosubmit = $form.data("autosubmit"),
				droppedFiles = false,
				showFiles = function (files) {
				    if (!files.length) return;
				    $label.text(files.length > 1 ? ($input.attr('data-multiple-caption') || '').replace('{count}', files.length) : files[0].name);
				},
				triggerFormSubmit = function () {
				    if (autosubmit) {

				        var formActionPrevious = $form.attr('action');
				        var formAction = $form.attr('action');
				        if (formAction.toLowerCase().indexOf("categoryid") < 0) {
				            formAction += "&CategoryId=" + $("#CategoryId").val();
				        }
				        $form.attr('action', formAction);

				        $form.trigger('submit');

				        $form.attr('action', formActionPrevious);
				    }
				};

            // letting the server side to know we are going to make an Ajax request
            if (autosubmit)
                $form.append('<input type="hidden" name="ajax" value="1" />');

            // automatically submit the form on file select
            $input.on('change', function (e) {
                showFiles(e.target.files || e.target.value);

                triggerFormSubmit();
            });


            // drag&drop files if the feature is available
            if (isAdvancedUpload) {
                $form
				.addClass('has-advanced-upload') // letting the CSS part to know drag&drop is supported by the browser
				.on('drag dragstart dragend dragover dragenter dragleave drop', function (e) {
				    // preventing the unwanted behaviours
				    e.preventDefault();
				    e.stopPropagation();
				})
				.on('dragover dragenter', function () //
				{
				    $form.addClass('is-dragover');
				})
				.on('dragleave dragend drop', function () {
				    $form.removeClass('is-dragover');
				})
				.on('drop', function (e) {
				    droppedFiles = e.originalEvent.dataTransfer.files; // the files that were dropped
				    showFiles(droppedFiles);

				    triggerFormSubmit(); // automatically submit the form on file drop
				});
            }


            // if the form was submitted

            $form.on('submit', function (e) {
                // preventing the duplicate submissions if the current one is in progress
                if ($form.hasClass('is-uploading')) return false;

                $form.addClass('is-uploading').removeClass('is-error').removeClass('is-success').removeClass('is-init');

                if (isAdvancedUpload) // ajax file upload for modern browsers
                {
                    e.preventDefault();

                    // gathering the form data
                    var ajaxData = new FormData($form.get(0));
                    if (droppedFiles) {
                        $.each(droppedFiles, function (i, file) {
                            ajaxData.append($input.attr('name'), file);
                        });
                    }

                    // ajax request
                    var firstFormSubmit = $form.find("[data-edm-url]");
                    var formAction;
                    if (firstFormSubmit.length === 0) {
                        formAction = $form.attr('action');
                        if (formAction.toLowerCase().indexOf("categoryid") < 0) {
                            formAction += "&CategoryId=" + $("#CategoryId").val();
                        }
                    } else {
                        formAction = firstFormSubmit.data().edmUrl;
                    }

                    $.ajax(
					{
					    url: formAction,
					    type: $form.attr('method'),
					    data: ajaxData,
					    dataType: 'json',
					    cache: false,
					    contentType: false,
					    processData: false,
					    complete: function () {
					        $form.removeClass('is-uploading');
					    },
					    success: function (data) {
					        $form.addClass(data.status === "success" ? 'is-success' : 'is-error');
					        if (data.status === "warning" || data.status === "danger") {
					            $errorMsg.text(data.message);
					        } else if (data.status === "error") {
					            $errorMsg.text(data.error !== undefined ? data.error : data.message);
					        }
					        var targetId = $form.data("target");
					        if (targetId)
					            $(targetId).submit();

					        var status = data.status;
					        var buttonHash = data.buttonHash;
					        if (data.documentId === 0 && data.documentIds != undefined && data.documentIds !== null) {
					            $.each(data.documentIds, function (index, documentId) {
					                showUploadedFile(status, documentId, buttonHash);
					            });
					        } else {
					            showUploadedFile(status, data.documentId, buttonHash);
					        }
					    },
					    error: function () {
					        showError("An error occurred when uploading your document. Please retry later or contact the 911.");
					    }
					});
                }
            });


            // restart the form if has a state of error/success
            $restart.on('click', function (e) {
                e.preventDefault();
                $form.removeClass('is-error is-success is-init');
                $label.html($labelOld);
                $input.trigger('click');
            });

            // Firefox focus bug fix for file input
            $input
			.on('focus', function () { $input.addClass('has-focus'); })
			.on('blur', function () { $input.removeClass('has-focus'); });
        });
    }
    return {
        init: init
    }
})(jQuery, window, document);
//End Drag and Drop

$('.modal').on('show.bs.modal', function (event) {
    if (exitIfNotDocumentModal(event)) {
        return;
    }

    var button = $(event.relatedTarget); // Button that triggered the modal
    var buttonData = button.data(); // Extract info from data-* attributes

    button.button('loading');

    var filteredCategories = buttonData.filterCategories === undefined ? '' : buttonData.filterCategories;
    var buttonId = button.attr('id') === undefined ? '' : button.attr('id');
    var filterOnTableName = buttonData.filterCategorytablename === "True" ? true : false;

    //var data = { filterCategories: filteredCategories, ButtonId: buttonId, IsFilterCategoriesWithTableName: filterOnTableName };
    var url = button.attr('href');

    url = addParameter(url, 'filterCategories', filteredCategories, false);
    url = addParameter(url, 'ButtonId', buttonId, false);
    url = addParameter(url, 'IsFilterCategoriesWithTableName', filterOnTableName, false);
    url = addParameter(url, 'noModalContent', true, false);

    button.attr('href', url);

});

$('.modal').on('shown.bs.modal', function (event) {
    if (exitIfNotDocumentModal(event)) {
        return;
    }

    var button = $(event.relatedTarget); // Button that triggered the modal
    var buttonData = button.data(); // Extract info from data-* attributes

    var dropzoneText = buttonData.dropzoneText;
    var hideZone = buttonData.hidezone === "True" ? true : false;

    var hideDeleteBtn = buttonData.hideDeleteBtn === "True" ? true : false;
    var showPrintBtn = buttonData.showPrintBtn === "False" ? false : true;
    var showValidation = buttonData.showValidation === "True" ? true : false;

    var buttonHashCode = buttonData.buttonhash;
    var modal = $(this);

    $('.modal').on('loaded.bs.modal', function (eventLoaded) {
        if (exitIfNotDocumentModal(event)) {
            return;
        }
        initModalAfterLoaded(modal, dropzoneText, hideZone, hideDeleteBtn, showPrintBtn, showValidation, buttonHashCode);
    });

    button.button('reset');
});


function initModalAfterLoaded(modal, dropzoneText, hideZone, hideDeleteBtn, showPrintBtn, showValidation, buttonHashCode) {

    if (dropzoneText !== undefined && dropzoneText !== "") {
        $("label[for='file'] > strong").text(dropzoneText);
    }

    if (typeof loadAllSelect2 !== 'undefined') {
        //TODO migrate to load mass action from AA
        loadMassActionEDM();
    } else {
        console.log("Document >> loadMassActionEDM disabled");
    }


    if (hideZone) {
        if (modal.find('#documentUpload').length) {
            modal.find('#documentUpload').remove();
        }
        if (modal.find('#defaultCategory').length) {
            modal.find('#defaultCategory').remove();
        }
    }

    var $td;
    if (hideDeleteBtn) {
        if (modal.find('.delete-document-btn').length) {
            $td = modal.find('.delete-document-btn').closest('td');
            $td.css('min-width', $td.outerWidth() - 35);

            modal.find('.delete-document-btn').remove();
        }
    }

    if (!showPrintBtn) {
        if (modal.find('.print-document-btn').length) {
            $td = modal.find('.print-document-btn').closest('td');
            $td.css('min-width', $td.outerWidth() - 35);

            modal.find('.print-document-btn').remove();
        }
    } else {
        loadPrintDiv();
    }

    if (showValidation) {
        modal.find('.validationStatus').show();
    }

    $("#DocumentDocumentList").data("buttonhash", buttonHashCode);

    $(this).on('select2-loaded', function () {
        CheckCategoryOnchange('#CategoryId');
    });
}

$('.modal').on('hidden.bs.modal', function (event) {
    if (exitIfNotDocumentModal(event)) {
        return;
    }

    var button = $(event.relatedTarget); // Button that triggered the modal

    button.button('reset');
});

function exitIfNotDocumentModal(event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    if (button.data("documentmodal") !== true) {
        return true;
    }
    return false;
}


function processFiles(files, url, parent) {
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var urlWithCategory = SetCategoryToPostUrl(url);
        upload(file, urlWithCategory, parent);
    }
};

initFileUpload();

function initFileUpload() {

    $('.inputFile').off().on("change", function () {
        var parent = this.parentElement.parentElement;
        if (this.files.length > 0) {
            filesUploading(this.files, parent);
        }

    });

    $(".fileUpload").on('dragleave', function () {
        $(this).toggleClass('dragged', false);
    });

    $(".fileUpload").off("drop").on("drop", function (event) {
        event.preventDefault();
        filesUploading(event.originalEvent.dataTransfer.files, this);
    });
    $(".fileUpload").on("dragover", function (event) {
        event.preventDefault();
        $(this).toggleClass('dragged', true);
    });
    $(".fileUpload").on('dragenter', function () {
        $(this).toggleClass('dragged', true);

    });
}



function filesUploading(files, parent) {
    var url = $(parent).data('edm-url');
    $(parent).toggleClass('dragged', false);
    $(parent).toggleClass('uploading', true);
    $(parent).append(appendLoading);
    processFiles(files, url, $(parent));
}

function PrepareDocumentDropFile($elmt) {
    var dropzone = $elmt ? $elmt.find(".drop-target")[0] : document.getElementById("drop-target");
    var parent = $elmt ? $elmt.find(".documentUpload") : $('#documentUpload');
    var url = $(dropzone).data('url');
    if (dropzone != null) {
        dropzone.addEventListener('drop', function (event) {
            event.preventDefault();
            parent.toggleClass('dragged', false);
            parent.toggleClass('uploading', true);
            $(parent).append(appendLoading);
            processFiles(event.dataTransfer.files, url, parent);
            return false;
        }, true);
        dropzone.addEventListener("dragover", function (event) {
            event.preventDefault();
        }, true);
        dropzone.addEventListener('dragenter', function () {
            parent.toggleClass('dragged', true);
        }, true);
        dropzone.addEventListener('dragleave', function () {
            parent.toggleClass('dragged', false);
        }, true);
    }
    else {
        // Box select file
    }
}

var upload = function (file, thisurl, parent) {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', thisurl, true);
    xhr.setRequestHeader('X-Filename', file.name);
    xhr.onreadystatechange = function (parameters) {
        var that = parameters.target;
        if (that.readyState === 4) {
            loadAllXeditable();
            loadAllSelect2();
            showInfoRaw(that.responseText, parent);
            parent.toggleClass('uploading', false);
            parent.find('[title=loading]').remove();

        }
    };
    xhr.send(file);
};

function showInfoRaw(text, parent) {
    var textJson = JSON.parse(text);
    var status = textJson.status;
    var message = textJson.message;
    var documentId = textJson.documentId;
    var buttonHash = textJson.buttonHash;
    showUploadedFile(status, documentId, buttonHash);

    var alertMain = $(parent).siblings("#dragalerts");
    CheckBootstrapVersion(status);
    alertMain.append('<div class="alert alert-' + status + '"> <a class="close" data-dismiss="alert">×</a><strong>' +
		((status === "danger" || status === "warning") ? "Error" : "Success") + '!</strong> ' + message + '</div>');
    var alertChildren = alertMain.children();
    setTimeout(function () {
        alertChildren.fadeOut("slow");
        if ($(parent).hasAttr("data-autosubmit") && $(parent).data("autosubmit") === true) {
            $(parent).closest('form').submit();
        }
        setTimeout(function () {
            alertChildren.remove();
        }, 1000);
    }, 6000);
}

function showUploadedFile(status, documentId, buttonHash) {
    if (status === 'success' && documentId !== 0) {
        var list = $('#DocumentDocumentList');
        if (list.length > 0) {
            $.ajax({
                url: list.data('url'),
                type: 'GET',
                data: { documentId: documentId },
                success: function(data) {
                    list.append(data);

                    runCallBack(status, 'upload', buttonHash, documentId);

                    //If any .validationstatus is visible, then show it oo
                    if ($('.validationStatus').is(':visible')) {
                        list.find('.validationStatus').show();
                    }

                    loadAllXeditable();
                    loadAllSelect2();
                    loadPrintDiv();
                }
            });
        } else {
            runCallBack(status, 'upload', buttonHash, documentId);
        }
    } else {
        runCallBack(status, 'upload', buttonHash, documentId);
    }
}

function deleteFileRequest(url) {
    var resp = confirm("Are you sure you want to delete this file ?");
    if (resp) {
        var parent = $('.box');

        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, true);
        xhr.onreadystatechange = function (parameters) {
            var that = parameters.target;
            if (that.readyState === 4) {
                showDeleteInfoRaw(that.responseText, parent);
            }
        };
        xhr.send();
    }
}

function deletePendingFileRequest(url) {
    var resp = confirm("Are you sure you want to delete this file ?");
    if (resp) {
        var xhr = new XMLHttpRequest();

        xhr.onreadystatechange = function (parameters) {
            var that = parameters.target;
            if (that.readyState === 4) {
                deletePendingDocumentRow(that.responseText);
                var $PreviewDiv = $('#InlinePreviewDiv');
                $PreviewDiv.empty();
            }
        };
        xhr.open('POST', url, true);
        xhr.send();
    }
}

function deletePendingDocumentRow(text) {
    var textJson = JSON.parse(text);
    var status = textJson.status;
    var documentId = textJson.documentId;
    var buttonHash = textJson.buttonHash;
    if (status === 'success') {
        $('#fileEntry_' + documentId).remove();
    }
    runCallBack(status, 'delete', buttonHash, documentId);
}
function PreviewPendingDoc(url) {
    var $PreviewDiv = $('#InlinePreviewDiv');
    $.get(url, function (data) {
        $PreviewDiv.html(data);
    });
}

function showDeleteInfoRaw(text, parent) {
    var textJson = JSON.parse(text);
    var status = textJson.status;
    var message = textJson.message;
    var documentId = textJson.documentId;
    var buttonHash = textJson.buttonHash;
    var alertMain = $(parent).siblings("#dragalerts");
    if (status === 'success') {
        $('#fileEntry_' + documentId).remove();
    }

    runCallBack(status, 'delete', buttonHash, documentId);

    CheckBootstrapVersion(status);
    alertMain.append('<div class="alert alert-' + status + '"> <a class="close" data-dismiss="alert">&times;</a><strong>' +
		(status === "danger" ? "Error" : "Success") + '!</strong> ' + message + '</div>');
    loadAllXeditable();
    loadAllSelect2();
    var alertChildren = alertMain.children();
    setTimeout(function () {
        alertChildren.fadeOut("slow");
        setTimeout(function () {
            alertChildren.remove();
        }, 1000);
    }, 5000);

}

function linkPendingFileRequest(url) {
    var resp = confirm("Are you sure you want to link this file to the current object ?");
    if (resp) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, true);
        xhr.onreadystatechange = function (parameters) {
            var that = parameters.target;
            if (that.readyState === 4) {
                deletePendingDocumentRow(that.responseText);
                var $PreviewDiv = $('#InlinePreviewDiv');
                $PreviewDiv.empty();
            }
        };
        xhr.send();
    }
}

function CheckCategoryOnchange(selector) {
    $(selector).on('change', function () {
        var category = $(this);
        var value = category.select2('val');
        CheckCategory(value);
    });
}

function CheckCategory(value) {
    //Need to keep the both for introducing file selector
    var documentUpload = $('#documentUpload, #documentUploadContainer');
    if (value == "0" || value === "") {
        documentUpload.addClass("is-init");
    } else {
        documentUpload.removeClass("is-error").removeClass('is-init');
        documentUpload.find(".box-error span").text("");
    }
}

function SetCategoryToPostUrl(url) {
    var value = $('.modal-body').find('#Category').select2('val');
    if (value !== 0) {
        url += '&categoryid=' + value;
        return url;
    } else {
        return url;
    }
}

function CheckBootstrapVersion(status) {
    var bootstrap3Enabled = (typeof $().emulateTransitionEnd === 'function');
    if (!bootstrap3Enabled && status === 'danger') {
        status = 'error';
    }
    return status;
}

function errorHandler(response, status, xhr) {
    if (status === "error") {
        // TODO
        var msg = "Sorry, an error occured. ";
        if (xhr.status !== "undefined" && xhr !== "Not Found") {
            msg += xhr.status + " " + xhr.statusText;
        }
        else if (xhr.status !== "undefined" && xhr === "Not Found") {
            msg += "Http" + " " + xhr;
        }

        var html = '<div class="modal-header">' +
			'<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h3>Error</h3></div>' +
			'<div class="modal-body">' +
			'<div class="alert alert-error" style="margin-bottom:0; z-index: 9999">' + msg + '</div>' +
			'</div>' +
			'<div class="modal-footer"><span class="btn" data-dismiss="modal">Close</span></div>';
        return html;
    }
    return '';
}

function LoadCountDocument() {
    $("span[data-edm-count-url]").each(function () {
        var self = $(this);
        $.post($(this).data('edm-count-url'), function (e) {
            var callback = self.data('edm-count-onsuccess');
            if (callback) {
                window[callback](e.count, self);
            } else {
                self.html(e.count);
            }
        });
    });
}

// Fetch the upload field
function LoadUploadField() {
    $("div[data-edm-upload]").each(function () {
        var $this = $(this);
        var url = $this.data("edm-upload");

        // dont reload the component many times
        if ($this.find("#documentUpload").length === 0) {
            $.ajax({
                type: "GET",
                url: url,
                success: function (html) {
                    $this.append(html);
                    $this.removeClass("icon-loading");
                    PrepareDocumentDropFile($this);
                }
            });
        }
    });
}

var disableInputButtonInForm = function (submit) {
    $(submit).attr('disabled', 'disabled');
    var fields = $(submit).prevAll('input');

    fields.on('change', function () {
        //If all are filled
        if (!fields.filter(function () { return $.trim(this.value) === ""; }).length)
            $(submit).removeAttr('disabled');
    });
};


//Begin InlinePreview
function loadAllInlinePreview() {
    $('[data-toggle=\"inline-ajax-doc\"]').each(function (i, divContainer) {
        $(divContainer).html(detailsInPopover($(this).data("url"), divContainer.id));
    });
}


loadAllInlinePreview();
loadAllPopoverAjax();
$(".modal").on("loaded.bs.modal", function (e) {
    if (typeof (loadAllInlinePreview) !== "undefined") {
        var Triggerall = false;
        $('[data-toggle=\"inline-ajax-doc\"]').each(function (i, divContainer) {
            if ($("#" + divContainer.id).attr("data-documentloaded_" + divContainer.id) !== "true") {
                Triggerall = true;
            }
        });
        if (Triggerall) {
            loadAllInlinePreview();
        }
    }
    if (typeof (loadAllPopoverAjax) !== "undefined")
        loadAllPopoverAjax();
});


// load inlinePreview for one element
function loadInlinePreview(elemId) {
    var elem = $('#' + elemId);
    alert(elem.id);
    elem.html(detailsInPopover($(this).data("url"), elem.id));
}
//End InlinePreview
//Begin Popover
function loadAllPopoverAjax() {
    $('[data-toggle=\"popover-ajax-doc\"]').each(function (i, e) {
        var divId = "tmp-id-" + $.now();
        var btnDlId = "tmp-btn-" + $.now();
        var $button = $(e);

        $button.popover({
            "html": true,
            "title": "<a class='btn btn-sm btn-success' title='Download file' id='" + btnDlId + "' href=''><i class='fa fa-download'></i> Download</a> <button type='button' class='close' data-dismiss='popover-ajax-doc'><span aria-hidden='true'>x</span></button>",
            "content": function () {
                return detailsInPopover($(this).data("url"), divId, btnDlId);
            },
            "template": "<div class='popover documentPopover' role='tooltip'><h3 class='popover-title documentPopoverTitle'></h3><div class='popover-content documentPopoverContent'></div></div>"
        });

        $button.popover().on('shown.bs.popover', function () {
            $button.data('bs.popover').tip().find('[data-dismiss="popover-ajax-doc"]').on('click', function () {
                $button.popover('hide');
            });
        });
    });
}

function detailsInPopover(link, divId, btnDlId) {
    $.ajax({
        url: link,
        async: true,
        success: function (response) {
            $('#' + divId).html(response.content);
            $('#' + divId).attr("data-documentloaded_" + divId, "true");
            var closestPopover = $('#' + divId).closest('.popover');
            if (closestPopover.length) {
                var firstPopover = closestPopover.first();
                response.width && firstPopover.css('max-width', response.width * 2);
                response.height && firstPopover.css('max-height', response.height * 2);
            }

            //title
            if (btnDlId) {
                if (response.downloadLink == '') {
                    $('#' + btnDlId).replaceWith('<span id="' + btnDlId + '">Download disabled</span>');
                } else {
                    $('#' + btnDlId).attr('href', response.downloadLink);
                }
            }

            $('#' + divId).find('[data-toggle=\"change-ajax-doc\"]').each(function (i, e) {
                $(e).click(function () {
                    detailsInPopover($(this).data("url"), divId, btnDlId);
                });
            });
        }
    });
    var imgPreview = '<img id="imagePreview" style="width: 25rem; height: 25rem;" alt="Loading ..." />';
    if (!$('#' + divId).length) {
        imgPreview = '<div id="' + divId + '">' + imgPreview + '</div>';
    }
    return imgPreview;
}
//End Popover



//Begin Send Email Mass Action

function loadMassActionEDM() {
    if (typeof MassAction == 'undefined') {
        MassActionEDM.load();
    } else {
        MassAction.load();
    }
}

var MassActionEDM = (function ($, document, window, undefined) {
    var _defaults = {
        submit: ".mass-action",
        checkbox: ".mass-checkbox",
        selectAll: ".mass-selectall",
        count: ".mass-selected",
        selector: ""
    };
    var options;

    var selected = function () {
        return $(options.checkbox + ":checked");
    }
    var insertInputs = function () {
        var inputs = [];
        //if ($(this).hasClass("disable-on-click")) {
        //    $(options.submit + ".disable-on-click").attr('disabled', true);
        //}
        selected().map(function (i, element) {
            var name = "[" + i + "]";
            var value = $(element).data('value');

            inputs.push($("<input>").attr({
                "type": "hidden",
                "value": value,
                "name": name
            }));
        });

        $(options.submit).parent().append(inputs);
    };

    var toggleDisabled = function () {
        var checked = selected().length > 0;
        $(options.submit).attr('disabled', !checked);
    }

    var initAction = function () {
        $(options.submit).on('click', insertInputs);
    }

    var updateCount = function () {
        var count = selected().length;
        $(options.count).html(count);
    }

    var initSelectionCheckboxes = function () {
        $(options.checkbox).on('change', toggleDisabled);
        $(options.checkbox).on('change', updateCount);
    }

    var initSelectAll = function () {
        $(options.selectAll).on('click', function (e) {
            var checked = $(this).prop('checked');
            $(options.checkbox).prop('checked', checked);

            var $label = $($(this).parent());
            if (checked)
                $label.attr('title', $label.data('true-title')).tooltip('fixTitle').tooltip('show');
            else
                $label.attr('title', $label.data('false-title')).tooltip('fixTitle').tooltip('show');

            toggleDisabled();
            updateCount();
        });
    }

    function init() {
        options = $.extend(_defaults, $(_defaults.submit).data());

        if (options.selector) {
            for (var key in _defaults) {
                if (_defaults.hasOwnProperty(key)) {
                    options[key] = options.selector + _defaults[key];
                }
            }
        }

        initAction();
        initSelectionCheckboxes();
        initSelectAll();
    }
    return {
        load: init,
        initAction: initAction
    };
}(jQuery, document, window));
//Begin Send Email Mass Action

//Begin Controls Zoom & Rotate

var InlinePreviewControls = (function () {
    var publicMethods = {};

    publicMethods.init = function (container) {
        var previewPicture = $(container).parent().find('#imagePreview');
        previewPicture.on('load', function () {
            previewPicture.guillotine();
        });
        // Bind button actions
        $(container).find('.rotate_left').click(function () {
            previewPicture.guillotine('rotateLeft');
        });
        $(container).find('.rotate_right').click(function () {
            previewPicture.guillotine('rotateRight');
        });
        $(container).find('.fit').click(function () {
            previewPicture.guillotine('fit');
        });
        $(container).find('.zoom_in').click(function () {
            previewPicture.guillotine('zoomIn');
        });
        $(container).find('.zoom_out').click(function () {
            previewPicture.guillotine('zoomOut');
        });
    }

    return publicMethods;
})();

//End Controls Zoom & Rotate

//Id = Id of the document created
function runCallBack(status, callbacktype, buttonHash, id) {
    var callback = "";
    if (status === "success") {
        if (callbacktype === "upload") {
            $("[data-buttonHash=" + buttonHash + "]").each(function () {
                callback = $(this).data().callbackUploadSuccess;
            });
        } else if (callbacktype === 'delete') {
            $("[data-buttonHash=" + buttonHash + "]").each(function () {
                callback = $(this).data().callbackDeleteSuccess;
            });
        }
    } else {
        if (callbacktype === "upload") {
            $('[data-buttonHash=' + buttonHash + "]").each(function () {
                callback = $(this).data().callbackUploadFail;
            });
        } else if (callbacktype === "delete") {
            $("[data-buttonHash=" + buttonHash + "]").each(function () {
                callback = $(this).data().callbackDeleteFail;
            });
        }
    }

    hideTableListFile();

    if (callback !== undefined && callback !== null && callback !== "") {
        if (callback.endsWith(')') || callback.endsWith('}')) {
            setTimeout(callback, 0);
        } else {
            setTimeout(callback + "(" + id + ")", 0);
        }
    }
}

loadPrintDiv();

function loadPrintDiv() {
    $('.print-document-btn:not(.print-loaded)').click(function (context) {
        PrintDiv(context);
    });
    $('.print-document-btn:not(.print-loaded)').addClass("print-loaded");
}

function PrintDiv(context) {
    var docId = context.currentTarget.getAttribute("data-doc-id");
    var $elt = $(context.currentTarget).parent().find("#document-print-" + docId);

    var btn = $elt.parent().find('button');
    btn.button('loading');

    var child = $elt.children('img');

    if (child === undefined || child === null) {
        return false;
    }

    var src = child.data("src");
    //The name of the document to download
    var docName = child.data("doc-name");

    $.ajax(src, {
        dataType: "json",
        context: $elt,
        success: function (data) {
            child.attr('src', 'data:image/png;base64,' + data);
            var contents = $elt.html();

            var frame1 = document.createElement('iframe');
            frame1.name = "frame1";
            frame1.style.position = "absolute";
            frame1.style.top = "-1000000px";
            document.body.appendChild(frame1);

            var frameDoc = frame1.contentWindow ? frame1.contentWindow :
				(frame1.contentDocument.document ? frame1.contentDocument.document : frame1.contentDocument);
            frameDoc.document.open();
            frameDoc.document.write(contents);
            frameDoc.document.close();

            setTimeout(function () {
                var documentTitle = document.title;
                //To save or print the document with the document name and not the current tab name
                document.title = docName;
                window.frames["frame1"].focus();
                window.frames["frame1"].print();
                document.body.removeChild(frame1);
                document.title = documentTitle;

                btn.button('reset');
            }, 500);
        },
        error: function () {
            showError("This file can't print!");
        }
    });
    return false;
}

function appendLoading() {
    return '<div title="loading" style="text-align:center;vertical-align:middle;"><img src="/Document/Content/images/windows-loader.gif" style="width:32px;height:32px;margin-top: 5%;" alt="Loading..."></div>';
}

function hideTableListFile() {
    var list = $('#DocumentDocumentList');
    if (list.has("td").length > 0) {
        list.closest("table").show();
    } else {
        list.closest("table").hide();
    }
}

function UnwrapDiv() {
    $("[data_unwrap], [data-unwrap]").each(function () {
        if ($(this).parent().is("div")) {
            $(this).unwrap();
        }
        if ($(this).data().unwrap != undefined) {
            $(this).removeData("unwrap");
        } else {
            $(this).removeAttr("data_unwrap");
        }
    });
}

/*Helper URL*/
function addParameter(url, parameterName, parameterValue, atStart/*Add param before others*/) {
    var replaceDuplicates = true;
    var urlhash;
    var cl;
    if (url.indexOf('#') > 0) {
        cl = url.indexOf('#');
        urlhash = url.substring(url.indexOf('#'), url.length);
    } else {
        urlhash = '';
        cl = url.length;
    }
    var sourceUrl = url.substring(0, cl);

    var urlParts = sourceUrl.split("?");
    var newQueryString = "";

    if (urlParts.length > 1) {
        var parameters = urlParts[1].split("&");
        for (var i = 0; (i < parameters.length) ; i++) {
            var parameterParts = parameters[i].split("=");
            if (!(replaceDuplicates && parameterParts[0] == parameterName)) {
                if (newQueryString == "")
                    newQueryString = "?";
                else
                    newQueryString += "&";
                newQueryString += parameterParts[0] + "=" + (parameterParts[1] ? parameterParts[1] : '');
            }
        }
    }
    if (newQueryString == "")
        newQueryString = "?";

    if (atStart) {
        newQueryString = '?' + parameterName + "=" + parameterValue + (newQueryString.length > 1 ? '&' + newQueryString.substring(1) : '');
    } else {
        if (newQueryString !== "" && newQueryString != '?')
            newQueryString += "&";
        newQueryString += parameterName + "=" + (parameterValue ? parameterValue : '');
    }
    return urlParts[0] + newQueryString + urlhash;
};

function FileUploaderWrapForm(id, formTag) {
    var fileId = document.getElementById('file-' + id);
    var form = fileId.closest('form');

    if (form === null || form === undefined || form.length === 0) {
        $(fileId.closest('div#div-' + id)).find('> div').wrapAll(formTag);
        form = fileId.closest('form');
        fileId.closest('.box-container').removeAttribute('data-edm-url');
    }

    form.className += 'box';
    form.setAttribute('data-autosubmit', 'true');
    form.setAttribute('method', 'post');
    form.setAttribute('enctype', 'multipart/form-data');

    FileUploader.init();
}