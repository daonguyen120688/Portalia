/* This js is used for these views: 

- ChangePasswordForNewPolicy.cshtml
- ChangePassword.cshtml
- ResetPassword.cshtml

*/

var changePasswordForNewPolicy = (function () {

    var initializeEvents = function() {
        function isPasswordValid() {
            var password = $("#NewPassword").val();
            var hasContainedLowerCasePattern = /[a-z]/;
            var hasContainedUpperCasePattern = /[A-Z]/;
            var hasContainedSpecialCharacterPattern = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;
            var hasContainedNumberPattern = /[0-9]/;
            var containedMore8Chars = password.length >= 8;
            var containedNumber = hasContainedNumberPattern.test(password);
            var containedLowerCases = hasContainedLowerCasePattern.test(password);
            var containedUpperCases = hasContainedUpperCasePattern.test(password);
            var containedSpecialCharacters = hasContainedSpecialCharacterPattern.test(password);
            var $validate8Chars = $("#password-validate-8-char");
            var $validateChars = $("#password-validate-char");
            var $validateNumber = $("#password-validate-number");
            var $validateSpecialChars = $("#password-validate-special-char");

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
                $("#NewPassword").addClass("input-validation-error");
            } else {
                $("#NewPassword").removeClass("input-validation-error");
            }

            return isValid;
        };

        $("#submit-change-password-form").on("click", function() {
            var $changePasswordForm = $("#change-password-form");

            if ($changePasswordForm.valid() === false) {
                return;
            }

            if (isPasswordValid() === false) {
                return;
            }

            $changePasswordForm.submit();
        });

        $("#NewPassword").keyup(function() {
            var password = $("#NewPassword").val();
            var hasContainedLowerCasePattern = /[a-z]/;
            var hasContainedUpperCasePattern = /[A-Z]/;
            var hasContainedSpecialCharacterPattern = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;
            var hasContainedNumberPattern = /[0-9]/;
            var containedMore8Chars = password.length >= 8;
            var containedNumber = hasContainedNumberPattern.test(password);
            var containedLowerCases = hasContainedLowerCasePattern.test(password);
            var containedUpperCases = hasContainedUpperCasePattern.test(password);
            var containedSpecialCharacters = hasContainedSpecialCharacterPattern.test(password);
            var $validate8Chars = $("#password-validate-8-char");
            var $validateChars = $("#password-validate-char");
            var $validateNumber = $("#password-validate-number");
            var $validateSpecialChars = $("#password-validate-special-char");
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

        $("#NewPassword").focus(function() {
            var $validate8Chars = $("#password-validate-8-char");
            var $validateChars = $("#password-validate-char");
            var $validateNumber = $("#password-validate-number");
            var $validateSpecialChars = $("#password-validate-special-char");

            $validate8Chars.removeClass("highlight-error");
            $validateChars.removeClass("highlight-error");
            $validateNumber.removeClass("highlight-error");
            $validateSpecialChars.removeClass("highlight-error");
        });
    };

    return {
        initializeEvents: initializeEvents
    };
})();