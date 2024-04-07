var addACategory = (function () {
    function init($container) {
        $('#addCategoryButton').click(function () {
            commonFuncs.startLoader();
            var neededName = $('#CategoryName').val();

            $.post('/Librarian/AddABookCategory', {
                categoryName:neededName
            },
                function (response) {
                    commonFuncs.endLoader();
                    if (response.status) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Успешна промяна',
                            text: response.message
                        });
                        location.reload();
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Грешка',
                            text: response.message
                        });
                    }
                }).fail(function (error) {
                    commonFuncs.endLoader();
                    console.log('AJAX request failed:', error);
                });
        });
    }
    return {
        init
    };
})();