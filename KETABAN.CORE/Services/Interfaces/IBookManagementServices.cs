using KETABAN.CORE.DTOs;
using KETEBAN.DATA.Entities.BookEN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace KETABAN.CORE.Services.Interfaces
{
    public interface IBookManagementServices
    {
        BookListDto GetBooks(int bookId, string BookTitle = "", string Author = "", string Publisher = "", int CurrentPage = 1);
        Task<OperationResult<BookOperationDTOs>> GetBookAsync(int bookId);
        Task<OperationResult<Book>> CreateAsync(BookOperationDTOs NewBook, List<int> categories);
        Task<OperationResult<Book>> UpdateAsync(BookOperationDTOs NewBook, List<int> categories);
        Task<OperationResult<Book>> DeleteAsync(int BookId);
        Task<IEnumerable<Categories>> GetCategoriesAsync();
        Task<IEnumerable<BookLanguage>> GetLanguagesAsync();
        Task<IEnumerable<int>> GetselectedBooksCategoryIdAsync(int bookid);
        Task<int> NumberofBooks();


    }
}
