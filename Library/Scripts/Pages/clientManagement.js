var clientManagement = (function () {
    function init($container) {
        $('.btn-ban').click(function () {
            var email = $(this).data('email');
            Swal.fire({
                title: 'Сигурен ли сте, че искате да блокирате този потребител?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Да',
                cancelButtonText: 'Отказ'
            }).then((result) => {
                if (result.isConfirmed) {
                    commonFuncs.startLoader();
                    $.ajax({
                        url: '/Admin/BanUser',
                        method: 'POST',
                        data: { email: email },
                        success: function (response) {
                            if (response.status) {
                                commonFuncs.endLoader();
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Успешна промяна',
                                    text: response.message,
                                    showCancelButton: true,
                                    showConfirmButton: true,
                                }).then((result) => {
                                    if (result.isConfirmed) {
                                        location.reload();
                                    }
                                });

                            }
                            else {
                                commonFuncs.endLoader();
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Грешка',
                                    text: response.message,
                                })
                            }
                        },
                        error: function (xhr, status, error) {
                            commonFuncs.endLoader();
                            Swal.fire({
                                icon: 'error',
                                title: 'Грешка',
                                text: 'Грешка в сървъра!',
                            })
                        }
                    });
                }
            });
        });
        $('.btn-unban').click(function () {
            var email = $(this).data('email');
            Swal.fire({
                title: 'Сигурен ли сте, че искате да отблокирате този потребител?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Да',
                cancelButtonText: 'Отказ'
            }).then((result) => {
                if (result.isConfirmed) {
                    commonFuncs.startLoader();
                    $.ajax({
                        url: '/Admin/UnbanUser',
                        method: 'POST',
                        data: { email: email },
                        success: function (response) {
                            if (response.status) {
                                commonFuncs.endLoader();
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Успешна промяна',
                                    text: response.message,
                                    showCancelButton: true,
                                    showConfirmButton: true,
                                }).then((result) => {
                                    if (result.isConfirmed) {
                                        location.reload();
                                    }
                                });

                            }
                            else {
                                commonFuncs.endLoader();
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Грешка',
                                    text: response.message,
                                })
                            }
                        },
                        error: function (xhr, status, error) {
                            commonFuncs.endLoader();
                            Swal.fire({
                                icon: 'error',
                                title: 'Грешка',
                                text: 'Грешка в сървъра!',
                            })
                        }
                    });
                }
            });
        });
    }
    return {
        init
    };
})();