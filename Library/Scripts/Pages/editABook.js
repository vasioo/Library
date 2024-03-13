var editABook = (function () {
    function init($container) {
        $('#editBookButton').click(function () {
            commonFuncs.startLoader();
            var bookData = {
                Id: $('#editBookForm').data('id'),
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre: $('#Genre').val() ,
                Description: $('#Description').val(),
                AvailableItems: $('#AvailableItems').val(),
                NeededMembership: $('#NeededMembership').val(),
                AmountOfBooks: $('#AmountOfBooks').val()
            };


            const image = $('.uploaded-image').attr('src');

            $.post('/Librarian/EditABookPost', {
                imageObj: image,
                book: bookData
            },
                function (response) {
                    commonFuncs.endLoader();
                    Swal.fire({
                        icon: 'info',
                        title: 'Отговор на Сървъра',
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
                        title: 'Грешка във валидацията на снимката',
                        text: 'Детайли: ' + imageData
                    });
                }
            });
        });

        $('.remove-book').click(function () {
            commonFuncs.startLoader();
            var book = $(this).attr('id');

            Swal.fire({
                title: 'ВНИМАНИЕ?',
                text: "Сигурни ли сте че искате да изтриете тази книга ?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Да, Изтрий!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.post('/Librarian/RemoveABook', { bookId: book }, function (response) {
                        commonFuncs.endLoader();
                        location.reload();
                    });
                    location.reload();
                }
            })
        });
    }
    return {
        init
    };
})();