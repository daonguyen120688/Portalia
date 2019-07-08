//var Dropzone = require("enyo-dropzone");
//Dropzone.autoDiscover = false;

// Get the template HTML and remove it from the doumenthe template HTML and remove it from the doument
var previewNode = document.querySelector("#template");
previewNode.id = "";
var previewTemplate = previewNode.parentNode.innerHTML;
previewNode.parentNode.removeChild(previewNode);
var proposalDocument = null;
var url = $("#UrlAction").val();
console.log(url);

var myDropzone = new Dropzone("#dropzone-area", { // Make the whole body a dropzone
    url: url, // Set the url
    thumbnailWidth: 80,
    thumbnailHeight: 80,
    parallelUploads: 20,
    previewTemplate: previewTemplate,
    autoQueue: true, // Make sure the files aren't queued until manually added
    previewsContainer: "#previews", // Define the container to display the previews
    clickable: ".fileinput-button" // Define the element that should be used as click trigger to select files.
});

myDropzone.on("removedfile", function (file) {
    // Hookup the start button
    console.log(file);
    var urlDelete = "";
    if ($('#UrlDelete').val()) {
        urlDelete = $('#UrlDelete').val();
    } else {
        urlDelete = $('#DeleteDocumentUrl').val();
    }
    $.ajax({
        type: 'POST',
        data: { document: proposalDocument },
        url: urlDelete,
        success: function (htmlContent) {

        }
    });
    //var _ref;
    //return (_ref = file.previewElement) != null ? _ref.parentNode.removeChild(file.previewElement) : void 0;
});

myDropzone.on("success", function (file, response) {
    // Hookup the start button
    console.log(response);
    proposalDocument = response.File;
});


myDropzone.on("addedfile", function (file) {
    // Hookup the start button
    file.previewElement.querySelector(".start").onclick = function () { myDropzone.enqueueFile(file); };
});

// Update the total progress bar
myDropzone.on("totaluploadprogress", function (progress) {
    document.querySelector("#total-progress .progress-bar").style.width = progress + "%";
});

myDropzone.on("sending", function (file, xhr, formData) {

    //formData.append("proposalId", $('#ProposalId').val());
    //formData.append("folderType", $('#FolderType').val());

    // Show the total progress bar when upload starts
    document.querySelector("#total-progress").style.opacity = "1";
    // And disable the start button
    file.previewElement.querySelector(".start").setAttribute("disabled", "disabled");
});

// Hide the total progress bar when nothing's uploading anymore
myDropzone.on("queuecomplete", function (progress) {
    document.querySelector("#total-progress").style.opacity = "0";
});

// Setup the buttons for all transfers
// The "add files" button doesn't need to be setup because the config
// `clickable` has already been specified.
document.querySelector("#actions .start").onclick = function () {
    myDropzone.enqueueFiles(myDropzone.getFilesWithStatus(Dropzone.ADDED));
};
document.querySelector("#actions .cancel").onclick = function () {
    myDropzone.removeAllFiles(true);
};