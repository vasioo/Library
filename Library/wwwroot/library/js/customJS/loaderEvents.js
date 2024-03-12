function startLoader() {
    $('.book_custom_loader').show();

    $('<div class="overlay"></div>').appendTo('body').css({
        'position': 'fixed',
        'top': 0,
        'left': 0,
        'width': '100%',
        'height': '100%',
        'background': 'rgba(0, 0, 0, 0.7)',
        'z-index': 9997 
    });
}

function endLoader() {
    $('.book_custom_loader').hide();

    $('.overlay').remove();
}
document.addEventListener('DOMContentLoaded', function () {
    startLoader();
});

document.onreadystatechange = function () {
    if (document.readyState === "complete") {
        endLoader();
    }
};
