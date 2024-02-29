$(document).ready(function () {
    var referrerUrl = document.referrer;

    if (referrerUrl && referrerUrl.includes(window.location.origin)) {
        $('#return-url-for-mobile').attr('href', referrerUrl);
    } else {
        $('#return-url-for-mobile').attr('href', '/');
    }
});