(function () {
    $("#upload-document").click(function () {
        var url = $(this).data('url');
        $.get(url, function (htmlContent) {
            $('#myModalContent').html(htmlContent);
            $('.header').css('z-index', '100');
            $('#myModal .modal-dialog').css('margin-top', '20vh');
            $('#myModal').modal();
            $('#myModal').modal('show');
        });
    });

    $('#myModal').on('hidden.bs.modal', function () {
        location.reload(); 
    });

    $(".delete-document").click(function (e) {
        e.preventDefault();
        var self = this;
        var data = $(this).data("document");
        var urlDelete = $(this).data("urlDelete");
        $.ajax({
            type: 'POST',
            data: { document: data },
            url: urlDelete,
            beforeSend: helper.showAjaxLoading,
            complete: helper.hideAjaxLoading,
            success: function (htmlContent) {
                self.closest("tr").remove();
            }
        });
    });
})()