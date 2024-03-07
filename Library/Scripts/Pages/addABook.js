var addABook = (function () {
    function init($container) {

        $('#addBookButton').click(function () {
            var bookData = {
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre:  $('#Genre').val(),
                Description: $('#Description').val(),
                AvailableItems: $('#AvailableItems').val(),
                NeededMembership: $('#NeededMembership').val(),
                AmountOfBooks: $('#AmountOfBooks').val()
            };


            const image = $('.uploaded-image').attr('src');

            $.post('/Librarian/AddABookPost', {
                imageObj: image,
                book: bookData
            },
                function (response) {
                    Swal.fire({
                        icon: 'info',
                        title: 'Server response',
                        html: `${response.message}`,
                        showClass: {
                            popup: 'animate__animated animate__fadeInDown'
                        },
                        hideClass: {
                            popup: 'animate__animated animate__fadeOutUp'
                        }
                    });

                    location.reload();
                }).fail(function (error) {
                    console.log('AJAX request failed:', error);
                });
        });

        $('.image-upload').change(function (event) {
            const $input = $(this),
                $uploadedImage = $input.parent().find('.uploaded-image'),
                file = event.target.files[0];
            commonFuncs.validateAndResizeImage(file, function (isValid, imageData) {
                if (isValid) {
                    $uploadedImage.attr('src', imageData);
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Image Validation Failed',
                        text: 'Error details: ' + imageData
                    });
                }
            });
        });

    }
    return {
        init
    };
})();