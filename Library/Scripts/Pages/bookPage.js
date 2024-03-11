var bookPage = (function () {
    function init($container) {
        let $borrowBookBtn = $container.find('.borrow-book-btn'),
            $UnathBorrowBookBtn = $container.find('.unauth-borrow-book-btn'),
            $readBookBtn = $container.find('.read-book-btn');

        $borrowBookBtn.click(function () {
            var book = $(this).attr('id');

            $.post('/Home/BorrowBook', { bookId: book }, function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Заявката беше подадена!',
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
                    text: 'Възникна грешка!',
                })
                alert('AJAX request failed: ' + error);
            });

        });

        $readBookBtn.click(function () {
            var book = $(this).data('id');
            $.ajax({
                type: 'POST',
                url: '/Home/ReadBook',
                data: { isbn: book },
                dataType: 'json',
                success: function (response) {
                    if (response.status) {
                        Swal.fire({
                            title: "Невероятно!",
                            text: "Вие първи четете тази книга, за което получихте 5 точки!",
                            icon: "success",
                            showCancelButton: false,
                            confirmButtonColor: "#3085d6",
                            confirmButtonText: "Супер"
                        }).then((result) => {
                            Swal.fire({
                                icon: 'success',
                                title: "Ще бъдете прехвърлени до 5 секунди да четете.",
                                showClass: {
                                    popup: 'animate__animated animate__fadeInDown'
                                },
                                hideClass: {
                                    popup: 'animate__animated animate__fadeOutUp'
                                }
                            });
                            setTimeout(function () {
                                window.location.href = response.message;
                            }, 300);
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: response.message,
                            showClass: {
                                popup: 'animate__animated animate__fadeInDown'
                            },
                            hideClass: {
                                popup: 'animate__animated animate__fadeOutUp'
                            }
                        });
                    }
                },
                error: function (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'УПС...',
                        text: 'Възникна грешка!',
                    });
                    console.error('AJAX request failed:', error);
                }
            });
        });

        $UnathBorrowBookBtn.click(function () {
            Swal.fire({
                icon: "error",
                title: "No no!",
                text: "A non authenticated user cannot borrow a book!",
                footer: '<a href="/Identity/Account/Register">Create an account</a>'
            });
        });

        function sendRatingData(stars, bookId) {
            $.ajax({
                url: '/Home/RateBook',
                type: 'POST',
                data: {
                    stars: stars,
                    bookId: bookId
                },
                success: function (response) {
                    if (response.status) {
                        Swal.fire({
                            title: 'Успех!',
                            text: response.message,
                            icon: 'success'
                        });
                    } else {
                        Swal.fire({
                            title: 'Грешка!',
                            text: response.message,
                            icon: 'error'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    Swal.fire({
                        title: 'Грешка!',
                        text: 'Възникна грешка докато се публикуваха данните: ' + error,
                        icon: 'error'
                    });
                }
            });
        }

        $('.rate input[type="radio"]').change(function () {
            var stars = $(this).val();
            var bookId = $(this).closest('.rate').data('book-id');

            sendRatingData(stars, bookId);
        });
    }
    return {
        init
    };
})();