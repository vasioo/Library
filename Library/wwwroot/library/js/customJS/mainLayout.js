var prevScrollpos = window.pageYOffset;
var navbar = document.getElementById("navbar");

window.onscroll = function () {
    var currentScrollPos = window.pageYOffset;

    if (prevScrollpos > currentScrollPos) {
        navbar.classList.add("sticky");
        navbar.classList.remove("navbar-hidden");
    } else {
        navbar.classList.remove("sticky");
        navbar.classList.add("navbar-hidden");
    }

    prevScrollpos = currentScrollPos;
}

$('#menu-open').change(function () {
    if ($(this).prop('checked')) {
        $('main').addClass('menu-opened');
        $('body').css('overflow-y', 'hidden');
        $('main > div:first').css('pointer-events', 'none');
    } else {
        $('main').removeClass('menu-opened');
        $('body').css('overflow-y', 'auto');
        $('main > div:first').css('pointer-events', 'all');
    }
});
$('main').on('click', function (event) {
    var $menuContainer = $('.menu');
    if (!$(event.target).closest($menuContainer).length && !$(event.target).is('.menu-open-button')) {
        $('#menu-open').prop('checked', false);
        $('main').removeClass('menu-opened');
        $('main > div:first').css('pointer-events', 'all');
        $('body').css('overflow-y', 'auto');

    }
});