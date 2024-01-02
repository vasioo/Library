var addABook = (function () {
    function init($container) {
        $('#addBookButton').click(function () {
            var bookData = {
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre: { Name: $('#Genre_Name').val() },
                Description: $('#Description').val(),
                AvailableItems: $('#AvailableItems').val(),
                NeededMembership: $('#NeededMembership').val()
            };

            var imageData = $('.uploaded-image').attr('src');

            $.post('/Librarian/AddABookPost', {
                book: bookData,
                image: imageData
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

            $.post('/Librarian/AddABookPost', {
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
var editStaffInformation = (function () {
    function init($container) {
    }
    return {
        init
    };
})();