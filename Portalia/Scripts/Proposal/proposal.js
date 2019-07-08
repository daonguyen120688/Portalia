(function (handleDocumentTab) {
    handleDocumentTab(window.jQuery, window, document);
}(function ($, window, document) {
    let selectedIndex = 0;

    /**
     * init mobile menu
     * 
     * */
    function initMobileMenu() {

        initMobileMenuItemList();

        var selectedMenuItem = $('.sub-menu__mobile__selected');
        var menuList = $('.sub-menu__mobile__menu');

        var selectedMenuItemWidth = selectedMenuItem.width() + 40;
        menuList.css({ 'width': selectedMenuItemWidth + 'px' });
    }

    /**
     * init mobile menu item - dropdown list - mobile screen
     * get from the mobile menu item in large screen
     * */
    function initMobileMenuItemList() {

        //TODO:
        //var mobileMenuItem = $('.sub-menu__mobile__menu > .sub-menu__mobile__menu__item');

        //console.log(mobileMenuItem);

        //var menuMobile = $(".sub-menu__mobile__menu");

        //var displayName = document.querySelector('.sub-menu__mobile__selected-name');

        //if (mobileMenuItem.length === 0) {
        //    $(".sub-menu > .sub-menu__item").each(function (index) {
        //        var el = $(this);

        //        var isMenuActive = el.hasClass('sub-menu__item--active');

        //        var isMenuInactive = el.hasClass('sub-menu__item--inactive');

        //        var link = el.children();

        //        // create new a
        //        var a = document.createElement('a');
        //        a.className = 'sub-menu__mobile__menu__item-link proposalDocument';
        //        a.dataset.index = index;
        //        a.dataset.url = link[0].dataset.url;
        //        a.innerText = link[0].textContent;
        //        a.onclick = handleGetProposalMobile;

        //        if (isMenuActive) {
        //            displayName.textContent = link[0].textContent;
        //        }

        //        // create new li
        //        var li = document.createElement('li');

        //        li.className = 'sub-menu__mobile__menu__item';

        //        if (isMenuActive && !isMenuInactive) {
        //            li.className += ' sub-menu__mobile__menu__item--selected';
        //        }

        //        if (isMenuInactive) {
        //            li.className += ' sub-menu__mobile__menu__item--inactive';
        //        }

        //        li.dataset.index = parseInt(el[0].dataset.index, 10);

        //        li.appendChild(a);

        //        // append
        //        menuMobile.append(li);
        //    });
        //} else {

        //    removeSelectedClassForMenuMobile();

        //    // find index of selected menu - large
        //    // add selected class for menu mobile item
        //    $('.sub-menu__mobile__menu > .sub-menu__mobile__menu__item').eq(selectedIndex).addClass('sub-menu__mobile__menu__item--selected');

        //    // set the name of selected menu
        //    var displayText = $('.sub-menu > .sub-menu__item').eq(selectedIndex).children().text();
        //    displayName.textContent = displayText;


        //}
    }

    // inital Menu - Large Screen
    function initMenu() {

        //removeSelectedClassForMenu();

        //$('.sub-menu > .sub-menu__item').eq(selectedIndex).addClass('sub-menu__item--active', 'active');
       
    }

    // get new data - mobile screen - dropdown list
    function handleGetProposalMobile(e) {
        e.preventDefault();

        removeSelectedClassForMenuMobile();

        var el = $(this);

        var parent = el.parent()[0];
        parent.classList.add('sub-menu__mobile__menu__item--selected');

        selectedIndex = parseInt(parent.dataset.index, 10);

        var displayName = $('.sub-menu__mobile__selected-name');

        displayName[0].innerText = el[0].innerText;

        var url = el[0].dataset.url;

        getProposal(url);
    }

    function getProposal(url) {
        $.ajax({
            type: 'GET',
            url: url,
            success: function (htmlContent) {
                $('#space-content').html(htmlContent);
            }
        });
    }

    /*
     * remove selected class - menu mobile item dropdown list
     */
    function removeSelectedClassForMenuMobile() {
       
        $('.sub-menu__mobile__menu > .sub-menu__mobile__menu__item').each(function (item) {
            var self = $(this);
            if (self.hasClass('sub-menu__mobile__menu__item--selected')) {
                self.removeClass("sub-menu__mobile__menu__item--selected");
            }
        });
    }

    /*
    * remove selected class - menu item dropdown list
    */
    function removeSelectedClassForMenu() {
        $('.sub-menu > .sub-menu__item').each(function (item) {
            var self = $(this);
            if (self.hasClass('sub-menu__item--active', 'active')) {
                self.removeClass('sub-menu__item--active', 'active');
            }
        });
    }

    $('#sub-menu > li').hover(function () {   
        var pos = $(this).position();
        var wid = $(this).width() + 50;
        $('.sub-menu--indicator').css({
            'width': wid + 'px',
            'left': pos.left + 'px'
        });
    }, function () {
        defaultMenuIndic(false);
        return;
    });

    $('#sub-menu > li').click(function () {
        $('.sub-menu__item--active').not(this).removeClass('sub-menu__item--active');
        $('.active').not(this).removeClass('active');
        $(this).addClass('sub-menu__item--active').addClass('active');
    });


    $('.proposalDocument').click(function (e) {

        e.stopPropagation();

        removeSelectedClassForMenu();

        // add sub-menu__item--active for li
        var el = $(this);

        var parent = el.parent()[0];

        // set selected index
        selectedIndex = parseInt(parent.dataset.index, 10);

        if (!parent.classList.contains('sub-menu__item--active') && !parent.classList.contains('active')) {
            parent.classList.add('sub-menu__item--active', 'active');
        }

        var url = el.data('url');

        getProposal(url);
    });

    $('.proposalDocument.active').click();

    $('.sub-menu__item').click(function (e) {
        var el = $(this);
        el[0].children[0].click();

    });

    $('.sendProposal').click(function () {
        var url = $(this).data('url');
        $.ajax({
            type: 'GET',
            url: url,
            success: function (htmlContent) {
                $(".modal-dialog").attr("style", "width: 500px;");
                $('#myModalContent').html(htmlContent);
                $('#myModal').modal();
                $('#myModal').modal('show');
            }
        });
    });

    $('.delete-proposal').click(function () {
        var url = $(this).data("url");
        $.get(url, function (data) {
            $('#myModalContent').html(data);
            $('#myModal').modal();
            $('#myModal').modal('show');
        });
    });

    /**
     * handle responsive
     * */
    function handleRepsonsive() {
        var width = $(window).width();

        if (width <= 992) {
            initMobileMenu();
        } else {
            initMenu();
        }
    }


    function defaultMenuIndic() {
        var pos = $('.sub-menu__item--active').position();
        var wid = $('.sub-menu__item--active').width() + 50;
        $('.sub-menu--indicator').css({
            'width': wid + 'px',
            'left': pos.left + 'px'
        });
    }

    window.addEventListener("resize", function () {
        handleRepsonsive();
        defaultMenuIndic();
    });

    document.addEventListener("DOMContentLoaded", function (event) {
        handleRepsonsive();
        defaultMenuIndic();

        setTimeout(function () {
            defaultMenuIndic();
        }, 500);
    });
}));
