var addDocumentPage = (function () {
    function init($container) {
        $(document).on('click', '#submit-btn',function () {

            var formDataObject = {
                Title: $('#title').val(),
                Content: $('#tiny').val(),
            };
            const image = $('.uploaded-image').attr('src');
            $.post('/Librarian/AddDocument', {
                doc: formDataObject,
                docImage: image
            }, function (response) {
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