var userFeedback = (function () {
    function init($container) {
        $('#contact-form-submition').submit(function (e) {
            commonFuncs.startLoader();
            e.preventDefault();

            var formData = {
                Email: $('#Email').val(),
                Message: $('#Message').val()
            };

            $.ajax({
                type: 'POST',
                url: '/Home/SubmitUserFeedback',
                data: formData,
                success: function (response) {
                    if (response.status) {
                        commonFuncs.endLoader();
                        Swal.fire({
                            icon: 'success',
                            title: 'Успех',
                            text: response.message,
                            timer: 3000, 
                            timerProgressBar: true,
                            didClose: () => {
                                window.location.reload();
                            }
                        });
                    } else {
                        commonFuncs.endLoader();
                        Swal.fire({
                            icon: 'error',
                            title: 'Грешка',
                            text: response.message
                        });
                    }
                },
                error: function () {
                    commonFuncs.endLoader();
                    Swal.fire({
                        icon: 'error',
                        title: 'Грешка',
                        text: 'Възникна грешка. Свържете се с отдел ИТ.'
                    });
                }
            });
        });
    }
    return {
        init
    };
})();