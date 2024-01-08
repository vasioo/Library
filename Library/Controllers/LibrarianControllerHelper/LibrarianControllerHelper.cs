using Library.Models.BaseModels;
using Library.Models.DTO;
using Library.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Web.Controllers.HomeControllerHelper
{
    public class LibrarianControllerHelper : ILibrarianControllerHelper
    {
        #region FieldsAndConstructors
        private readonly IBookService _bookService;
        private readonly IBookCategoryService _bookCategoryService;
        private readonly IBookSubjectService _bookSubjectService;

        public LibrarianControllerHelper(IBookService bookService, IBookCategoryService bookCategoryService, IBookSubjectService bookSubjectService)
        {
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
            _bookSubjectService = bookSubjectService;
        }
        #endregion

        #region BooksHelpers
        public async Task<bool> AddABookToDatabase(BookDTO book, string image)
        {
            try
            {
                var bookCat = _bookCategoryService.GetBookCategoryByBookCategoryName(book.Genre!.CategoryName);

                var bookNew = new Book();

                bookNew.Id = book.Id;
                bookNew.Name = book.Name;
                bookNew.Author = book.Author;
                bookNew.DateOfBookCreation = book.DateOfBookCreation;
                bookNew.Genre = bookCat;
                bookNew.Description = book.Description;
                bookNew.AvailableItems = book.AvailableItems;
                bookNew.NeededMembership = book.NeededMembership;
                bookNew.FavouriteBooks = book.FavouriteBooks;

                await _bookService.AddAsync(bookNew);

                await _bookService.SaveImage(book.Id, image);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<string> AddBookSubjectAndCategoriesToDb(List<BookSubjectDTO> bookSubjectsDTO, List<BookCategoryDTO> bookCategoriesDTO)
        {
            try
            {
                string out2 = await RemoveAllCategoriesThatAreNotInTheDTO(bookCategoriesDTO);
                string out1 = await RemoveAllSubjectsThatAreNotInTheDTO(bookSubjectsDTO);

                if (!String.IsNullOrEmpty(out1) || !String.IsNullOrEmpty(out2))
                {
                    return out1 + out2;
                }

                foreach (var subject in bookSubjectsDTO)
                {
                    var subjectObj = await _bookSubjectService
                        .IQueryableGetAllAsync()
                        .Where(x => x.SubjectName == subject.SubjectName).Include(x => x.BookCategories)
                        .FirstOrDefaultAsync();

                    if (subjectObj != null)
                    {
                        var nonExistentBookCategories = new List<BookCategory>();
                        foreach (var category in bookCategoriesDTO.Where(x => x.SubjectName == subjectObj.SubjectName))
                        {
                            var categoryObj = await _bookCategoryService.IQueryableGetAllAsync().Where(x => x.CategoryName == category.CategoryName).FirstOrDefaultAsync();

                            if (categoryObj == null)
                            {
                                var catObj = new BookCategory();
                                catObj.CategoryName = category!.CategoryName;

                                var entity = await _bookCategoryService.AddAsync(catObj);
                                catObj.Id = entity;

                                nonExistentBookCategories.Add(catObj);
                            }
                        }

                        foreach (var cat in nonExistentBookCategories)
                        {
                            subjectObj.BookCategories.Add(cat);
                        }
                        await _bookSubjectService.UpdateAsync(subjectObj);
                    }

                    else
                    {
                        var bSubj = new BookSubject();
                        bSubj.SubjectName = subject.SubjectName;

                        var bookCategories = new List<BookCategory>();
                        foreach (var category in bookCategoriesDTO.Where(x => x.SubjectName == subject.SubjectName))
                        {
                            var bCat = new BookCategory();
                            bCat.CategoryName = category.CategoryName;

                            var entityId = await _bookCategoryService.AddAsync(bCat);

                            bookCategories.Add(await _bookCategoryService.GetByIdAsync(entityId));
                        }
                        bSubj.BookCategories = bookCategories;
                        await _bookSubjectService.AddAsync(bSubj);
                    }
                }
                return "";

            }
            catch (Exception)
            {
                return "An error occured!";
                throw;
            }
        }

        private async Task<string> RemoveAllSubjectsThatAreNotInTheDTO(List<BookSubjectDTO> bookSubjectDTOs)
        {
            var dbSubjects = await _bookSubjectService.GetAllAsync();
            var output = "";
            var subjectsToRemove = dbSubjects
                .Where(dbSubject => !bookSubjectDTOs
                    .Any(dto => string.Equals(dto.SubjectName, dbSubject.SubjectName, StringComparison.OrdinalIgnoreCase)))
                 .ToList();

            foreach (var subjectToRemove in subjectsToRemove)
            {

                foreach (var categ in subjectToRemove.BookCategories)
                {
                    if (!_bookService.IQueryableGetAllAsync().Where(x => x.Genre == categ).Any())
                    {
                        await _bookCategoryService.RemoveAsync(categ.Id);
                    }
                    else
                    {
                        output += $"The category: {categ.CategoryName} has books connected to it and cannot be deleted!-";
                    }
                }
                if (subjectToRemove.BookCategories.Count() == 0)
                {
                    await _bookSubjectService.RemoveAsync(subjectToRemove.Id);
                }
                else
                {
                    output += $"The subject: {subjectToRemove.SubjectName} has books connected to it and cannot be deleted!-";
                }

            }
            return output;
        }

        private async Task<string> RemoveAllCategoriesThatAreNotInTheDTO(List<BookCategoryDTO> bookCategoryDTOs)
        {
            var dbCategories = await _bookCategoryService.GetAllAsync();
            var output = "";
            var categoriesToRemove = dbCategories
                .Where(dbCategory =>
                    !bookCategoryDTOs.Any(dto =>
                        string.Equals(dto.CategoryName, dbCategory.CategoryName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // Remove identified categories
            foreach (var categoryToRemove in categoriesToRemove)
            {
                if (!_bookService.IQueryableGetAllAsync().Where(x => x.Genre == categoryToRemove).Any())
                {
                    await _bookCategoryService.RemoveAsync(categoryToRemove.Id);
                }
                else
                {
                    output += $"The category: {categoryToRemove.CategoryName} has books connected to it and cannot be deleted!-";
                }
            }
            return output;
        }

        public async Task<bool> EditABook(BookDTO book, string imageObj)
        {
            try
            {
                var bookCat = _bookCategoryService.GetBookCategoryByBookCategoryName(book.Genre!.CategoryName);

                var bookNew = new Book();

                bookNew.Id = book.Id;
                bookNew.Name = book.Name;
                bookNew.Author = book.Author;
                bookNew.DateOfBookCreation = book.DateOfBookCreation;
                bookNew.Genre = bookCat;
                bookNew.Description = book.Description;
                bookNew.AvailableItems = book.AvailableItems;
                bookNew.NeededMembership = book.NeededMembership;
                bookNew.FavouriteBooks = book.FavouriteBooks;


                await _bookService.UpdateAsync(bookNew);

                await _bookService.UpdateImageData(book.Id, imageObj);

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public IQueryable<string> GetAllBookCategories()
        {
            return _bookCategoryService.IQueryableGetAllAsync().Select(x => x.CategoryName);
        }

        public IQueryable<BookSubject> GetAllBookSubjects()
        {
            return _bookSubjectService.IQueryableGetAllAsync();
        }

        public async Task<Book> GetBook(int bookId)
        {
            return await _bookService.GetByIdAsync(bookId);
        }

        public IQueryable<Book> GetAllBooks()
        {
            return _bookService.IQueryableGetAllAsync();
        }

        public async Task<int> RemoveABook(int bookId)
        {
            return await _bookService.RemoveAsync(bookId);
        }
        #endregion
    }
}
