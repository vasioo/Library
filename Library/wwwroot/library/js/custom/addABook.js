var addABook = (function () {
    function init($container) {

        let addBookButton = $container.find('#addBookButton');

        addBookButton.click(function () {
            var bookData = {
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre: { Name: $('#Genre_Name').val() },
                Description: $('#Description').val(),
                AvailableItems: $('#AvailableItems').val(),
                NeededMembership: $('#NeededMembership').val()
            };

            $.ajax({
                type: 'POST',
                url: '/Librarian/AddABookPost ',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(bookData),
                success: function (data) {
                    window.location.href = '/Librarian/AddABook';
                },
                error: function (error) {
                    console.error('Error adding book:', error);
                }
            });
        });

    }
    return {
        init
    };
})();