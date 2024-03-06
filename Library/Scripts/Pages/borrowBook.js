var borrowBook = (function () {
    function init($container) {
        //let $unborrowBookBtn = $container.find('.unborrow-book-btn');

        //$unborrowBookBtn.click(function () {
        //    var book = $(this).attr('id');
        //    $.ajax({
        //        type: 'POST',
        //        url: '/Home/UnborrowBook',
        //        data: { bookId: book },
        //        dataType: 'json',
        //        success: function (response) {
        //            if (response.status === true) {
        //                // Handle success
        //                Swal.fire({
        //                    icon: 'success',
        //                    title: response.message,
        //                    showClass: {
        //                        popup: 'animate__animated animate__fadeInDown'
        //                    },
        //                    hideClass: {
        //                        popup: 'animate__animated animate__fadeOutUp'
        //                    }
        //                });
        //                setTimeout(function () {
        //                    location.reload()
        //                }, 1000);
        //            } else {
        //                // Handle failure
        //                Swal.fire({
        //                    icon: 'error',
        //                    title: response.message,
        //                    showClass: {
        //                        popup: 'animate__animated animate__fadeInDown'
        //                    },
        //                    hideClass: {
        //                        popup: 'animate__animated animate__fadeOutUp'
        //                    }
        //                });
        //            }
        //        },
        //        error: function (error) {
        //            // Handle AJAX request failure
        //            Swal.fire({
        //                icon: 'error',
        //                title: 'Oops...',
        //                text: 'Something went wrong!',
        //            });
        //            console.error('AJAX request failed:', error);
        //        }
        //    });
        //});

        $(document).on('click', '.read-book-btn', function () {

        });
    }
    return {
        init
    };
})();