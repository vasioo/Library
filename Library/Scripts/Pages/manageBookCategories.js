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
                    Swal.fire({
                        icon: 'info',
                        title: 'Отговор на сървъра',
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