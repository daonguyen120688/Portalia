(function (handleWelcomeCard) {
    handleWelcomeCard(window.jQuery, window, document);
}(function ($, window, document) {

    var body = document.querySelector('body');
    var welcomeCard = document.querySelector('.welcome-card');
    var firstCard = document.querySelector('.welcome-card--first');
    var firstCardAction = document.querySelector('.welcome-card__content-action--first-card');

    var secondCard = document.querySelector('.welcome-card--second');
    var secondCardAction = document.querySelector('.welcome-card__content-action--second-card');

    document.addEventListener("DOMContentLoaded", function (event) {
        getLoggedInUserInfo();
    });

    firstCardAction.addEventListener("click", function () {
        firstCard.classList.add('welcome-card--slide');
        secondCard.classList.remove('welcome-card--hide');

        setTimeout(function () {
            firstCard.classList.add('welcome-card--hide');
            secondCard.classList.add('welcome-card--show');
        }, 300);

    });

    secondCardAction.addEventListener("click", function () {
        welcomeCard.classList.add('welcome-card--closing');
      
        const url = window.location.href.indexOf('localhost') !== -1 ? '/Account/CloseWelcomeCards' : '/Portalia/Account/CloseWelcomeCards';

        $.ajax({
            url: url,
            method: 'GET',
            success: function (data) {

                setTimeout(function () {
                    welcomeCard.classList.add('welcome-card--hide');
                }, 1200);

                setTimeout(function () {
                    // delete DOM node
                    removeWelcomeCardNode();
                }, 1250);
            },
            errror: function (error) {
                console.log(error);
            }
        });


    });

    function getLoggedInUserInfo() {

        const url = window.location.href.indexOf('localhost') !== -1 ? '/Account/GetLoggedUserInfor' : '/Portalia/Account/GetLoggedUserInfor';

        $.ajax({
            url: url,
            method: 'GET',
            success: function (data) {
                if (data.CanSeeWelcomeCards) {
                    welcomeCard.classList.remove('welcome-card--hide');

                    // hide scroll bar when displaying welcome card
                    body.setAttribute("style", "overflow-y: hidden;");

                } else {
                   removeWelcomeCardNode();
                }
            },
            errror: function (error) {
                console.log(error);
            }
        });
    }

    function removeWelcomeCardNode() {

        if (body.getAttribute("style") !== null) {
            body.removeAttribute("style");
        }


        //var element = document.querySelector('section.welcome-card');
        //if (element) {
        //    element.parentNode.removeChild(element);
        //}
    }

}));