$(document).ready(function () {
    const $profileImages = $('.image-upload');

    $profileImages.change(function (event) {
        const $self = $(this),
            $uploadedImage = $self.closest('.uploaded-image'),
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
                    text: 'Грешка при валидацията на снимката...'
                })
            }
        });
    });
    $('#custom-view-profile-image-container-id').click(function () {
        var itemContainer = document.getElementById('custom-view-profile-image-container-id');
        itemContainer.style.setProperty('display', 'none', 'important');

        var neededBody = document.getElementsByTagName('body')[0];
        neededBody.style.overflow = 'hidden';
    });
    $(document).on('error', '.profile-image-class', function () {
        $(this).off('error').attr('src', '/handbook/images/anonymousUser.png');
    }).on('load', '.profile-image-class', function () { });

    $(document).ready(function () {
        $(document).find('.profile-image-class').each(function () {
            if (!this.complete || (typeof this.naturalWidth !== 'undefined' && this.naturalWidth === 0)) {
                $(this).trigger('error');
            }
        });
    });
});
