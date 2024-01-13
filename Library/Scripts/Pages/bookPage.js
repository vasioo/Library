var bookPage = (function () {
    function init($container) {
        let $borrowBookBtn = $container.find('.borrow-book-btn'),
         $unborrowBookBtn = $container.find('.unborrow-book-btn');

        $borrowBookBtn.click(function () {
            var book = $(this).attr('id');

            $.post('/Home/BorrowBook', { bookId:book }, function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'The book has been borrowed!',
                    showClass: {
                        popup: 'animate__animated animate__fadeInDown'
                    },
                    hideClass: {
                        popup: 'animate__animated animate__fadeOutUp'
                    }
                })
                location.reload();
            }).fail(function (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong!',
                })
                alert('AJAX request failed: ' + error);
            });
            
        });
        $unborrowBookBtn.click(function () {
            var book = $(this).attr('id');

            $.post('/Home/UnborrowBook', { bookId: book }, function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'The book has been removed from borrowed!',
                    showClass: {
                        popup: 'animate__animated animate__fadeInDown'
                    },
                    hideClass: {
                        popup: 'animate__animated animate__fadeOutUp'
                    }
                })
                location.reload();
            }).fail(function (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong!',
                })
                alert('AJAX request failed: ' + error);
            });
        });
    }
    return {
        init
    };
})();