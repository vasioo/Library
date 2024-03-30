 $(document).ready(function () {
    const $profileImages = $('.image-upload');

    $profileImages.change(function (event) {
        const $self = $(this),
            $uploadedImage = $('#main-image-for-showing'),
            $uploadedImageContainer = $('#custom-view-profile-image-id'),
            file = event.target.files[0];

        commonFuncs.validateAndResizeImage(file, function (isValid, imageData) {
            if (isValid) {
                $uploadedImage.attr('src', imageData);
                $uploadedImageContainer.attr('src', imageData);

            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Упс...',
                    text: 'Грешка при валидация на снимката:' 
                })
            }
        });
    });
});

$('#custom-view-profile-image-container-id').click(function () {
    var itemContainer = $('#custom-view-profile-image-container-id');
    itemContainer.css('display', 'none');

    var neededBody = $('body');
    neededBody.css('overflow', 'hidden');
});

function handleProfileImageError(event) {
    if ($(this).hasClass('profile-image-class')) {
        $(this).off('error').attr('src', '/images/anonymous-user.jpg');
    }
}
$(document).ready(function () {

    $('.profile-image-class').each(function () {
        if (!this.complete || (typeof this.naturalWidth !== 'undefined' && this.naturalWidth === 0)) {
            $(this).on('error', handleProfileImageError).trigger('error');
        }
    });
});
$('.profile-image-class').click(function () {
    var itemContainer = document.getElementById('custom-view-profile-image-container-id');
    itemContainer.style.setProperty('display', 'flex', 'important');

    var neededBody = document.getElementsByTagName('body')[0];
    neededBody.style.overflow = 'hidden';
});
