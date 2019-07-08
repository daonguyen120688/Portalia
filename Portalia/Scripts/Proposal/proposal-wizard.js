(function() {
    $('#smartwizard').smartWizard({
        anchorSettings: {
            enableAllAnchors: true 
        }
    });
    var data = $("#upload-document").data();
    var myDropzone = new Dropzone("#upload-document", { url: data.url });
    myDropzone.on("complete", function (file) {
        $("#user-project-documents").show();
    });

    $("#btnSendProposal").click(function () {
        var urlUpdateProposal = $(this).data();
        var success = $("#proposal-success-alert");
        var danger = $("#proposal-danger-alert");
        var seft = $(this);
        success.hide().html("");
        danger.hide().html("");
        helper.showAjaxLoading();
        var nextStepButton = $(".sw-btn-next");
        var previousStepButton = $(".sw-btn-prev");
        $.post(urlUpdateProposal.url, function (result) {
            if (result.Message === "OK") {
                success.show().html("La proposition a bien été envoyée !");
                seft.hide();
                
                nextStepButton.html("Terminé");
                nextStepButton.removeClass("disabled");
                nextStepButton.attr("data-dismiss", "modal");
                previousStepButton.hide();
            } else {
                danger.show().html(result.Message);
            }
            helper.hideAjaxLoading();
        });
    });

    //var checkUserValidationUrl = $("#CheckUserProfileValidationUrl").val();
    //$.get(checkUserValidationUrl, function (data) {
    //    var success = $("#user-success-alert");
    //    var danger = $("#user-error-alert");
    //    var btnUserProfile=$("#user-profile-link");
    //    success.hide().html("");
    //    danger.hide().html("");
    //    if (data.result) {
    //        success.show().html("Vous avez bien complété votre profil!");
    //        btnUserProfile.hide();
    //    } else {
    //        danger.show().html("Veuillez renseigner ces informations avant la création de votre projet: " + data.error);
    //        btnUserProfile.show();
    //    }
    //});

    $('#myModal').on('hidden.bs.modal', function () {
        location.reload();
    });
})();