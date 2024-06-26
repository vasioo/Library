var addABook = (function () {
    function init($container) {
        function isValidUrl(url) {
            try {
                new URL(url);
                return true;
            } catch (_) {
                return false;
            }
        }
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

        $('#addBookButton').click(function () {
            commonFuncs.startLoader();
            var bookData = {
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

                $.post('/Librarian/AddABookPost', {
                    imageObj: image,
                    book: bookData
                },
                    function (response) {
                        commonFuncs.endLoader();
                        if (response.status) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Успешна промяна',
                                text: response.message,
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
                        title: 'Грешка с валидацията на снимката',
                        text: 'Детайли: ' + imageData
                    });
                }
            });
        });

    }
    return {
        init
    };
})();
var addABookByISBN = (function () {
    function init($container) {

        function displayBookDetails(book) {
            var isbnData = $('#isbnInput').val();
            $('#isbnOfBook').text(`ISBN: ${isbnData}`);
            $('#isbnOfBook').data('id', isbnData);

            if (book.title) {
                $('#bookTitle').val(book.title);
            }
            if (book.publishers && book.publishers.length > 0) {
                $('#bookAuthor').val(book.publishers[0]);
            }
            if (book.publish_date) {
                var publishDate = new Date(book.publish_date);
                var year = publishDate.getFullYear();
                $('#bookCreationDate').val(year + '-01-01');
            }
            if (book.subjects && book.subjects.length > 0) {
                $('#category').text(`Техен жанр: ${book.subjects[0]}`);
            }
            if (book.languages && book.languages.length > 0) {
                var languageKey = book.languages[0].key;
                var languageCode = languageKey.split('/').pop();
                $('#language').val(languageCode);
            }
            if (book.covers && book.covers.length > 0) {
                $('#bookCover').attr('src', `https://covers.openlibrary.org/b/id/${book.covers[0]}-L.jpg`);
            }
            $('#bookDetailsContainer').show();
        }

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

        $('#submit-isbn-info').click(function () {
            commonFuncs.startLoader();
            var isbn = document.getElementById('isbnInput').value;

            if (isbn.trim() === "") {
                Swal.fire({
                    title: "Грешка",
                    text: "Моля въведете ISBN номер.",
                    icon: "error",
                });
                return;
            }

            if (!isValidISBN10(isbn) && !isValidISBN13(isbn)) {
                Swal.fire({
                    title: "Грешка",
                    text: "Моля въведете валиден ISBN номер.",
                    icon: "error",
                });
                return;
            }

            var xhr = new XMLHttpRequest();
            xhr.open("GET", "/Librarian/FindBookByISBN?isbn=" + isbn, true);
            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        if (response.status) {
                            var bookData = JSON.parse(response.message);
                            displayBookDetails(bookData);
                            commonFuncs.endLoader();
                        } else {
                            commonFuncs.endLoader();
                            Swal.fire({
                                title: "Грешка",
                                text: response.message,
                                icon: "error",
                            });
                        }
                    } else {
                        commonFuncs.endLoader();
                        Swal.fire({
                            title: "Грешка",
                            text: "Възникна грешка с извличането на информацията.",
                            icon: "error",
                        });
                    }
                }
            };
            xhr.send();

        });

        $('#bookForm input, #bookForm textarea, #bookForm select').on('input', function () {
            $(this).removeClass('error');
        });

        $('#saveFormData').click(function (e) {
            e.preventDefault();
            commonFuncs.startLoader();
            var emptyFields = [];

            $('#bookForm textarea, #bookForm select').each(function () {
                var fieldValue = $(this).val().trim();
                if (!fieldValue) {
                    emptyFields.push($(this)); 
                    $(this).addClass('error'); 
                } else {
                    $(this).removeClass('error');
                }
            });

            if (emptyFields.length > 0) {
                Swal.fire("Грешка", "Моля запълнете всички полета.", "error");
                emptyFields[0].focus();
                return;
                commonFuncs.endLoader();
            }
            var bookData = {
                ISBN: $('#isbnOfBook').data('id'),
                Title: $('#bookTitle').val(),
                Authors: $('#bookAuthor').val(),
                PublishDate: $('#bookCreationDate').val(),
                Category: $('#bookGenre').val(),
                Description: $('#bookDescription').val(),
                Language: $('#language').val(),
                AmountOfBooks: $('#bookAmount').val(),
                ImageURL: $('.uploaded-image').attr('src') 
            };
            $.ajax({
                url: $('#bookForm').attr('action'),
                method: 'POST',
                data: bookData,
                success: function (response) {
                    if (response.status) {
                        commonFuncs.endLoader();
                        Swal.fire("Успех", response.message, "success");
                    } else {
                        commonFuncs.endLoader();
                        Swal.fire("Грешка", response.message, "error");
                    }
                },
                error: function () {
                    commonFuncs.endLoader();
                    Swal.fire("Грешка", "Възникна грешка при изпращането на заявката.", "error");
                }
            });
        });

    }
    return {
        init
    };
})();
var addACategory = (function () {
    function init($container) {
        $('#addCategoryButton').click(function () {
            commonFuncs.startLoader();
            var neededName = $('#CategoryName').val();

            $.post('/Librarian/AddABookCategory', {
                categoryName:neededName
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
        });
    }
    return {
        init
    };
})();
var addDocumentPage = (function () {
    function init($container) {
        $(document).on('click', '#submit-btn',function () {
            commonFuncs.startLoader();
            var contentData = tinyMCE.activeEditor.getContent()

            var formDataObject = {
                Title: $('#title').val(),
                Content: contentData,
            };
            const image = $('.uploaded-image').attr('src');
            $.post('/Librarian/AddDocument', {
                doc: formDataObject,
                docImage: image
            }, function (response) {
                if (response.status) {
                    commonFuncs.endLoader();
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
                    commonFuncs.endLoader();
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
                commonFuncs.endLoader();
                console.log('AJAX request failed:', error);
            });
        });

    }
    return {
        init
    };
})();
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
                        title: 'Успешна заявка!',
                        text: response.message,
                        icon: 'success',
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
                            showConfirmButton: true,
                        }).then((result) => {
                            if (result.isConfirmed) {
                                location.reload();
                            }
                        });

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
                                showCancelButton: false,
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
var editDocumentPage = (function () {
    function init($container) {

        $(document).on('click', '#submit-edit-btn', function () {
            commonFuncs.startLoader();
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
                    commonFuncs.endLoader();
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
                    commonFuncs.endLoader();
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
                commonFuncs.endLoader();
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
                    commonFuncs.startLoader();
                    $.post('/Librarian/DeleteDocumentPost', {
                        id: Id,
                    }, function (response) {
                        if (response.status) {
                            commonFuncs.endLoader();
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
                            commonFuncs.endLoader();
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
                        commonFuncs.endLoader();
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
var editStaffInformation = (function () {
    function init($container) {
        $('#Position').change(function () {
            if ($(this).val() === 'Потребител') {
                $('#Salary').prop('disabled', true).val('');
            } else {
                $('#Salary').prop('disabled', false);
            }
        });
        $('#submitButton').click(function () {
            commonFuncs.startLoader();
            var formData = {
                Id: $('#Id').val(),
                Salary: $('#Salary').val(),
                Position: $('#Position').val(),
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            };

            $.ajax({
                type: "POST",
                url: '/Admin/EditInfo',
                data: formData,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("RequestVerificationToken", formData.__RequestVerificationToken);
                },
                success: function (response) {
                    if (response.status) {
                        commonFuncs.endLoader();
                        Swal.fire("Успех", response.message, "success");
                    } else {
                        commonFuncs.endLoader();
                        Swal.fire("Грешка", response.errors, "error");
                    }
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    Swal.fire("Грешка", xhr.responseText, "error");
                }
            });
        });
    }
    return {
        init
    };
})();
var leasedTracker = (function () {
    function init($container) {
        $('#deleteBtn').on('click', function (e) {
            commonFuncs.startLoader();
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/DeleteUserLeasedEntity",
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    commonFuncs.endLoader();
                    location.reload();
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    console.error(xhr.responseText);
                }
            }); 
        });

        $('#stopLeasingBtn').on('click', function (e) {
            commonFuncs.startLoader();
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/StopLeasing",
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    commonFuncs.endLoader();
                    location.reload();
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    console.error(xhr.responseText);
                }
            }); 
        });

        $('#leaseBookBtn').on('click', function (e) {
            commonFuncs.startLoader();
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/LeaseBookOrNot",
                data: { userLeasedId: userLeasedId, lease: true },
                success: function (response) {
                    if (response.status) {
                        commonFuncs.endLoader();
                        Swal.fire("Успех", response.message, "success");
                        location.reload();
                    } else {
                        commonFuncs.endLoader();
                        Swal.fire("Грешка", response.message, "error");
                    }
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    console.error(xhr.responseText);
                }
            });
        });

        $('#rejectLeaseBtn').on('click', function (e) {
            commonFuncs.startLoader();
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/LeaseBookOrNot",
                data: { userLeasedId: userLeasedId, lease: false },
                success: function (response) {
                    commonFuncs.endLoader();
                    location.reload();
                },
                error: function (xhr, status, error) {
                    commonFuncs.endLoader();
                    console.error(xhr.responseText);
                }
            });
        }); 
    }
    return {
        init
    };
})();
var manageBookCategories = (function () {
    function init($container) {
        let $submitBtn = $container.find('#save-btn'),
            $btnAddSubjectRow = $container.find('#add-subject-row-btn'),
            $bookSubjectTableDiv = $container.find('#bookSubject'),
            counter = 0,
            newTemplateSubjectRow =
                '<tr class="sub-row">' +
                '   <td><input type="text" class="form-control subject-name fs-5" required></td>' +
                '   <td id="for-book-categories">' +
                '       <a class="btn btn-warning fs-5 p-1 col-12 toggle-categories" data-toggle="collapse" href="" role="button" aria-expanded="false" aria-controls="">' +
                '           Скрий Категории' +
                '       </a > ' +
                '       <div class="card book-categories-table show" id="">' +
                '               <div class="card-header"></div>' +
                '               <div class="card-body">' +
                '                 <table class="col-12">' +
                '                     <thead>' +
                '                         <tr>' +
                '                             <th class="fs-4 text-center">Име на категорията</th>' +
                '                             <th></th>' +
                '                         </tr>' +
                '                     </thead>' +
                '                     <tbody class="book-category-tbody">' +
                '                     </tbody>' +
                '                 </table>' +
                '                <div class="d-flex pt-3">'+
                '                   <button type="button" class="btn btn-primary col-12 add-book-category-row-btn fs-5" id=""><i class="fas fa-plus"></i> Добави нова категория</button>'+
                '                </div>' +
                '              </div>' +
                '           </div>' +
                '   </td>' +
                '   <td>' +
                '       <button type="button" class="btn btn-danger delete-row m-1"><i class="fa fa-trash"></i></button>' +
                '   </td>' +
                '</tr>';


        $submitBtn.click(function () {
            commonFuncs.startLoader();
            let selectedBookSubjectsDTO = [],
                selectedBookCategoriesDTO = [];

            let $subjectTable = $container.find('.subject-table');

            $subjectTable.find('tbody .sub-row').each(function () {
                const $row = $(this),
                    subjectName = $row.find('.subject-name').val().trim();
                const subject = {
                    SubjectName: subjectName,
                };

                $row.find('tbody .cat-row').each(function () {
                    const $catRow = $(this),
                        categoryName = $catRow.find('.category-name').val().trim();
                    const category = {
                        CategoryName: categoryName,
                        SubjectName: subject.SubjectName
                    };

                    selectedBookCategoriesDTO.push(category);
                });

                selectedBookSubjectsDTO.push(subject);
            });

            const validator = new FormValidator(),
                isValid = validator.validateBookSubjectsAndCategories(selectedBookSubjectsDTO, selectedBookCategoriesDTO);
            if (isValid) {

                $.post('/Librarian/ManageBookCategories', {
                    bookSubjectsDTO: selectedBookSubjectsDTO,
                    bookCategoriesDTO: selectedBookCategoriesDTO
                }, function (response) {
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
            else {
                let errors = validator.getErrors(),
                    errorList = document.createElement("ul");

                errors.forEach((error) => {
                    const listItem = document.createElement("li");
                    listItem.textContent = error;
                    listItem.style.color = "red";
                    listItem.style.textAlign = "left";
                    errorList.appendChild(listItem);
                });

                commonFuncs.endLoader();
                Swal.fire({
                    icon: 'error',
                    title: 'Упс...',
                    html: `${errorList.innerHTML}`,
                    showClass: {
                        popup: 'animate__animated animate__fadeInDown'
                    },
                    hideClass: {
                        popup: 'animate__animated animate__fadeOutUp'
                    }
                });
            }
        });

        class FormValidator {
            constructor() {
                this.errors = [];
            }

            validateBookSubjectsAndCategories(subjects, categories) {
                this.errors = [];

                this.validateBookSubjects(subjects);
                this.validateBookCategories(categories);

                // Check for duplicates
                this.checkForDuplicates(subjects, categories);

                return this.errors.length === 0;
            }

            validateBookCategories(categories) {
                if (!categories || categories.length === 0) {
                    this.errors.push('At least one category is required.');
                } else {
                    categories.forEach((category) => {
                        if (!category.CategoryName || category.CategoryName.trim() === '') {
                            this.errors.push('Category name is required.');
                        }
                    });
                }
            }

            validateBookSubjects(subjects) {
                if (!subjects || subjects.length === 0) {
                    this.errors.push('At least one subcategory is required.');
                } else {
                    subjects.forEach((subject) => {
                        if (!subject.SubjectName || subject.SubjectName.trim() === '') {
                            this.errors.push('Subcategory name is required.');
                        }
                    });
                }
            }

            checkForDuplicates(subjects, categories) {
                const allNames = subjects.map(subject => subject.SubjectName.toLowerCase())
                    .concat(categories.map(category => category.CategoryName.toLowerCase()));

                const duplicates = allNames.filter((name, index, array) => array.indexOf(name) !== index);

                if (duplicates.length > 0) {
                    this.errors.push('Duplicate names found: ' + duplicates.join(', '));
                }
            }

            getErrors() {
                return this.errors;
            }
        }

        $container.on('click', '.add-book-category-row-btn', function () {
            commonFuncs.startLoader();
            var $button = $(this);
            var $tempTableDiv = $button.closest('.book-categories-table');
            var $nearestTbody = $tempTableDiv.find('tbody');

            let categoryRow = '<tr class="cat-row">' +
                '   <td><input type="text" class="form-control category-name fs-5" required></td>' +
                '   <td>' +
                '       <button type="button" class="btn btn-danger delete-row m-1"><i class="fa fa-trash"></i></button>' +
                '   </td>' +
                '</tr>';

            let $newRow = $(categoryRow);

            $nearestTbody.append($newRow);
            commonFuncs.endLoader();
        });

        $btnAddSubjectRow.click(function () {
            commonFuncs.startLoader();
            counter++;

            let $newRow = $(newTemplateSubjectRow);

            let $aTag = $newRow.find('a');
            let $categoryDiv = $newRow.find('.book-categories-table');

            $categoryDiv.attr('id', 'book-categories-table-' + counter + '');

            $aTag.attr('aria-controls', 'book-categories-table-' + counter + '');
            $aTag.attr('href', '#book-categories-table-' + counter + '');


            $bookSubjectTableDiv.find('#subjects-tbody').append($newRow);
            commonFuncs.endLoader();
        });

        $(document).on('click', '.delete-row',function () {
            Swal.fire({
                title: 'Сигурни ли сте?',
                text: "Няма да можете да върнете този ред!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Да, Изтрий!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $(this).closest('tr').remove();
                }
            })
        });

        $container.on('change', '.subject-name', function () {
            let $categoryTable = $bookSubjectTableDiv.closest('#book-categories-table')
            $categoryTable.attr('id', 'for-categories-' + $(this).val() + '');
            $categoryTable.attr('href', '#for-categories-' + $(this).val() + '');
        });


        $(document).on('click', '.toggle-categories',function () {
            var $this = $(this);
            var expanded = $this.attr('aria-expanded');
            if (expanded === "true") {
                $this.text('Скрий Категории');
            } else {
                $this.text('Покажи Категории');
            }
        });
    }
    return {
        init
    };
})();
var manageMemberships = (function () {
    function init($container) {

        $container.on('click', '.addMembershipBtn', function () {
            $('#addMembershipModal').modal('show');
        });

        $container.on('click', '.editBtn', function () {
            commonFuncs.startLoader();
            var membershipId = $(this).data('id');
            var membershipName = $(this).data('name');
            var membershipStarterPoints = $(this).data('start');
            var membershipExitPoints = $(this).data('end');

            $('#membershipId').val(membershipId);
            $('#editMembershipName').val(membershipName);
            $('#editStarterNeededPoints').val(membershipStarterPoints);
            $('#editNeededAmountOfPoints').val(membershipExitPoints);

            $('#editMembershipModal').modal('show');
            commonFuncs.endLoader();
        });

        $container.on('click', '.deleteBtn', function () {
            var membershipItemId = $(this).data('id');
            Swal.fire({
                title: 'Сигурни ли сте че искате да изтриете членството ?',
                text: "След натискането на 'Потвърждавам' няма да можете да върнете членството!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Потвърждавам!'
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.fire({
                        title: 'Накъде искате да преместите съществуващите елементи в даденото членство?',
                        text: 'Нагоре(в горната категория по точки), Надолу(в по-долната категория по точки)',
                        icon: 'question',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Нагоре',
                        cancelButtonText: 'Надолу'
                    }).then((transferResult) => {
                        commonFuncs.startLoader();
                        var isUpperConfirmed = transferResult.isConfirmed;
                        $.ajax({
                            url: '/Admin/DeleteMembership',
                            type: 'POST',
                            data: { id: membershipItemId, upper: isUpperConfirmed },
                            success: function (response) {
                                commonFuncs.endLoader();
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Успех',
                                    text: response.message,
                                    showConfirmButton: false,
                                    timer: 3000
                                }).then((result) => {
                                    location.reload();
                                });
                            },
                            error: function (xhr, status, error) {
                                commonFuncs.endLoader();
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Грешка',
                                    text: 'Възникна грешка. Опитайте отново'
                                });
                            }
                        });
                    });
                }
            });
        });


        $('#addMembershipForm').submit(function (event) {
            commonFuncs.startLoader();
            event.preventDefault();

            var membershipName = $('#addMembershipName').val();
            var starterNeededPoints = parseInt($('#addStarterNeededPoints').val());
            var neededAmountOfPoints = parseInt($('#addNeededAmountOfPoints').val());

            if (membershipName === '' || isNaN(starterNeededPoints) || isNaN(neededAmountOfPoints) ||
                starterNeededPoints < 0 || neededAmountOfPoints <= starterNeededPoints || neededAmountOfPoints < 0) {
                commonFuncs.endLoader();
                Swal.fire({
                    icon: 'error',
                    title: 'Грешка',
                    text: 'Моля въведете данните с адекватни стойности.'
                });
                return;
            }

            var formData = $(this).serialize();

            $.ajax({
                url: '/Admin/AddMembership',
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response.status) {
                        commonFuncs.endLoader();
                        Swal.fire({
                            icon: 'success',
                            title: 'Успех',
                            text: response.message,
                            showConfirmButton: false,
                            timer: 3000
                        }).then((result) => {
                            location.reload();
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
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                    Swal.fire({
                        icon: 'error',
                        title: 'Грешка',
                        text: 'Възникна грешка. Опитайте пак.'
                    });
                }
            });
        });

        $('#editMembershipForm').submit(function (event) {
            commonFuncs.startLoader();
            event.preventDefault();
            var membershipName = $('#editMembershipName').val().trim();
            var starterNeededPoints = parseInt($('#editStarterNeededPoints').val().trim());
            var neededAmountOfPoints = parseInt($('#editNeededAmountOfPoints').val().trim());

            if (membershipName === '' || isNaN(starterNeededPoints) || isNaN(neededAmountOfPoints) ||
                starterNeededPoints <= 0 || neededAmountOfPoints <= starterNeededPoints || neededAmountOfPoints < 0) {
                commonFuncs.endLoader();
                Swal.fire({

                    icon: 'error',
                    title: 'Грешка',
                    text: 'Моля въведете данните с адекватни стойности.'
                });
                return;
            }

            var formData = $(this).serialize();

            $.ajax({
                url: '/Admin/EditMembership',
                type: 'POST',
                data: formData,
                success: function (response) {
                    if (response.status) {
                        commonFuncs.endLoader();
                        Swal.fire({
                            icon: 'success',
                            title: 'Успех',
                            text: response.message,
                            showConfirmButton: false,
                            timer: 3000
                        }).then((result) => {
                            location.reload();
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
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                    commonFuncs.endLoader();
                    Swal.fire({
                        icon: 'error',
                        title: 'Грешка',
                        text: 'Възникна грешка. Опитайте пак.'
                    });
                }
            });
        });

    }
    return {
        init
    };
})();

var reportPageLibrarian = (function () {
    function init($container) {
      
        function loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity) {
            $.ajax({
                type: 'POST',
                url: '/Admin/LoadBookInformation',
                data: {
                    startDate: startDateEntity,
                    endDate: endDateEntity,
                    selectedCountOfItems: selectedCountOfItemsEntity
                },
                success: function (response) {
                    if (response.status) {
                        var books = response.data;

                        var fragment = document.createDocumentFragment();
                        if (books && books.length > 0) {
                            books.forEach(function (book) {

                                var link = document.createElement('a');
                                link.href = '/Home/BookPage?bookId=' + book.id;

                                var columnDiv = document.createElement('div');
                                columnDiv.className = 'col-md-4 mb-4'; 

                                var imageContainer = document.createElement('div');
                                imageContainer.className = 'd-flex justify-content-center align-items-center mb-2 book-container';
                                imageContainer.style.height = '30rem';

                                var image = document.createElement('img');
                                image.src = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/image-for-book-' + book.id + '.png';
                                image.style.maxWidth = '100%';
                                image.style.maxHeight = '100%';

                                var nameDiv = document.createElement('div');
                                nameDiv.className = 'h3 text-center py-3 text-dark';
                                nameDiv.textContent = book.name;

                                imageContainer.appendChild(image);
                                link.appendChild(imageContainer);
                                link.appendChild(nameDiv);
                                columnDiv.appendChild(link);

                                fragment.appendChild(columnDiv);
                            });
                        } else {
                            var message = document.createElement('h2');
                            message.textContent = 'Няма записи в базата между тези дати!';
                            message.style.textAlign = 'center';
                            message.style.color = 'red';
                            fragment.appendChild(message);
                        }
                        var container = $('#bookContainer');
                        container.empty();
                        container.append(fragment);
                    } else {
                        console.error(response.Message);
                    }
                },
                error: function (xhr, status, error) {
                    console.error(error);
                }
            });
        }

        $(document).on('change', '#bookSelect', function () {
            commonFuncs.startLoader();
            if ($(this).val() === 'Personalized') {
                $('#book-personalized-row').show();
                commonFuncs.endLoader();
            } else {
                $('#book-personalized-row').hide();
                var dateEntity = $(this).val();
                var startDateEntity = '';
                var endDateEntity = '';

                var currentDate = new Date();

                switch (dateEntity) {
                    case 'Today':
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Yesterday':
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 48));
                        endDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        break;
                    case 'Week':
                        startDateEntity = getFormattedDateTime(getFirstDayOfWeek(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Month':
                        startDateEntity = getFormattedDateTime(getFirstDayOfMonth(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Year':
                        startDateEntity = getFormattedDateTime(getFirstDayOfYear(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    default:
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                }

                var selectedCountOfItemsEntity = $('#amountOfBookItems').val();

                loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity);
                commonFuncs.endLoader();
            }
        });

        $('#personalized-book-time-btn').click(function () {
            commonFuncs.startLoader();
            var startDateEntity = $('#from-date-book').val();
            var endDateEntity = $('#to-date-book').val();
            var selectedCountOfItemsEntity = $('#amountOfBookItems').val();

            loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity);
            commonFuncs.endLoader();
        });

        $(document).on('change', '#amountOfBookItems', function () {
            commonFuncs.startLoader();
            var dateEntity = $('#bookSelect').val();
            var startDateEntity = '';
            var endDateEntity = '';

            var currentDate = new Date();

            switch (dateEntity) {
                case 'Today':
                    startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
                case 'Yesterday':
                    startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 48));
                    endDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                    break;
                case 'Week':
                    startDateEntity = getFormattedDateTime(getFirstDayOfWeek(currentDate));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
                case 'Month':
                    startDateEntity = getFormattedDateTime(getFirstDayOfMonth(currentDate));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
                case 'Year':
                    startDateEntity = getFormattedDateTime(getFirstDayOfYear(currentDate));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
                default:
                    startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
            }

            var selectedCountOfItemsEntity = $(this).val();

            loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity);
            commonFuncs.endLoader();
        });

        function getDateHoursAgo(date, hours) {
            return new Date(date.getTime() - (hours * 60 * 60 * 1000));
        }

        function getFirstDayOfWeek(date) {
            var dayOfWeek = date.getDay();
            var diff = date.getDate() - dayOfWeek + (dayOfWeek === 0 ? -6 : 1);
            return new Date(date.setDate(diff));
        }

        function getFirstDayOfMonth(date) {
            return new Date(date.getFullYear(), date.getMonth(), 1);
        }

        function getFirstDayOfYear(date) {
            return new Date(date.getFullYear(), 0, 1);
        }

        function getFormattedDateTime(date) {
            var year = date.getFullYear();
            var month = (date.getMonth() + 1).toString().padStart(2, '0');
            var day = date.getDate().toString().padStart(2, '0');
            var hours = date.getHours().toString().padStart(2, '0');
            var minutes = date.getMinutes().toString().padStart(2, '0');
            return year + '-' + month + '-' + day + 'T' + hours + ':' + minutes;
        }

        var isChartLoading = false; 

        $(document).on('change', '#genreSelect', function () {
            commonFuncs.startLoader();
            if ($(this).val() === 'Personalized') {
                $('#genre-personalized-row').show();
                $('#personalized-genre-time-btn').trigger('click');
                commonFuncs.endLoader();
            } else {
                $('#genre-personalized-row').hide();
                var dateEntity = $(this).val();
                var startDateEntity = '';
                var endDateEntity = '';

                var currentDate = new Date();

                switch (dateEntity) {
                    case 'Today':
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Yesterday':
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 48));
                        endDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        break;
                    case 'Week':
                        startDateEntity = getFormattedDateTime(getFirstDayOfWeek(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Month':
                        startDateEntity = getFormattedDateTime(getFirstDayOfMonth(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Year':
                        startDateEntity = getFormattedDateTime(getFirstDayOfYear(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    default:
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                }

                if (!isChartLoading) {
                    isChartLoading = true;

                    $.ajax({
                        type: 'POST',
                        url: '/Admin/LoadGenreInformation',
                        data: {
                            startDate: startDateEntity,
                            endDate: endDateEntity,
                        },
                        success: function (response) {
                            if (response.status) {
                                var genresData = response.data;
                                var canvas = document.getElementById('genreChart');
                                var ctx = canvas.getContext('2d');
                                ctx.clearRect(0, 0, canvas.width, canvas.height);
                                if (genresData && genresData.length > 0) {
                                    commonFuncs.endLoader();
                                    $('#no-chart-items-in-db').hide();
                                    $('#genreChart').show();
                                    loadGenreChart(genresData);
                                }
                                else {
                                    commonFuncs.endLoader();
                                    $('#no-chart-items-in-db').show();
                                    $('#genreChart').hide();
                                }
                            } else {
                                console.error(response.Message);
                            }

                            isChartLoading = false;
                        },
                        error: function (xhr, status, error) {
                            console.error(error);
                            isChartLoading = false; 
                        }
                    });
                }
            }
        });

        $('#personalized-genre-time-btn').click(function () {
            commonFuncs.startLoader();
            var startDateEntity = $('#from-date-genre').val();
            var endDateEntity = $('#to-date-genre').val();

            if (!isChartLoading) {
                isChartLoading = true; 

                $.ajax({
                    type: 'POST',
                    url: '/Admin/LoadGenreInformation',
                    data: {
                        startDate: startDateEntity,
                        endDate: endDateEntity,
                    },
                    success: function (response) {
                        if (response.status) {
                            var genresData = response.data;

                            var canvas = document.getElementById('genreChart');
                            var ctx = canvas.getContext('2d');
                            ctx.clearRect(0, 0, canvas.width, canvas.height);
                            if (genresData && genresData.length > 0) {
                                commonFuncs.endLoader();
                                $('#no-chart-items-in-db').hide();
                                $('#genreChart').show();
                                loadGenreChart(genresData);
                            }
                            else {
                                commonFuncs.endLoader();
                                $('#no-chart-items-in-db').show();
                                $('#genreChart').hide();
                            }
                        } else {
                            commonFuncs.endLoader();
                            console.error(response.Message);
                        }

                        isChartLoading = false; 
                    },
                    error: function (xhr, status, error) {
                        console.error(error);
                        isChartLoading = false; 
                    }
                });
            }
        });

        function loadGenreChart(genresData) {
            var canvas = document.getElementById('genreChart');
            var ctx = canvas.getContext('2d');

            // Clear the canvas
            ctx.clearRect(0, 0, canvas.width, canvas.height);

            // Destroy previous chart instance if it exists
            if (window.myChart) {
                window.myChart.destroy();
            }

            var genres = [];
            var percentages = [];
            var colors = [];

            genresData.forEach(function (item) {
                var parts = item.split('-');
                var genreName = parts[0].trim();
                var percentage = parseFloat(parts[1].trim());

                genres.push(genreName);
                percentages.push(percentage);

                var randomColor = '#' + Math.floor(Math.random() * 16777215).toString(16);
                colors.push(randomColor);
            });

            window.myChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: genres,
                    datasets: [{
                        label: 'Percentage',
                        data: percentages,
                        backgroundColor: colors,
                        borderWidth: 1
                    }]
                },
                options: {
                    plugins: {
                        datalabels: {
                            formatter: (value, ctx) => {
                                let sum = 0;
                                let dataArr = ctx.chart.data.datasets[0].data;
                                dataArr.map(data => {
                                    sum += data;
                                });
                                let percentage = (value * 100 / sum).toFixed(2) + "%";
                                return percentage;
                            },
                            color: '#fff',
                        }
                    },
                    legend: {
                        display: true,
                        position: 'left',
                        align: 'start',
                        labels: {
                            boxWidth: 10
                        },
                        title: {
                            display: true,
                            text: 'Legend'
                        }
                    }
                }
            });
        }

    }
    return {
        init
    };
})();
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