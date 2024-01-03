var prevScrollpos = window.pageYOffset;
var navbar = document.getElementById("navbar");

window.onscroll = function () {
    var currentScrollPos = window.pageYOffset;

    if (prevScrollpos > currentScrollPos && currentScrollPos > distanceToHide) {
        navbar.classList.add("sticky");
        navbar.classList.remove("navbar-hidden");
    } else {
        navbar.classList.remove("sticky");
        navbar.classList.add("navbar-hidden");
    }

    prevScrollpos = currentScrollPos;
}


$(document).ready(function () {
    var signInContainer = $('.sign-in-container');

    $('#signIn').mouseenter(function () {
        signInContainer.css('display', 'block');
    });

    $(document).on('click', function (e) {
        if (!signInContainer.is(e.target) && signInContainer.has(e.target).length === 0) {
            // If the click is outside the signInContainer, hide it
            signInContainer.css('display', 'none');
        }
    });
});
