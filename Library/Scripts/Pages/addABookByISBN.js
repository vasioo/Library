var addABookByISBN = (function () {
    function init($container) {

        function displayBookDetails(book) {
            var isbnData = $('#isbnInput').val();
            $('#isbnOfBook').text(`ISBN:${isbnData}`);
            $('#bookTitle').html(`<input type="text" name="Title" value="${book.title}"required>`);
            $('#bookSubtitle').html(`<input type="text" name="Subtitle" value="${book.subtitle}"required>`);
            $('#otherTitles').html(book.other_titles.map(title => `<input type="text" name="OtherTitles" value="${title}"required>`).join(', '));
            $('#authors').html(book.authors.map(author => `<input type="text" name="Authors" value="${author.name}"required>`).join(', '));
            $('#publishers').html(book.publishers.map(publisher => `<input type="text" name="Publishers" value="${publisher}"required>`).join(', '));
            $('#publishDate').html(`<input type="date" name="PublishDate" value="${book.publish_date}"required>`);
            $('#category').text(book.subject);
            $('#language').html(`<input type="text" name="Language" value="${book.languages[0].name}" required>`);
            $('#physicalFormat').html(`<input type="text" name="PhysicalFormat" value="${book.physical_format}" required>`);
            $('#bookUrl').attr('href', book.url);
            $('#bookCover').attr('src', `https://covers.openlibrary.org/b/id/${book.covers[0]}-L.jpg`);
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
                        } else {
                            Swal.fire({
                                title: "Грешка",
                                text: response.message,
                                icon: "error",
                            });
                        }
                    } else {
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

        $('#saveFormData').click(function (e) {
            e.preventDefault();


            var errors = [];

            $('#bookForm input[type="text"]').each(function () {
                var inputValue = $(this).val();
                var inputName = $(this).attr('name');
                if (!inputValue || inputValue.trim() === '') {
                    errors.push(`Моля попълнете полето "${inputName}".`);
                    $(this).css('outline', '1px solid red');
                }
            });

            $('#bookForm input[type="number"]').each(function () {
                var inputValue = $(this).val();
                var inputName = $(this).attr('name');
                if (!inputValue || inputValue.trim() === ''||inputValue<0) {
                    errors.push(`Полето "${inputName} трябва да е положително и налично".`);
                    $(this).css('outline', '1px solid red');
                }
            });

            if ($('#genreDropdown').val() === '') {
                errors.push('Моля изберете жанр от падащия списък.');
                $('#genreDropdown').css('outline', '1px solid red');
            }

            if (errors.length > 0) {
                var errorMessage = errors.join('\n');
                Swal.fire("Грешка", errorMessage, "error");
                return;
            }

            $.ajax({
                url: $('#bookForm').attr('action'),
                method: 'POST',
                data: $('#bookForm').serialize(),
                success: function (response) {
                    if (response.status) {
                        Swal.fire("Успех", response.message, "success");
                    } else {
                        Swal.fire("Грешка", response.message, "error");
                    }
                },
                error: function () {
                    Swal.fire("Грешка", "Възникна грешка при изпращането на заявката.", "error");
                }
            });
        });


    }
    return {
        init
    };
})();