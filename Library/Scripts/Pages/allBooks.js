var allBooks = (function () {
    function init($container) {
        $('.remove-book').click(function () {
            commonFuncs.startLoader();
            var book = $(this).attr('id');

            Swal.fire({
                title: 'ВНИМАНИЕ?',
                text: "Сигурни ли сте че искате да изтриете тази книга ?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Да, Изтрий!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.post('/Librarian/RemoveABook', { bookId: book }, function (response) {
                        commonFuncs.endLoader();
                        location.reload();
                    });
                    location.reload();
                }
            })
            commonFuncs.endLoader();
        });
    }
    return {
        init
    };
})();