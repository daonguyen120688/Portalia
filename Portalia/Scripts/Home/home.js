(function () {
    $('.navbar-nav [data-toggle="tooltip"]').tooltip();
    $('.navbar-twitch-toggle').on('click', function (event) {
        event.preventDefault();
        $('.navbar-twitch').toggleClass('open');
    });

    $('.nav-style-toggle').on('click', function (event) {
        event.preventDefault();
        var $current = $('.nav-style-toggle.disabled');
        $(this).addClass('disabled');
        $current.removeClass('disabled');
        $('.navbar-twitch').removeClass('navbar-' + $current.data('type'));
        $('.navbar-twitch').addClass('navbar-' + $(this).data('type'));
    });
    $("#CallBack").click(function (e) {
        e.preventDefault();
        var url = $(this).data().url;
        helper.showAjaxLoading();
        $.get(url, function (html) {
            $('#myModalContent').html(html);
            $('#myModal').modal();
            $('#myModal').modal('show');
            helper.hideAjaxLoading();
        });
    });
    $('#scroll-down').on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({ scrollTop: ($(this)).offset().top }, 500, 'linear');
    });
    $("#upload-picture").click(function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        helper.showAjaxLoading();
        $.get(url, function (htmlContent) {
            $('#myModalContent').html(htmlContent);
            $('.header').css('z-index', '100');
            $('#myModal').modal();
            $('#myModal').modal('show');
            helper.hideAjaxLoading();
        });
    });
    $("#get-suggest-time").click(function () {
        var data = $(this).data();
        helper.ShowModal({
            url: data.url,
            onInit: function () {
                return true;
            },
            modalWidth: 1000
        });
    });

    $("#upload-user-document").click(function (event) {
        event.preventDefault();
        var data = $(this).data();
        helper.ShowModal({
            url: data.url,
            onInit: function () {
                return true;
            },
            modalWidth: 1000
        });
    });

    // Show work contract sticky button for the first 10s
    setTimeout(function () {
        $('.wc__sticky-button').parent().removeClass('wc__sticky-button-wrapper--show');
    }, 10000);

    //if (!localStorage.getItem('isHideWCButtonPendingOnCandicate') && $('.wc__sticky-button:not(.wc__sticky-button--yellow)').length > 0) {
    //    localStorage.setItem('isHideWCButtonPendingOnCandicate', true);
    //    setTimeout(function () {
    //        $('.wc__sticky-button:not(.wc__sticky-button--yellow)').parent().removeClass('wc__sticky-button-wrapper--show');
    //    }, 7000);
    //} else {
    //    $('.wc__sticky-button:not(.wc__sticky-button--yellow)').parent().removeClass('wc__sticky-button-wrapper--show');
    //}

    //if (!localStorage.getItem('isHideWCButtonPendingOnManager') && $('.wc__sticky-button--yellow').length > 0) {
    //    localStorage.setItem('isHideWCButtonPendingOnManager', true);
    //    setTimeout(function () {
    //        $('.wc__sticky-button--yellow').parent().removeClass('wc__sticky-button-wrapper--show');
    //    }, 7000);
    //} else {
    //    $('.wc__sticky-button--yellow').parent().removeClass('wc__sticky-button-wrapper--show');
    //}
    
    // Work Contract Validated button
    $(".wc__sticky-button-wrapper--validate").click(function () {
        let $buttonWrapper = $(this);
        $buttonWrapper.append("<i class='wc__sticky-button-icon fas fa-spinner fa-spin fa-lg'></i>");
        // Update status to ValidationAcknowledged
        $.post($('#AcknowledgeValidationUrl').val(), function (result) {
            if (result.IsSuccess) {
                // Slide button to the right and hide
                $buttonWrapper.addClass("wc__sticky-button-wrapper--hide");
                $buttonWrapper.children(".wc__sticky-button-icon").remove();
            }
        });
    });
})();