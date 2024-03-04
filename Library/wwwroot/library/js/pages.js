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
                NeededMembership: $('#NeededMembership').val()
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
var bookPage = (function () {
    function init($container) {
        let $borrowBookBtn = $container.find('.borrow-book-btn'),
            $UnathBorrowBookBtn = $container.find('.unauth-borrow-book-btn'),
            $unborrowBookBtn = $container.find('.unborrow-book-btn');

        $borrowBookBtn.click(function () {
            var book = $(this).attr('id');

            $.post('/Home/BorrowBook', { bookId: book }, function (response) {
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
            $.ajax({
                type: 'POST',
                url: '/Home/UnborrowBook',
                data: { bookId: book },
                dataType: 'json',
                success: function (response) {
                    if (response.status === true) {
                        // Handle success
                        Swal.fire({
                            icon: 'success',
                            title: response.message,
                            showClass: {
                                popup: 'animate__animated animate__fadeInDown'
                            },
                            hideClass: {
                                popup: 'animate__animated animate__fadeOutUp'
                            }
                        });
                        setTimeout(function () {
                            location.reload()
                        }, 5000);

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
    }
    return {
        init
    };
})();
var borrowBook = (function () {
    function init($container) {
        let $unborrowBookBtn = $container.find('.unborrow-book-btn');

        $unborrowBookBtn.click(function () {
            var book = $(this).attr('id');
            $.ajax({
                type: 'POST',
                url: '/Home/UnborrowBook',
                data: { bookId: book },
                dataType: 'json',
                success: function (response) {
                    if (response.status === true) {
                        // Handle success
                        Swal.fire({
                            icon: 'success',
                            title: response.message,
                            showClass: {
                                popup: 'animate__animated animate__fadeInDown'
                            },
                            hideClass: {
                                popup: 'animate__animated animate__fadeOutUp'
                            }
                        });
                        setTimeout(function () {
                            location.reload()
                        }, 1000);
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
                NeededMembership: $('#NeededMembership').val()
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
var editStaffInformation = (function () {
    function init($container) {
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
        $(document).on('change', '#genreSelect', function () {
            if ($(this).val() === 'Personalized') {
                $('#genre-personalized-row').show();
            } else {
                $('#genre-personalized-row').hide();
            }
        });
        $(document).on('change', '#bookSelect', function () {
            if ($(this).val() === 'Personalized') {
                $('#book-personalized-row').show();
            } else {
                $('#book-personalized-row').hide();
            }
        });
    }
    return {
        init
    };
})();