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

            //$.post({
            //    type: 'POST',
            //    url: '/Librarian/AddABookPost ',
            //    contentType: 'application/json; charset=utf-8',
            //    data: JSON.stringify(bookData),
            //    success: function (data) {
            //        window.location.href = '/Librarian/AddABook';
            //    },
            //    error: function (error) {
            //        console.error('Error adding book:', error);
            //    }
            //});

            $.post('/Librarian/AddABookPost', {
                book:bookData
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

                    window.location.href = '/Librarian/AddABook';

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