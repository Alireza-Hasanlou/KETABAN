using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Context;
using KETEBAN.DATA.Entities;
using KETEBAN.DATA.Entities.BookEN;
using KETEBAN.DATA.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.WebPages.Html;

namespace KETABAN.CORE.Services
{
    public class BookManagementServices : IBookManagementServices
    {
        private readonly _KetabanContext _Context;
        public BookManagementServices(_KetabanContext context)
        {
            _Context = context;
        }

        // شمارش تعداد کل کتاب‌ها
        public async Task<int> NumberofBooks()
        {
            return await _Context.Books.CountAsync();
        }

        // افزودن کتاب جدید همراه با دسته‌بندی‌های انتخاب‌شده و جزئیات
        public async Task<OperationResult<Book>> CreateAsync(BookOperationDTOs newBook, List<int> categories)
        {
            var result = new OperationResult<Book>();

            // انتخاب دسته‌بندی‌های مورد نظر از پایگاه داده
            var selectedCategories = await _Context.Categories
                .Where(i => categories.Contains(i.Id))
                .ToListAsync();

            try
            {
                // ایجاد شیء کتاب با داده‌های ورودی
                var book = new Book
                {
                    Author = newBook.Author,
                    Title = newBook.BookTitle,
                    Edition = newBook.Edition,
                    Id = newBook.BookId,
                    IsAvailable = true,
                    publisher = newBook.Publisher,
                    BookLanguageId = newBook.LaguageId,
                    BookPosition = newBook.BookPosition,
                    bookDetails = new BookDetails
                    {
                        AddedDate = DateTime.Now,
                        Pages = newBook.Pages,
                        Summary = newBook.Summary,
                        year_of_publication = newBook.year_of_publication,
                        BookId = newBook.BookId
                    },
                    Categories = selectedCategories
                };

                await _Context.Books.AddAsync(book);
                await _Context.SaveChangesAsync();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = $"خطا در افزودن کتاب: {ex.Message}";
            }

            return result;
        }

        // حذف کتاب بر اساس شناسه آن
        public async Task<OperationResult<Book>> DeleteAsync(int bookId)
        {
            var result = new OperationResult<Book>();
            if (bookId <= 0)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "شناسه کتاب نمیتواند خالی باشد";
                return result;
            }

            try
            {
                // یافتن کتاب از پایگاه داده
                var book = await _Context.Books.FindAsync(bookId);
                if (book == null)
                {
                    result.IsSuccess = false;
                    result.ErrorDetails = "کتابی با شناسه مورد نظر یافت نشد";
                    return result;
                }

                // حذف کتاب از پایگاه داده
                _Context.Books.Remove(book);
                await _Context.SaveChangesAsync();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = $"خطا در حذف کتاب: {ex.Message}";
            }

            return result;
        }

        // دریافت اطلاعات کتاب بر اساس شناسه آن
        public async Task<OperationResult<BookOperationDTOs>> GetBookAsync(int bookId)
        {
            var result = new OperationResult<BookOperationDTOs>();
            if (bookId <= 0)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "شناسه کتاب نمیتواند خالی باشد";
                return result;
            }

            var book = await _Context.Books
                .Include(d => d.bookDetails)
                .Where(i => i.Id == bookId)
                .Select(b => new BookOperationDTOs
                {
                    BookId = b.Id,
                    BookPosition = b.BookPosition,
                    Author = b.Author,
                    BookTitle = b.Title,
                    categories = b.Categories,
                    Edition = b.Edition,
                    LaguageId = b.BookLanguageId,
                    Pages = b.bookDetails.Pages,
                    Publisher = b.publisher,
                    Summary = b.bookDetails.Summary,
                    year_of_publication = b.bookDetails.year_of_publication
                }).FirstOrDefaultAsync();

            if (book == null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "کتابی با شناسه وارد شده یافت نشد";
            }
            else
            {
                result.IsSuccess = true;
                result.Data = book;
            }
            return result;
        }

        // دریافت لیست کتاب‌ها با قابلیت فیلتر و صفحه‌بندی
        public BookListDto GetBooks(int bookId, string bookTitle = "", string author = "", string publisher = "", int currentPage = 1)
        {
            IQueryable<BookInfoDto> result = _Context.Books.Select(a => new BookInfoDto
            {
                Bookid = a.Id,
                BookPosition = a.BookPosition,
                Title = a.Title,
                Author = a.Author,
                Edition = a.Edition,
                IsAvailable = a.IsAvailable,
                Publisher = a.publisher
            });

            if (bookId > 0)
                result = result.Where(i => i.Bookid == bookId);
            if (!string.IsNullOrEmpty(bookTitle))
                result = result.Where(f => f.Title.Contains(bookTitle));
            if (!string.IsNullOrEmpty(author))
                result = result.Where(f => f.Author.Contains(author));
            if (!string.IsNullOrEmpty(publisher))
                result = result.Where(f => f.Publisher.Contains(publisher));

            int pageSize = 5;
            int pageCount = (result.Count() + pageSize - 1) / pageSize;
            result = result.OrderByDescending(f => f.Bookid)
                           .Skip((currentPage - 1) * pageSize)
                           .Take(pageSize);

            return new BookListDto
            {
                Books = result.ToList(),
                CureentPage = currentPage,
                PageCount = pageCount
            };
        }

        // دریافت لیست دسته‌بندی‌های موجود
        public async Task<IEnumerable<Categories>> GetCategoriesAsync()
        {
            return await _Context.Categories.ToListAsync();
        }

        // دریافت لیست زبان‌های موجود
        public async Task<IEnumerable<BookLanguage>> GetLanguagesAsync()
        {
            return await _Context.languages.ToListAsync();
        }

        // دریافت شناسه دسته‌بندی‌های انتخاب‌شده برای یک کتاب
        public async Task<IEnumerable<int>> GetselectedBooksCategoryIdAsync(int bookId)
        {
            var book = await _Context.Books
                .Include(c => c.Categories)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            return book?.Categories.Select(c => c.Id) ?? Enumerable.Empty<int>();
        }

        // به‌روزرسانی اطلاعات کتاب و دسته‌بندی‌های آن
        public async Task<OperationResult<Book>> UpdateAsync(BookOperationDTOs bookDto, List<int> categories)
        {
            var result = new OperationResult<Book>();

            try
            {
                var book = await _Context.Books
                    .Include(d => d.bookDetails)
                    .Include(c => c.Categories)
                    .FirstOrDefaultAsync(i => i.Id == bookDto.BookId);

                if (book == null)
                {
                    result.IsSuccess = false;
                    result.ErrorDetails = "کتابی با شناسه مورد نظر یافت نشد";
                    return result;
                }

                book.Title = bookDto.BookTitle;
                book.Author = bookDto.Author;
                book.BookPosition = bookDto.BookPosition;
                book.publisher = bookDto.Publisher;
                book.BookLanguageId = bookDto.LaguageId;
                book.Edition = bookDto.Edition;
                book.bookDetails.Pages = bookDto.Pages;
                book.bookDetails.Summary = bookDto.Summary;
                book.bookDetails.year_of_publication = bookDto.year_of_publication;
                book.Categories = await _Context.Categories
                    .Where(i => categories.Contains(i.Id))
                    .ToListAsync();

                await _Context.SaveChangesAsync();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = $"خطا در ویرایش کتاب: {ex.Message}";
            }

            return result;
        }

    }
}