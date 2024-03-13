var addABook = (function () {
    function init($container) {

        $('#addBookButton').click(function () {
            commonFuncs.startLoader();
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
                        title: 'Отговор на сървъра',
                        html: `${response.message}`,
                        showClass: {
                            popup: 'animate__animated animate__fadeInDown'
                        },
                        hideClass: {
                            popup: 'animate__animated animate__fadeOutUp'
                        }
                    });
                    commonFuncs.endLoader();
                    location.reload();
                }).fail(function (error) {
                    commonFuncs.endLoader();
                    console.log('AJAX request failed:', error);
                });
            commonFuncs.endLoader();
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
                        title: 'Грешка с валидацията на снимката',
                        text: 'Детайли: ' + imageData
                    });
                }
            });
        });

    }
    return {
        init
    };
})();