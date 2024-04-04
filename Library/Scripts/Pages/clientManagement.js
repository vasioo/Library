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
                    $.ajax({
                        url: '/Admin/BanUser',
                        method: 'POST',
                        data: { email: email },
                        success: function (response) {
                        },
                        error: function (xhr, status, error) {
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