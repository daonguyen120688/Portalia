window.onload = function () {

    var x = document.cookie;

    if (x == "visiting=true") {
        $('#loader').css('display', 'none');
        return;
    }
    setTimeout(function () {
        $('#loader').fadeOut(400);

    }, 2200);
    document.cookie = "visiting=true";
    return;
};
$('.cta').hover(function () {
    var id = '#description-' + $(this).attr('id');

    $(id).slideDown(100);

}, function () {
    var id = '#description-' + $(this).attr('id');
    $(id).slideUp(100);
}
);



$('.mail-test').click(function () {


    var mail = $('#mail-put').val();

    if (mail == false) {

        $('#mail-put').addClass('mail-false');

        setTimeout(function () { $('#mail-put').removeClass('mail-false'); }, 1500);

        return;

    }

    $("#mail-box").val(mail);

    $('#inscription').fadeIn(200);
    $(document.body).css('overflow', 'hidden');

    setTimeout(function () { $('#form').fadeIn(150); }, 250);

    return;

});



$('.inscription-process').click(function () {

    $('#inscription').fadeIn(200);
    $(document.body).css('overflow', 'hidden');

    setTimeout(function () { $('#form').fadeIn(150); }, 250);
});


/////

$('#close-inscription').click(
    function () {
        $('#form').fadeOut(150);
        setTimeout(function () {
            $('#inscription').fadeOut(200);
            $(document.body).css('overflow', 'inherit');
        }, 250);

    }
);

// Smooth Scroll

$('a[href*="#"]')
    // Remove links that don't actually link to anything
    .not('[href="#"]')
    .not('[href="#0"]')
    .click(function (event) {
        // On-page links
        if (
            location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '')
            &&
            location.hostname == this.hostname
        ) {
            // Figure out element to scroll to
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            // Does a scroll target exist?
            if (target.length) {
                // Only prevent default if animation is actually gonna happen
                event.preventDefault();
                $('html, body').animate({
                    scrollTop: target.offset().top
                }, 400, function () {
                    // Callback after animation
                    // Must change focus!
                    var $target = $(target);
                    $target.focus();
                    if ($target.is(":focus")) { // Checking if the target was focused
                        return false;
                    } else {
                        $target.attr('tabindex', '-1'); // Adding tabindex for elements not focusable
                        $target.focus(); // Set focus again
                    };
                });
            }
        }
    });



$('.faq-clicker').click(function () {

    var id = $(this).attr("faq");

    $('.faq').fadeOut(100);

    setTimeout(function () {
        $(`#${id}`).fadeIn(200);

    }, 110);


});

//SIGN/LOG

//$('#launch-sign').click(function () {

//    $('#login').fadeOut(200);
//    $('#cta-login').fadeIn(200);

//    $('#sign').fadeIn(200);
//    $('#cta-signin').fadeOut(200);

//    $('#column-login').css('background', 'transparent');
//    $('#column-signin').css('background', 'white');


//});

//$('#launch-login').click(function () {

//    $('#login').fadeIn(200);
//    $('#cta-login').fadeOut(200);

//    $('#sign').fadeOut(200);
//    $('#cta-signin').fadeIn(200);

//    $('#column-signin').css('background', 'transparent');
//    $('#column-login').css('background', 'white');
//});

//contracter 

$('#mission-launcher').click(function () {

    $('#contract-1').fadeOut(200);
    $('#menu-entreprise').fadeOut(200);
    $(".entypo-comment").html('');
    $('.inscription-process').css('padding', '14px 14px');
    $('.toolbar').css('bottom', '0');
    $('#progression-bar').css('display', 'inline-block');


    setTimeout(function () { $('#contract-2').fadeIn(200) }, 200);

});

$(document).ready(function () {
    $('#back-top').on('click', function () {
        $('html, body').animate({ scrollTop: 0 }, 'slow');
        $('#back-top').removeClass('active');
    });
});

/* Every time the window is scrolled ... */
$(window).on('scroll', function () {
    backTopAndMailTo();
    /* Check the location of each desired element */
    $('.hideme').each(function (i) {

        var bottom_of_object = $(this).offset().top + $(this).outerHeight();


        /* If the object is completely visible in the window, fade it it */
        if (bottom_of_window > bottom_of_object) {

            $(this).css({
                "opacity": "1", "transform": "translate(0,0)"
            });

        }

    });
});

$(document.body).on('touchmove', function () {
    backTopAndMailTo();
});

function backTopAndMailTo() {
    var viewTop = (document.documentElement && document.documentElement.scrollTop) || document.body.scrollTop;
    if (viewTop > 100) {
        $('#back-top').addClass('active');
        $('#mail-to').addClass('active');
    } else {
        $('#back-top').removeClass('active');
        $('#mail-to').removeClass('active');
    }
}


//FORM VERIFICATION
$(document).ready(function (e) {
    $('#prenom').keyup(function (e) {
        if ($(this).val().length > 3) { $(this).css({ "color": " rgb(73, 182, 120)", "border-color": "rgb(73, 182, 120)" }); }
    });

    $('#nom').keyup(function (e) {
        if ($(this).val().length > 3) { $(this).css({ "color": " rgb(73, 182, 120)", "border-color": "rgb(73, 182, 120)" }); }
    });

    //$('#mail-box').keyup(function (e) {
    //    if ($(this).val().length > 3) { $(this).css({ "color": " rgb(73, 182, 120)", "border-color": "rgb(73, 182, 120)" }); }
    //});

    $('#phone').keyup(function (e) {
        if ($(this).val().length > 8) { $(this).css({ "color": " rgb(73, 182, 120)", "border-color": "rgb(73, 182, 120)" }); }
    });
});

$('#hide-cookie').click(function () {
    $("#cookie-popup").fadeOut(300);
});

(function (handleTelephoneInput) {
    handleTelephoneInput(window.jQuery, window, document);
}(function ($, window, document) {

    const pattern = new RegExp('^[0-9-+ ()]*$');

    const formTelephoneInput = $('.form-telephone-input');
    const telephoneInput = $('.telephone-input');

    const formTelephoneInputConfirm = $('.form-telephone-input__confirm');
    const telephoneInputConfirmed = $('.telephone-input-confirm');

    formTelephoneInput.on("focus", ".telephone-input", function (e) {
        resetForm();
    });

    formTelephoneInput.on("keypress", ".telephone-input", function (e) {
        if (e.charCode >= 48 && e.charCode <= 57 || // numbers from 0 to 9
            e.charCode === 40 ||
            e.charCode === 41 ||
            e.charCode === 43 ||
            e.charCode === 45 ||
            e.charCode === 32) {
            // console.log('valid', this.state.value);
        } else {
            e.preventDefault();
        }
    });

    formTelephoneInput.on("click", ".telephone-input-button", function (e) {
        e.preventDefault();

        const telephoneInputValue = $(telephoneInput).val().trim();

        const isConfirmedTelephoneInput = telephoneInputConfirmed.is(':checked');

        if (telephoneInputValue === "") {
            $(telephoneInput).val('');
            displayErrorState("Ce champ est requis.", "input");
        } else if (!pattern.test(telephoneInputValue)) {
            displayErrorState("Cette valeur n'est pas valide. Tapez \u00E0 nouveau.", "input");
        } else if (telephoneInputValue && pattern.test(telephoneInputValue) && !isConfirmedTelephoneInput) {
            //displayErrorState("Veuillez accepter les conditions d'utilisation.","checkbox");
            $(formTelephoneInputConfirm).addClass('form-telephone-input__confirm--has-error');
        }
        else {
            submitTelephone(telephoneInputValue)
               .done(function (response) {
                    if (!response.IsSuccess) {
                        displayErrorState(response.Message, "server");
                    } else {
                        displaySuccessState();
                    }
                });
        }
    });

    formTelephoneInput.on("change", ".telephone-input-confirm", function (e) {
        resetForm();
    });

    function submitTelephone(value) {

        displayProcessingState();

        var telephoneData = {};
        telephoneData["phoneNumber"] = value;

        var domain = '';
        var url = '/comp/SendEmail';

        if (document.domain.indexOf('localhost') !== -1) {
            domain = 'https://inte.portalia.fr'; // hard code
        } else { 
            domain = window.location.origin;
        }

        var postUrl = [url.slice(0, 0), domain, url.slice(0)].join(''); // build post url

        return $.ajax({
            method: "POST",
            url: postUrl,
            data: telephoneData
        });
    }

    function resetForm() {
        if ($(formTelephoneInput).hasClass('form-telephone-input--has-error')) {
            $(formTelephoneInput).removeClass('form-telephone-input--has-error');
        }
        if ($(formTelephoneInputConfirm).hasClass('form-telephone-input__confirm--has-error')) {
            $(formTelephoneInputConfirm).removeClass('form-telephone-input__confirm--has-error');
        }

        $(formTelephoneInput).find(".button-label").replaceWith("<span class='button-label'>\u00catre rappel\u00e9</span>"); // Être rappelé
        $(formTelephoneInput).find('.error-message').css("display", "none");
    }

    function displaySuccessState() {
        $(telephoneInput).css("display", "none");
        $(formTelephoneInput).find(".telephone-input-button").addClass("btn-full-width").attr("disabled", "disabled");
        $(formTelephoneInput).addClass("disabled").removeClass('is-processing');
        $(formTelephoneInput).find(".button-label").replaceWith("<span class='button-label'><i class='fa fa-check'></i> Termin\u00e9</span>");
        $(telephoneInputConfirmed).attr("disabled", true);
    }

    function displayErrorState(errorMessage, type) {
        $(formTelephoneInput).addClass('form-telephone-input--has-error');
        if (type === "input") {
            $(formTelephoneInput).find(".button-label").replaceWith("<span class='button-label'><i class='fa fa-times'></i> Erreur!</span>");
            $(formTelephoneInput).find('.error-message-input').css("display", "block").text(errorMessage);
        } else if (type === "server") {
            $(formTelephoneInput).find('.error-message-server').css("display", "block").text(errorMessage);
        } else {
            //$(formTelephoneInput).find('.error-message-checkbox').css("display", "block").text(errorMessage);
        }       
    }

    function displayProcessingState() {
        $(formTelephoneInput).addClass('form-telephone-input--is-processing');
        $(formTelephoneInput).find('.button-label').replaceWith('<span class="button-label"><i class="fa fa-spinner fa-spin"></i> En traitement... </span>');
        $(formTelephoneInput).find('.error-message').css("display", "none");
    }
}));

$('body').on('click', function (event) {
    if ($('.header__menu-container').has(event.target).length === 0 && !$('.header__menu-container').is(event.target) &&
        $('.btn-mobile').has(event.target).length === 0 && !$('.btn-mobile').is(event.target)) {
        if ($('.header__menu-container').hasClass('in') === true) {
            $('.header__menu-container').removeClass('in');
            $('.header__menu-container').addClass('collapse');
            $('body').removeClass('bodyover');
        }
    }
});

