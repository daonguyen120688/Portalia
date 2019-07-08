var register = (function () {

    var initializeEvents = function () {
        var timeoutFunc = null;

        function bindFullNameToConvention() {
            var firstName = $("#FirstName").val(),
                lastName = $("#LastName").val(),
                fullName = "";

            if (firstName !== undefined &&
                firstName !== null &&
                firstName.length > 0) {
                fullName += firstName.trim();
            }

            if (lastName !== undefined &&
                lastName !== null &&
                lastName.length > 0) {

                if (fullName.length > 0) {
                    fullName += " ";
                }

                fullName += lastName.trim();
            }

            if (fullName.length === 0) {
                fullName = "<i>" + "&lt;Entrez votre nom&gt;" + "</i>";
            }

            $("#replace-name").html("<strong>" + fullName + "</strong>");
        };

        function isUserNameValid() {
            return new Promise(function (resolve, reject) {

                var url = $("#is-user-name-valid-url").val(),
                    username = $("#Email").val();

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { username: username },
                    success: function (response) {
                        resolve(response);
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });
        };

        function isPasswordValid() {
            var $password = $("#Password");
            var password = $password.val();
            var hasContainedLowerCasePattern = /[a-z]/;
            var hasContainedUpperCasePattern = /[A-Z]/;
            var hasContainedSpecialCharacterPattern = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;
            var hasContainedNumberPattern = /[0-9]/;
            var containedMore8Chars = password.length >= 8;
            var containedNumber = hasContainedNumberPattern.test(password);
            var containedLowerCases = hasContainedLowerCasePattern.test(password);
            var containedUpperCases = hasContainedUpperCasePattern.test(password);
            var containedSpecialCharacters = hasContainedSpecialCharacterPattern.test(password);
            var $validate8Chars = $(".password-validate-8-char");
            var $validateChars = $(".password-validate-char");
            var $validateNumber = $(".password-validate-number");
            var $validateSpecialChars = $(".password-validate-special-char");

            if (containedMore8Chars === false) {
                $validate8Chars.addClass("error");
                $validate8Chars.addClass("highlight-error");
            } else {
                $validate8Chars.removeClass("error");
                $validate8Chars.removeClass("highlight-error");
            }

            if (containedNumber === false) {
                $validateNumber.addClass("error");
                $validateNumber.addClass("highlight-error");
            } else {
                $validateNumber.removeClass("error");
                $validateNumber.removeClass("highlight-error");
            }

            if (containedLowerCases === false || containedUpperCases === false) {
                $validateChars.addClass("error");
                $validateChars.addClass("highlight-error");
            } else {
                $validateChars.removeClass("error");
                $validateChars.removeClass("highlight-error");
            }

            if (containedSpecialCharacters === false) {
                $validateSpecialChars.addClass("error");
                $validateSpecialChars.addClass("highlight-error");
            } else {
                $validateSpecialChars.removeClass("error");
                $validateSpecialChars.removeClass("highlight-error");
            }

            var isValid = containedMore8Chars === true &&
                containedNumber === true &&
                containedLowerCases === true &&
                containedUpperCases === true &&
                containedSpecialCharacters === true;

            if (isValid === false) {
                $password.addClass("input-validation-error");
            } else {
                $password.removeClass("input-validation-error");
            }

            return isValid;
        };

        function handleUISuivantBtn() {
            var $btnSuivant = $(".register__form-btn--show-convention"),
                $btnContainer = $($btnSuivant.data("btn-container")),
                $indicator = $($btnSuivant.data("indicator"));

            $btnSuivant.toggleClass("disabled");
            $btnContainer.toggleClass("hidden");
            $indicator.toggleClass("hidden");
        };

        function handleUIValidStepOneRegistration() {
            // Hide form
            $('.register__form').addClass('register__form--fadeout');
            timeoutFunc = setTimeout(function () {
                $('.register__form').addClass('register__form--hidden');
                timeoutFunc = null;
            }, 300);

            // Show step 2 message
            $('.register__step-2').removeClass('register__form-message-section--fadeout');
            timeoutFunc = setTimeout(function () {
                $('.register__step-2').removeClass('register__form-message-section--hidden');
                timeoutFunc = null;
            }, 300);

            // Show convention
            timeoutFunc = setTimeout(function () {
                $('.register__contract-wrapper').addClass('register__contract-wrapper--opened');
                timeoutFunc = null;
            }, 500);

            // Show confirm convention button
            timeoutFunc = setTimeout(function () {
                $('.register__form-sign-convention').addClass('register__form-sign-convention--show');
                timeoutFunc = null;
            }, 1000);

            // Show close button on mobile
            $('.register__contract-close').addClass('register__contract-close--showed');

            // Scroll to top
            if ($(window).width() < 768) {
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }

            $('#back-top').removeClass('active');
        };

        // Click 'Suivant' button
        $('.register__form-btn--show-convention').on('click', function () {
            // Clear form message
            $(".register__form-message").find("ul").empty();

            var $btnSuivant = $(this);
            var $form = $(".register__form");

            if ($btnSuivant.hasClass("disabled")) {
                return;
            }

            if ($form.valid() === false) {
                return;
            }

            if (isPasswordValid() === false) {
                showValidationMessage($('.register__form-control--password'));
                return;
            }

            // Check if user accept legal term
            var $checkboxLegalTerm = $(".register__form-control--checkbox");
            if (!$checkboxLegalTerm[0].checked) {
                $(".register__form-checkbox").addClass("register__form-checkbox--has-error");
                return;
            } else {
                $(".register__form-checkbox").removeClass("register__form-checkbox--has-error");
            }

            // Hide password validation message
            hideValidationMessage();

            // Disabled btn and show indicator of Suivant button
            handleUISuivantBtn();

            isUserNameValid().then(function (response) {
                if (response.HasError) {
                    // Display errors
                    $(".register__form-message")
                        .removeClass("validation-summary-valid")
                        .addClass("validation-summary-errors");
                    $(".register__form-message").find("ul").html("<li>" + response.Message + "</li>");
                } else {
                    // Remove errors
                    $(".register__form-message")
                        .removeClass("validation-summary-errors")
                        .addClass("validation-summary-valid");

                    // Show convention panel
                    handleUIValidStepOneRegistration();
                }

                // Enabled and hide indicator of Suivant button
                handleUISuivantBtn();
            }).catch(function (err) {
                handleUISuivantBtn();
                return;
            });
        });

        // Click 'Back' button
        $('.register__form-btn--back').on('click',
            function () {
                // Hide convention
                $('.register__contract-wrapper').removeClass('register__contract-wrapper--opened');
                // Hide confirm convention button
                $('.register__form-sign-convention').removeClass('register__form-sign-convention--show');
                // Hide step 2 message
                $('.register__step-2').addClass('register__form-message-section--fadeout');
                timeoutFunc = setTimeout(function () {
                    $('.register__step-2').addClass('register__form-message-section--hidden');
                    timeoutFunc = null;
                },
                    300);
                // Hide step 3 message
                $('.register__step-3').addClass('register__form-message-section--fadeout');
                timeoutFunc = setTimeout(function () {
                    $('.register__step-3').addClass('register__form-message-section--hidden');
                    timeoutFunc = null;
                },
                    300);
                // Show form
                $('.register__form').removeClass('register__form--fadeout');
                timeoutFunc = setTimeout(function () {
                    $('.register__form').removeClass('register__form--hidden');
                    timeoutFunc = null;
                },
                    300);
            });

        // Scroll convention
        $('.register__contract-container').on('scroll', function (event) {
            let $this = $(this);
            let $conventionContent = $('.register__contract-content');
            let conventionHeight = $conventionContent.innerHeight();
            let screenHeight = $(window).innerHeight();
            let totalHeight = conventionHeight;
            totalHeight += parseInt($('.register__contract-container').css('padding-top'));
            totalHeight += parseInt($('.register__contract-container').css('padding-bottom'));
            totalHeight += $('.register__contract-title').innerHeight();
            totalHeight += parseInt($('.register__contract-title').css('margin-top'));
            totalHeight += parseInt($('.register__contract-title').css('margin-bottom'));
            if ($this.scrollTop() + screenHeight >= totalHeight * 0.8) {
                $('.register__form-btn--confirm').removeClass('register__form-btn--disabled');
            }
        });

        // Click confirm convention button
        $('.register__form-btn--confirm').on('click', function () {
            // Hide convention
            $('.register__contract-wrapper').removeClass('register__contract-wrapper--opened');
            // Hide close button
            $('.register__contract-close').removeClass('register__contract-close--showed');
            // Hide step 2 message
            $('.register__step-2').addClass('register__form-message-section--fadeout');
            timeoutFunc = setTimeout(function () {
                $('.register__step-2').addClass('register__form-message-section--hidden');
                timeoutFunc = null;
            }, 300);
            // Show step 3 message
            $('.register__step-3').removeClass('register__form-message-section--fadeout');
            timeoutFunc = setTimeout(function () {
                $('.register__step-3').removeClass('register__form-message-section--hidden');
                timeoutFunc = null;
            }, 300);
            // Change backdrop image
            $('.register__contract-backdrop-img').attr('src', 'https://cdn.o2f-it.com/download/21215/bg-register-portalia-alt.jpg');
        });

        // Click link open convention
        $('.register__form-convention-btn').on('click', function () {
            // Show convention
            $('.register__contract-wrapper').addClass('register__contract-wrapper--opened');

            // Show confirm convention button
            $('.register__form-sign-convention').addClass('register__form-sign-convention--show');

            // Show close button on mobile
            $('.register__contract-close').addClass('register__contract-close--showed');

            // Scroll to top
            if ($(window).width() < 768) {
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }

            $('#back-top').removeClass('active');
        })

        // Click close convention button
        $('.register__contract-close').on('click', function () {
            // Hide convention
            $('.register__contract-wrapper').removeClass('register__contract-wrapper--opened');
            // Hide close button
            $('.register__contract-close').removeClass('register__contract-close--showed');
        });

        // Click finish button
        $('.register__form-btn--finish').on('click', function () {
            // Trigger submit button
            $('.register__form-btn--submit').trigger('click');
        });

        $("#FirstName").on("keyup", function (e) {
            bindFullNameToConvention();
        });

        $("#LastName").on("keyup", function (e) {
            bindFullNameToConvention();
        });

        // Prevent Hitting Enter to submit form
        $(window).keydown(function (event) {
            if (event.keyCode === 13) {
                event.preventDefault();
                return false;
            }
        });

        // Show password validation message
        function showValidationMessage(passwordElement) {
            if ($(window).innerWidth() > 1024) {
                var passwordInputPosition = passwordElement.position();
                $('.password-valid-message').css({
                    'opacity': 1,
                    'top': passwordInputPosition.top,
                    'left': passwordInputPosition.left + passwordElement.outerWidth() + 20
                })
            } else {
                $('.password-valid-message').css({
                    'display': 'block',
                    'opacity': 1
                })
            }
        }

        // Hide password validation message

        function hideValidationMessage() {
            if ($(window).innerWidth() > 1024) {
                $('.password-valid-message').css({
                    'opacity': 0,
                    'top': 'auto',
                    'left': 'auto'
                })
            } else {
                $('.password-valid-message').css({
                    'display': 'none',
                    'opacity': 0
                })
            }
        }

        $('.register__form-control--password').on('click focus', function (event) {
            showValidationMessage($(this));

            var $validate8Chars = $(".password-validate-8-char");
            var $validateChars = $(".password-validate-char");
            var $validateNumber = $(".password-validate-number");
            var $validateSpecialChars = $(".password-validate-special-char");

            $validate8Chars.removeClass("highlight-error");
            $validateChars.removeClass("highlight-error");
            $validateNumber.removeClass("highlight-error");
            $validateSpecialChars.removeClass("highlight-error");
        });

        $('.register__form-control--password').on('focusout', function (event) {
            hideValidationMessage();
        });

        $('.register__form-control--password').on('keyup',
            function () {
                var password = $("#Password").val();
                var hasContainedLowerCasePattern = /[a-z]/;
                var hasContainedUpperCasePattern = /[A-Z]/;
                var hasContainedSpecialCharacterPattern = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;
                var hasContainedNumberPattern = /[0-9]/;
                var containedMore8Chars = password.length >= 8;
                var containedNumber = hasContainedNumberPattern.test(password);
                var containedLowerCases = hasContainedLowerCasePattern.test(password);
                var containedUpperCases = hasContainedUpperCasePattern.test(password);
                var containedSpecialCharacters = hasContainedSpecialCharacterPattern.test(password);
                var $validate8Chars = $(".password-validate-8-char");
                var $validateChars = $(".password-validate-char");
                var $validateNumber = $(".password-validate-number");
                var $validateSpecialChars = $(".password-validate-special-char");
                var svgAppearClass = $validate8Chars.find(".fa-check").attr("class");
                var svgHiddenClass = svgAppearClass.replace("hidden", "");

                if (containedMore8Chars === false) {
                    $validate8Chars.addClass("error");
                    $validate8Chars.find(".fa-check").attr("class", svgHiddenClass);
                } else {
                    $validate8Chars.removeClass("error");
                    $validate8Chars.find(".fa-check").attr("class", svgAppearClass);
                }

                if (containedNumber === false) {
                    $validateNumber.addClass("error");
                    $validateNumber.find(".fa-check").attr("class", svgHiddenClass);
                } else {
                    $validateNumber.removeClass("error");
                    $validateNumber.find(".fa-check").attr("class", svgAppearClass);
                }

                if (containedLowerCases === false || containedUpperCases === false) {
                    $validateChars.addClass("error");
                    $validateChars.find(".fa-check").attr("class", svgHiddenClass);
                } else {
                    $validateChars.removeClass("error");
                    $validateChars.find(".fa-check").attr("class", svgAppearClass);
                }

                if (containedSpecialCharacters === false) {
                    $validateSpecialChars.addClass("error");
                    $validateSpecialChars.find(".fa-check").attr("class", svgHiddenClass);
                } else {
                    $validateSpecialChars.removeClass("error");
                    $validateSpecialChars.find(".fa-check").attr("class", svgAppearClass);
                }
            });


        $('body').on('click',
            function (event) {
                if ($('.register__form-control--password').has(event.target).length === 0 &&
                    !$('.register__form-control--password').is(event.target) &&
                    $('.password-valid-message').has(event.target).length === 0 &&
                    !$('.password-valid-message').is(event.target) &&
                    $('.register__form-btn--show-convention').has(event.target).length === 0 &&
                    !$('.register__form-btn--show-convention').is(event.target)) {
                    hideValidationMessage();
                }
            });
    };

    var initializeCaptchaFor15Minutes = function () {
        setInterval(function () { grecaptcha.reset(); }, 5 * 60 * 1000);
    };

    return {
        initializeEvents: initializeEvents,
        initializeCaptchaFor15Minutes: initializeCaptchaFor15Minutes
    };
})();