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

                    location.reload();
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