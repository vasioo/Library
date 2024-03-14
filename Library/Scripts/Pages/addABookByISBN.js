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