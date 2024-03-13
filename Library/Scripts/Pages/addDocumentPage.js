var addDocumentPage = (function () {
    function init($container) {
        $(document).on('click', '#submit-btn',function () {
            commonFuncs.startLoader();
            var contentData = tinyMCE.activeEditor.getContent()

            var formDataObject = {
                Title: $('#title').val(),
                Content: contentData,
            };
            const image = $('.uploaded-image').attr('src');
            $.post('/Librarian/AddDocument', {
                doc: formDataObject,
                docImage: image
            }, function (response) {
                if (response.status) {
                    commonFuncs.endLoader();
                    Swal.fire({
                        icon: 'success',
                        title: 'Успех',
                        text: response.message,
                        showConfirmButton: false,
                        allowOutsideClick: false,
                        timerProgressBar: true,
                        timer: 3000
                    });
                } else {
                    commonFuncs.endLoader();
                    Swal.fire({
                        icon: 'error',
                        title: 'Грешка',
                        text: response.message,
                        showConfirmButton: false,
                        allowOutsideClick: false,
                        timerProgressBar: true,
                        timer: 3000
                    });
                }
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