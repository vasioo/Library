var addABook = (function () {
    function init($container) {
        $('#addBookButton').click(function () {
            var bookData = {
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre: { CategoryName: $('#Genre_CategoryName').val() },
                Description: $('#Description').val(),
                AvailableItems: $('#AvailableItems').val(),
                NeededMembership: $('#NeededMembership').val()
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
var addACategory = (function () {
    function init($container) {
        $('#addCategoryButton').click(function () {
            var neededName = $('#CategoryName').val();

            $.post('/Librarian/AddABookCategory', {
                categoryName:neededName
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
    }
    return {
        init
    };
})();
var clientManagement = (function () {
    function init($container) {
    }
    return {
        init
    };
})();
var editABook = (function () {
    function init($container) {
        $('#editBookButton').click(function () {
            var bookData = {
                Id: $('#editBookForm').attr('class'),
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre: { CategoryName: $('#Genre_CategoryName').val() },
                Description: $('#Description').val(),
                AvailableItems: $('#AvailableItems').val(),
                NeededMembership: $('#NeededMembership').val()
            };


            const image = $('.uploaded-image').attr('src');

            $.post('/Librarian/EditABookPost', {
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

        $('.remove-book').click(function () {

            var book = $(this).attr('id');

            Swal.fire({
                title: 'WARNING?',
                text: "Are you sure you want to remove the book ?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Remove!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.post('/Librarian/RemoveABook', { bookId:book }, function (response) {

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
var editStaffInformation = (function () {
    function init($container) {
    }
    return {
        init
    };
})();