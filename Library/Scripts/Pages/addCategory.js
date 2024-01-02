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