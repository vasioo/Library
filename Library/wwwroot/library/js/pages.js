var addABook = (function () {
    function init($container) {

        $('#addBookButton').click(function () {
            var bookData = {
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre:  $('#Genre').val(),
                Description: $('#Description').val(),
                AvailableItems: $('#AvailableItems').val(),
                NeededMembership: $('#NeededMembership').val(),
                AmountOfBooks: $('#AmountOfBooks').val()
            };


            const image = $('.uploaded-image').attr('src');

            $.post('/Librarian/AddABookPost', {
                imageObj: image,
                book: bookData
            },
                function (response) {
                    Swal.fire({
                        icon: 'info',
                        title: 'Server response',
                        html: `${response.message}`,
                        showClass: {
                            popup: 'animate__animated animate__fadeInDown'
                        },
                        hideClass: {
                            popup: 'animate__animated animate__fadeOutUp'
                        }
                    });

                    location.reload();
                }).fail(function (error) {
                    console.log('AJAX request failed:', error);
                });
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
                        title: 'Image Validation Failed',
                        text: 'Error details: ' + imageData
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
            $('#bookTitle').val(book.title);
            $('#bookAuthor').val(book.publishers[0]);
            var publishDate = new Date(book.publish_date);
            var year = publishDate.getFullYear(); 
            $('#bookCreationDate').val(year + '-01-01');
            $('#category').text(`Техен жанр: ${book.subjects[0]}`);
            var languageKey = book.languages[0].key;
            var languageCode = languageKey.split('/').pop();
            $('#language').val(languageCode);
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
        $('#bookForm input, #bookForm textarea, #bookForm select').on('input', function () {
            $(this).removeClass('error');
        });
        $('#saveFormData').click(function (e) {
            e.preventDefault();

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
                        Swal.fire("Success", response.message, "success");
                    } else {
                        Swal.fire("Error", response.message, "error");
                    }
                },
                error: function () {
                    Swal.fire("Error", "An error occurred while sending the request.", "error");
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
            var neededName = $('#CategoryName').val();

            $.post('/Librarian/AddABookCategory', {
                categoryName:neededName
            },
                function (response) {
                    Swal.fire({
                        icon: 'info',
                        title: 'Server response',
                        html: `${response.message}`,
                        showClass: {
                            popup: 'animate__animated animate__fadeInDown'
                        },
                        hideClass: {
                            popup: 'animate__animated animate__fadeOutUp'
                        }
                    });

                    location.reload();
                }).fail(function (error) {
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

            var formDataObject = {
                Title: $('#title').val(),
                Content: $('#tiny').val(),
            };
            const image = $('.uploaded-image').attr('src');
            $.post('/Librarian/AddDocument', {
                doc: formDataObject,
                docImage: image
            }, function (response) {
                Swal.fire({
                    icon: 'info',
                    title: 'Server response',
                    html: `${response.message}`,
                    showClass: {
                        popup: 'animate__animated animate__fadeInDown'
                    },
                    hideClass: {
                        popup: 'animate__animated animate__fadeOutUp'
                    }
                });

                location.reload();
            }).fail(function (error) {
                console.log('AJAX request failed:', error);
            });
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
                    if (response.status === true) {
                        Swal.fire({
                            icon: 'success',
                            title: "Ще бъдете прехвърлени до 5 сек.",
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

                    } else {
                        // Handle failure
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
                    // Handle AJAX request failure
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Something went wrong!',
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
                    if (response.success) {
                        swal({
                            title: 'Успех!',
                            text: response.message,
                            icon: 'success'
                        });
                    } else {
                        swal({
                            title: 'Грешка!',
                            text: response.message,
                            icon: 'error'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    swal({
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
            var book = $(this).attr('id');
            $.ajax({
                type: 'POST',
                url: '/Home/ReadBook',
                data: { isbn: book },
                dataType: 'json',
                success: function (response) {
                    if (response.status === true) {
                        Swal.fire({
                            icon: 'success',
                            title: response.message,
                            showClass: {
                                popup: 'animate__animated animate__fadeInDown'
                            },
                            hideClass: {
                                popup: 'animate__animated animate__fadeOutUp'
                            }
                        }).then(function () {
                            if (response.previewUrl) {
                                Swal.fire({
                                    title: 'Redirecting...',
                                    text: 'You will be redirected shortly',
                                    timer: 3000,
                                    showConfirmButton: false,
                                    allowOutsideClick: false,
                                    timerProgressBar: true,
                                    onBeforeOpen: () => {
                                        Swal.showLoading();
                                    }
                                }).then(() => {
                                    window.location.href = response.previewUrl;
                                });
                            }
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
                        title: 'Oops...',
                        text: 'Something went wrong!',
                    });
                    console.error('AJAX request failed:', error);
                }
            });
        });

    }
    return {
        init
    };
})();
var clientManagement = (function () {
    function init($container) {
    }
    return {
        init
    };
})();
var editABook = (function () {
    function init($container) {
        $('#editBookButton').click(function () {
            var bookData = {
                Id: $('#editBookForm').data('id'),
                Name: $('#Name').val(),
                Author: $('#Author').val(),
                DateOfBookCreation: $('#DateOfBookCreation').val(),
                Genre: $('#Genre').val() ,
                Description: $('#Description').val(),
                AvailableItems: $('#AvailableItems').val(),
                NeededMembership: $('#NeededMembership').val(),
                AmountOfBooks: $('#AmountOfBooks').val()
            };


            const image = $('.uploaded-image').attr('src');

            $.post('/Librarian/EditABookPost', {
                imageObj: image,
                book: bookData
            },
                function (response) {
                    Swal.fire({
                        icon: 'info',
                        title: 'Server response',
                        html: `${response.message}`,
                        showClass: {
                            popup: 'animate__animated animate__fadeInDown'
                        },
                        hideClass: {
                            popup: 'animate__animated animate__fadeOutUp'
                        }
                    });

                    location.reload();
                }).fail(function (error) {
                    console.log('AJAX request failed:', error);
                });
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
                        title: 'Image Validation Failed',
                        text: 'Error details: ' + imageData
                    });
                }
            });
        });

        $('.remove-book').click(function () {

            var book = $(this).attr('id');

            Swal.fire({
                title: 'WARNING?',
                text: "Are you sure you want to remove the book ?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Remove!'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.post('/Librarian/RemoveABook', { bookId:book }, function (response) {
                        location.reload();
                    });
                    location.reload();
                }
            })
        });
    }
    return {
        init
    };
})();
var editDocumentPage = (function () {
    function init($container) {
        $(document).on('click', '#submit-btn', function () {

            var formDataObject = {
                Title: $('#title').val(),
                Content: $('#tiny').val(),
                DateOfCreation: $('#DateOfCreation').val(),
                Id:$('#Id').val(),
            };
            const image = $('.uploaded-image').attr('src');
            $.post('/Librarian/EditDocumentPost', {
                doc: formDataObject,
                docImage: image
            }, function (response) {
                Swal.fire({
                    icon: 'info',
                    title: 'Server response',
                    html: `${response.message}`,
                    showClass: {
                        popup: 'animate__animated animate__fadeInDown'
                    },
                    hideClass: {
                        popup: 'animate__animated animate__fadeOutUp'
                    }
                });

                location.reload();
            }).fail(function (error) {
                console.log('AJAX request failed:', error);
            });
        });
    }
    return {
        init
    };
})();
var editStaffInformation = (function () {
    function init($container) {
    }
    return {
        init
    };
})();
var leasedTracker = (function () {
    function init($container) {
        $('#deleteBtn').on('click', function (e) {
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/DeleteUserLeasedEntity",
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });

        $('#stopLeasingBtn').on('click', function (e) {
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/StopLeasing",
                data: { userLeasedId: userLeasedId },
                success: function (response) {
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });

        $('#leaseBookBtn').on('click', function (e) {
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/LeaseBookOrNot",
                data: { userLeasedId: userLeasedId, lease: true },
                success: function (response) {
                    location.reload();
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });

        $('#rejectLeaseBtn').on('click', function (e) {
            e.preventDefault();
            var userLeasedId = $(this).closest('.operations-container').data('userleasedid');
            $.ajax({
                type: 'POST',
                url: "/Librarian/LeaseBookOrNot",
                data: { userLeasedId: userLeasedId, lease: false },
                success: function (response) {
                    location.reload();
                },
                error: function (xhr, status, error) {
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
                '   <td><input type="text" class="form-control subject-name" required></td>' +
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

                    Swal.fire({
                        icon: 'info',
                        title: 'Server response',
                        html: `${response.message}`,
                        showClass: {
                            popup: 'animate__animated animate__fadeInDown'
                        },
                        hideClass: {
                            popup: 'animate__animated animate__fadeOutUp'
                        }
                    });

                    location.reload();

                }).fail(function (error) {
                    // Handle the AJAX request failure
                    // This function will be executed if the AJAX request encounters an error
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


                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
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

            var $button = $(this);
            var $tempTableDiv = $button.closest('.book-categories-table');
            var $nearestTbody = $tempTableDiv.find('tbody');

            let categoryRow = '<tr class="cat-row">' +
                '   <td><input type="text" class="form-control category-name" required></td>' +
                '   <td>' +
                '       <button type="button" class="btn btn-danger delete-row m-1"><i class="fa fa-trash"></i></button>' +
                '   </td>' +
                '</tr>';

            let $newRow = $(categoryRow);

            $nearestTbody.append($newRow);

        });

        $btnAddSubjectRow.click(function () {

            counter++;

            let $newRow = $(newTemplateSubjectRow);

            let $aTag = $newRow.find('a');
            let $categoryDiv = $newRow.find('.book-categories-table');

            $categoryDiv.attr('id', 'book-categories-table-' + counter + '');

            $aTag.attr('aria-controls', 'book-categories-table-' + counter + '');
            $aTag.attr('href', '#book-categories-table-' + counter + '');


            $bookSubjectTableDiv.find('#subjects-tbody').append($newRow);

        });

        $(document).on('click', '.delete-row',function () {
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this row!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, Delete!'
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
            var membershipId = $(this).data('id');
            var membershipName = $(this).data('name');
            var membershipStarterPoints = $(this).data('start');
            var membershipExitPoints = $(this).data('end');

            $('#membershipId').val(membershipId);
            $('#editMembershipName').val(membershipName);
            $('#editStarterNeededPoints').val(membershipStarterPoints);
            $('#editNeededAmountOfPoints').val(membershipExitPoints);

            $('#editMembershipModal').modal('show');
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
                        text:'Нагоре(в горната категория по точки), Надолу(в по-долната категория по точки)',
                        icon: 'question',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Нагоре',
                        cancelButtonText: 'Надолу'
                    }).then((transferResult) => {
                        var isUpperConfirmed = transferResult.isConfirmed;
                        $.ajax({
                            url: '/Admin/DeleteMembership',
                            type: 'POST',
                            data: { id: membershipItemId, upper: isUpperConfirmed },
                            success: function (response) {
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
            event.preventDefault();

            var membershipName = $('#addMembershipName').val();
            var starterNeededPoints = parseInt($('#addStarterNeededPoints').val());
            var neededAmountOfPoints = parseInt($('#addNeededAmountOfPoints').val());

            if (membershipName === '' || isNaN(starterNeededPoints) || isNaN(neededAmountOfPoints) ||
                starterNeededPoints <0 || neededAmountOfPoints <= starterNeededPoints || neededAmountOfPoints < 0) {
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
            event.preventDefault();
            var membershipName = $('#editMembershipName').val().trim();
            var starterNeededPoints = parseInt($('#editStarterNeededPoints').val().trim());
            var neededAmountOfPoints = parseInt($('#editNeededAmountOfPoints').val().trim());

            // Check if input fields are valid
            if (membershipName === '' || isNaN(starterNeededPoints) || isNaN(neededAmountOfPoints) ||
                starterNeededPoints <= 0 || neededAmountOfPoints <= starterNeededPoints || neededAmountOfPoints < 0) {
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
                url: '/Librarian/LoadBookInformation',
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

                                var imageContainer = document.createElement('div');
                                imageContainer.className = 'd-flex justify-content-center align-items-center mb-2 book-container';
                                imageContainer.style.height = '30rem';
                                imageContainer.style.width = '30rem';
                                imageContainer.style.border = '1px solid #ccc';

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

                                fragment.appendChild(link);
                            });
                        } else {
                            var message = document.createElement('h2');
                            message.textContent = 'Няма записи в базата между тези дати!';
                            message.style.textAlign = 'center';
                            message.style.color = 'red';
                            fragment.appendChild(message);
                        }
                        var container = document.getElementById('bookContainer');
                        container.innerHTML = '';
                        container.appendChild(fragment);
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
            if ($(this).val() === 'Personalized') {
                $('#book-personalized-row').show();
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
            }
        });

        $('#personalized-book-time-btn').click(function () {
            var startDateEntity = $('#from-date-book').val();
            var endDateEntity = $('#to-date-book').val();
            var selectedCountOfItemsEntity = $('#amountOfBookItems').val();

            loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity);
        });

        $(document).on('change', '#amountOfBookItems', function () {
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

        var isChartLoading = false; // Flag to indicate whether a chart is currently being loaded

        $(document).on('change', '#genreSelect', function () {
            if ($(this).val() === 'Personalized') {
                $('#genre-personalized-row').show();
                $('#personalized-genre-time-btn').trigger('click');
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

                // Check if a chart is currently being loaded
                if (!isChartLoading) {
                    isChartLoading = true; // Set the flag to true to indicate that a chart is being loaded

                    $.ajax({
                        type: 'POST',
                        url: '/Librarian/LoadGenreInformation',
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
                                    $('#no-chart-items-in-db').hide();
                                    $('#genreChart').show();
                                    loadGenreChart(genresData);
                                }
                                else {
                                    $('#no-chart-items-in-db').show();
                                    $('#genreChart').hide();
                                }
                            } else {
                                console.error(response.Message);
                            }

                            isChartLoading = false; // Reset the flag after chart loading is complete
                        },
                        error: function (xhr, status, error) {
                            console.error(error);
                            isChartLoading = false; // Reset the flag in case of an error
                        }
                    });
                }
            }
        });

        $('#personalized-genre-time-btn').click(function () {
            var startDateEntity = $('#from-date-genre').val();
            var endDateEntity = $('#to-date-genre').val();

            // Check if a chart is currently being loaded
            if (!isChartLoading) {
                isChartLoading = true; // Set the flag to true to indicate that a chart is being loaded

                $.ajax({
                    type: 'POST',
                    url: '/Librarian/LoadGenreInformation',
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
                                $('#no-chart-items-in-db').hide();
                                $('#genreChart').show();
                                loadGenreChart(genresData);
                            }
                            else {
                                $('#no-chart-items-in-db').show();
                                $('#genreChart').hide();
                            }
                        } else {
                            console.error(response.Message);
                        }

                        isChartLoading = false; // Reset the flag after chart loading is complete
                    },
                    error: function (xhr, status, error) {
                        console.error(error);
                        isChartLoading = false; // Reset the flag in case of an error
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
var statisticsPage = (function () {
    function init($container) {
        function loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity) {
            $.ajax({
                type: 'POST',
                url: '/Librarian/LoadBookInformation',
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

                                var imageContainer = document.createElement('div');
                                imageContainer.className = 'd-flex justify-content-center align-items-center mb-2 book-container';
                                imageContainer.style.height = '30rem';
                                imageContainer.style.width = '30rem';
                                imageContainer.style.border = '1px solid #ccc';

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

                                fragment.appendChild(link);
                            });
                        } else {
                            var message = document.createElement('h1');
                            message.textContent = 'Няма записи в базата между тези дати!';
                            message.style.textAlign = 'center';
                            message.style.color = 'red';
                            fragment.appendChild(message);
                        }
                        var container = document.getElementById('bookContainer');
                        container.innerHTML = '';
                        container.appendChild(fragment);
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
            if ($(this).val() === 'Personalized') {
                $('#book-personalized-row').show();
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
            }
        });

        $('#personalized-book-time-btn').click(function () {
            var startDateEntity = $('#from-date-book').val();
            var endDateEntity = $('#to-date-book').val();
            var selectedCountOfItemsEntity = $('#amountOfBookItems').val();

            loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity);
        });

        $(document).on('change', '#amountOfBookItems', function () {
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

        var isChartLoading = false; // Flag to indicate whether a chart is currently being loaded

        $(document).on('change', '#genreSelect', function () {
            if ($(this).val() === 'Personalized') {
                $('#genre-personalized-row').show();
                $('#personalized-genre-time-btn').trigger('click');
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

                // Check if a chart is currently being loaded
                if (!isChartLoading) {
                    isChartLoading = true; // Set the flag to true to indicate that a chart is being loaded

                    $.ajax({
                        type: 'POST',
                        url: '/Librarian/LoadGenreInformation',
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
                                    $('#no-chart-items-in-db').hide();
                                    $('#genreChart').show();
                                    loadGenreChart(genresData);
                                }
                                else {
                                    $('#no-chart-items-in-db').show();
                                    $('#genreChart').hide();
                                }
                            } else {
                                console.error(response.Message);
                            }

                            isChartLoading = false; // Reset the flag after chart loading is complete
                        },
                        error: function (xhr, status, error) {
                            console.error(error);
                            isChartLoading = false; // Reset the flag in case of an error
                        }
                    });
                }
            }
        });

        $('#personalized-genre-time-btn').click(function () {
            var startDateEntity = $('#from-date-genre').val();
            var endDateEntity = $('#to-date-genre').val();

            // Check if a chart is currently being loaded
            if (!isChartLoading) {
                isChartLoading = true; // Set the flag to true to indicate that a chart is being loaded

                $.ajax({
                    type: 'POST',
                    url: '/Librarian/LoadGenreInformation',
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
                                $('#no-chart-items-in-db').hide();
                                $('#genreChart').show();
                                loadGenreChart(genresData);
                            }
                            else {
                                $('#no-chart-items-in-db').show();
                                $('#genreChart').hide();
                            }
                        } else {
                            console.error(response.Message);
                        }

                        isChartLoading = false; // Reset the flag after chart loading is complete
                    },
                    error: function (xhr, status, error) {
                        console.error(error);
                        isChartLoading = false; // Reset the flag in case of an error
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
                        Swal.fire({
                            icon: 'error',
                            title: 'Грешка',
                            text: response.message
                        });
                    }
                },
                error: function () {
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