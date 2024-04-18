var editABook = (function () {
    function init($container) {
        function isValidISBN10(isbn) {
            isbn = isbn.replace(/[^\dX]/gi, '');
            if (isbn.length !== 10) return false;

            var sum = 0;
            for (var i = 0; i < 9; i++) {
                sum += parseInt(isbn[i]) * (10 - i);
            }

            var checksum = isbn[9].toUpperCase() === 'X' ? 10 : parseInt(isbn[9]);
            sum += checksum;

            return sum % 11 === 0;
        }

        function isValidISBN13(isbn) {
            isbn = isbn.replace(/[^\d]/gi, '');
            if (isbn.length !== 13) return false;

            var sum = 0;
            for (var i = 0; i < 12; i++) {
                sum += parseInt(isbn[i]) * (i % 2 === 0 ? 1 : 3);
            }

            var checksum = parseInt(isbn[12]);
            var remainder = (10 - (sum % 10)) % 10;

            return checksum === remainder;
        }

        $('#editBookButton').click(function () {
            commonFuncs.startLoader();
            var bookData = {
                Id: $('#editBookForm').data('id'),
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre: $('#Genre').val(),
                Description: $('#Description').val(),
                NeededMembership: $('#NeededMembership').val(),
                AmountOfBooks: $('#AmountOfBooks').val(),
                ISBN: $('#ISBN').val(),
                Language: $('#Language').val(),
                PreviewLink: $('#PreviewLink').val()
            };
            var errors = [];


            if (!bookData.Id) errors.push("Идентификационният номер е задължителен.");
            if (!bookData.Name) errors.push("Името на книгата е задължително.");
            if (!bookData.Author) errors.push("Авторът на книгата е задължителен.");
            if (!bookData.DateOfBookCreation) errors.push("Дата на създаване на книгата е задължителна.");
            if (!bookData.Genre) errors.push("Жанрът на книгата е задължителен.");
            if (!bookData.Description) errors.push("Описанието на книгата е задължително.");
            if (!bookData.NeededMembership) errors.push("Необходимият абонамент за книгата е задължителен.");
            if (!bookData.AmountOfBooks) errors.push("Броят на книгите е задължителен.");
            if (!bookData.Language) errors.push("Езикът на книгата е задължителен.");
            if (!isValidUrl(bookData.PreviewLink)) errors.push("Връзката към прегледа на книгата не е валидна.");
            if (!bookData.ISBN) {
                errors.push("ISBN номерът е задължителен.");
            } else {
                var isbn = bookData.ISBN.replace(/[^\dX]/gi, '');
                if (isbn.length !== 10 && isbn.length !== 13) {
                    errors.push("ISBN номерът трябва да бъде от 10 или 13 символа.");
                } else {
                    if (!(isValidISBN10(isbn) || isValidISBN13(isbn))) {
                        errors.push("Невалиден ISBN номер.");
                    }
                }
            }
            if (bookData.PreviewLink !== "Unavailable" && bookData.PreviewLink !== "" && !isValidUrl(bookData.PreviewLink)) {
                errors.push("Връзката към прегледа на книгата не е валидна.");
            }
            if (errors.length > 0) {
                commonFuncs.endLoader();
                var errorMessage = "<div class='error-message' style='color: red;text-align: left;'>";
                errorMessage += "<p style='color:black !important;'>Моля, коригирайте следните грешки:</p><br><br>";
                errorMessage += errors.join("<br>");
                errorMessage += "</div>";

                Swal.fire({
                    title: 'Грешка!',
                    html: errorMessage,
                    icon: 'error',
                    confirmButtonText: 'Разбрах',
                    customClass: {
                        htmlContainer: 'text-left'
                    }
                });
            } else {
                const image = $('.uploaded-image').attr('src');

                $.post('/Librarian/EditABookPost', {
                    book: bookData,
                    imageObj: image
                },
                    function (response) {
                        commonFuncs.endLoader();
                        if (response.status) {
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
                            Swal.fire({
                                icon: 'error',
                                title: 'Грешка',
                                text: response.message
                            });
                        }

                    }).fail(function (error) {
                        commonFuncs.endLoader();
                        console.log('AJAX request failed:', error);
                    });
            }
        });
        function isValidUrl(url) {
            try {
                new URL(url);
                return true;
            } catch (_) {
                return false;
            }
        }
        $('.image-upload').change(function (event) {
            const $input = $(this),
                $uploadedImage = $input.parent().find('.uploaded-image'),
                file = event.target.files[0];
            commonFuncs.validateAndResizeImage(file, function (isValid, imageData) {
                if (isValid) {
                    $uploadedImage.attr('src', imageData);
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Грешка във валидацията на снимката',
                    });
                }
            });
        });

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