var bookPage = (function () {
    function init($container) {
        let $borrowBookBtn = $container.find('.borrow-book-btn'),
            $UnathBorrowBookBtn = $container.find('.unauth-borrow-book-btn'),
            $readBookBtn = $container.find('.read-book-btn');

        $borrowBookBtn.click(function () {
            commonFuncs.startLoader();
            var book = $(this).attr('id');

            $.post('/Home/BorrowBook', { bookId: book }, function (response) {
                commonFuncs.endLoader();
                if (response.status) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Успешна промяна',
                        text: response.message
                    });
                    location.reload();
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Грешка',
                        text: response.message
                    });
                }
            }).fail(function (error) {
                commonFuncs.endLoader();
                Swal.fire({
                    icon: 'error',
                    title: 'Упс...',
                    text: 'Възникна грешка!',
                })
                alert('AJAX request failed: ' + error);
            });
        });

        $readBookBtn.click(function () {
            commonFuncs.startLoader();
            var book = $(this).data('id');
            $.ajax({
                type: 'POST',
                url: '/Home/ReadBook',
                data: { isbn: book },
                dataType: 'json',
                success: function (response) {
                    if (response.status) {
                        if (typeof firstUserRead !== "undefined") {
                            commonFuncs.endLoader();
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
                        }
                        else {
                            commonFuncs.endLoader();
                            Swal.fire({
                                icon: 'success',
                                title: "Ще бъдете прехвърлени до 5 секунди да прегледате книгата.",
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
                        }
                        
                    } else {
                        commonFuncs.endLoader();
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
                    commonFuncs.endLoader();
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
                title: "Невъзможно!",
                text: "Неоторизиран потребител не може да вземе книга назаем!",
                footer: '<a href="/Identity/Account/Register">Създайте акаунт</a>'
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
                        commonFuncs.endLoader();
                        Swal.fire({
                            title: 'Успех!',
                            text: response.message,
                            icon: 'success',
                            showCancelButton: true,
                            showConfirmButton: true,
                            showCloseButton: true,
                            timer: 2000,
                            timerProgressBar: true
                        });
                        location.reload();
                    } else {
                        commonFuncs.endLoader();
                        Swal.fire({
                            title: 'Грешка!',
                            text: response.message,
                            icon: 'error'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    Swal.fire({
                        title: 'Грешка!',
                        text: 'Възникна грешка докато се публикуваха данните: ' + error,
                        icon: 'error'
                    });
                }
            });
        }

        $('.rate input[type="radio"]').change(function () {
            commonFuncs.startLoader();
            var stars = $(this).val();
            var bookId = $(this).closest('.rate').data('book-id');

            sendRatingData(stars, bookId);
            commonFuncs.endLoader();
        });
    }
    return {
        init
    };
})();