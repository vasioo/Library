var editDocumentPage = (function () {
    function init($container) {
        $(document).on('click', '#submit-edit-btn', function () {
            var contentData = tinyMCE.activeEditor.getContent()
            var formDataObject = {
                Title: $('#title').val(),
                Content: contentData,
                DateOfCreation: $('#DateOfCreation').val(),
                Id:$('#Id').val(),
            };
            const image = $('.uploaded-image').attr('src');
            $.post('/Librarian/EditDocumentPost', {
                doc: formDataObject,
                docImage: image
            }, function (response) {
                if (response.status) {
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
                console.log('AJAX request failed:', error);
            });
        });
        $(document).on('click', '.deleteBlogPostButton', function () {
            var Id = $('#Id').val();

            Swal.fire({
                title: 'Сигурни ли сте?',
                text: 'Документът ще бъде изтрит перманентно!',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Да, изтрий!',
                cancelButtonText: 'Отказ'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.post('/Librarian/DeleteDocumentPost', {
                        id: Id,
                    }, function (response) {
                        if (response.status) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Успех',
                                text: 'Успешно изтрит документ.',
                                showConfirmButton: false,
                                allowOutsideClick: false,
                                timerProgressBar: true,
                                timer: 3000
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Грешка',
                                text: 'Възникна грешка при изтриването.',
                                showConfirmButton: false,
                                allowOutsideClick: false,
                                timerProgressBar: true,
                                timer: 3000
                            });
                        }

                        var currentURL = window.location.href;

                        var baseURL = currentURL.split('/')[0] + '//' + currentURL.split('/')[2];

                        window.location.href = baseURL + '/Home/Search?searchCategory=Authors';
                    }).fail(function (error) {
                        console.log('AJAX request failed:', error);
                    });
                }
            });

        });
    }
    return {
        init
    };
})();