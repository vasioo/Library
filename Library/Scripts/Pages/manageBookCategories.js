var manageBookCategories = (function () {
    function init($container) {
        let $submitBtn = $container.find('#save-btn'),
            $btnAddCategoryRow = $container.find('#add-category-row-btn'),
            $bookSubjectTableDiv = $container.find('#bookSubject'),
            counter = 0,
            newTemplateCategoryRow =
                '<tr class="cat-row">' +
                '   <td><input type="text" class="form-control subject-name" required></td>' +
                '   <td id="for-book-categories">' +
                '       <a class="btn btn-primary" data-toggle="collapse" href="" role="button" aria-expanded="false" aria-controls="">' +
                '        Categories' +
                '       </a > ' +
                '       <div class="card bookCategoriesTable" id="">' +
                '               <div class="card-header"></div>' +
                '               <div class="card-body">' +
                '                 <table>' +
                '                     <thead>' +
                '                         <tr>' +
                '                             <th>Book Category Name</th>' +
                '                             <th></th>' +
                '                         </tr>' +
                '                     </thead>' +
                '                     <tbody class="book-category-tbody">' +
                '                         <!--Add options for change-->' +
                '                     </tbody>' +
                '                 </table>' +
                '                 <button type="button" class="btn btn-primary add-book-category-row-btn" id=""><i class="fas fa-plus"></i> Add Book Category</button>' +
                '              </div>' +
                '           </div>' +
                '   </td>' +
                '   <td>' +
                '       <button type="button" class="btn btn-danger delete-row m-1"><i class="fa fa-trash"></i></button>' +
                '   </td>' +
                '</tr>';
    }
    return {
        init
    };
})();