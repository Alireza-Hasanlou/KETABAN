using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Entities.BookEN;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Caching.Memory;
using Org.BouncyCastle.Tls.Crypto.Impl;
using System.Net;

namespace KETABAN.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookManagementServices _BookManagementServices;
        private readonly IMemoryCache _MemoryCach;
        private readonly ILogger<BookController> _Logger;
        public BookController(IBookManagementServices bookManagementServices, IMemoryCache memoryCache, ILogger<BookController> logger)
        {
            _BookManagementServices = bookManagementServices;
            _MemoryCach = memoryCache;
            _Logger = logger;
        }
        // متدی عمومی برای بررسی کش و بازیابی داده‌ها در صورت عدم موجودیت در کش
        private async Task<T> GetOrSetCacheAsync<T>(string cacheKey, Func<Task<T>> fetchFromDb) where T : class
        {
         
            if (_MemoryCach.TryGetValue(cacheKey, out T cachedData))
            {
                _Logger.Log(LogLevel.Information, $"{cacheKey} found in cache");
                return cachedData;
            }

            _Logger.Log(LogLevel.Information, $"{cacheKey} not found in cache, fetching from DB");
            var data = await fetchFromDb(); 

 
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(60)) 
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600)) 
                .SetPriority(CacheItemPriority.Normal);

            _MemoryCach.Set(cacheKey, data, cacheEntryOptions); 
            return data;
        }

        // اکشن متدی برای صفحه اصلی که کش را پاکسازی می‌کند و لیست کتاب‌ها را بازیابی می‌کند
        public IActionResult Index(int bookId, string BookTitle = "", string Author = "", string Publisher = "", int CurrentPage = 1)
        {

            _MemoryCach.Remove("Categories");
            _MemoryCach.Remove("Languages");
            _MemoryCach.Remove("CategoriesId");

    
            return View(_BookManagementServices.GetBooks(bookId, BookTitle, Author, Publisher, CurrentPage));
        }

        // اکشن متد برای نمایش فرم افزودن کتاب جدید
        public async Task<IActionResult> Create()
        {
            var bookOperationDTO = new BookOperationDTOs();

     
            bookOperationDTO.categories = await GetOrSetCacheAsync<IEnumerable<Categories>>(
                "Categories",
                () => _BookManagementServices.GetCategoriesAsync()
            );

            bookOperationDTO.Languages = await GetOrSetCacheAsync<IEnumerable<BookLanguage>>(
                "Languages",
                () => _BookManagementServices.GetLanguagesAsync()
            );

            return View(bookOperationDTO); 
        }

        // اکشن متد برای پردازش فرم افزودن کتاب جدید
        [HttpPost]
        public async Task<IActionResult> Create(BookOperationDTOs newbook, List<int> Categories)
        {
           
            newbook.categories = await GetOrSetCacheAsync<IEnumerable<Categories>>("Categories",
                    () => _BookManagementServices.GetCategoriesAsync());
            newbook.Languages = await GetOrSetCacheAsync<IEnumerable<BookLanguage>>("Languages",
                () => _BookManagementServices.GetLanguagesAsync());

            if (!ModelState.IsValid) 
                return View(newbook);

            var result = await _BookManagementServices.CreateAsync(newbook, Categories);
            if (result.IsSuccess)
            {
                ViewBag.AddBookStatus = "Successfully";
                _Logger.Log(LogLevel.Information, "Book added successfully and cache cleared.");
                return View(newbook);
            }
            else
            {
                ViewData["Errors"] = result.ErrorDetails;
                return View(newbook);
            }
        }

        // اکشن متد برای بارگذاری صفحه ویرایش کتاب بر اساس شناسه کتاب
        public async Task<IActionResult> Update(int BookId)
        {
            var result = await _BookManagementServices.GetBookAsync(BookId);
            if (!result.IsSuccess)
            {
                ViewData["Errors"] = result.ErrorDetails;
                return View();
            }

       
            result.Data.categories = await GetOrSetCacheAsync<IEnumerable<Categories>>("Categories",
                () => _BookManagementServices.GetCategoriesAsync());

            result.Data.CategoriesId = await GetOrSetCacheAsync<IEnumerable<int>>("CategoriesId",
              () => _BookManagementServices.GetselectedBooksCategoryIdAsync(BookId));

            result.Data.Languages = await GetOrSetCacheAsync<IEnumerable<BookLanguage>>("Languages",
                () => _BookManagementServices.GetLanguagesAsync());

            return View(result.Data); 
        }

        // اکشن متد برای پردازش فرم ویرایش کتاب
        [HttpPost]
        public async Task<IActionResult> Update(BookOperationDTOs Book, List<int> Categories)
        {
     
            Book.categories = await GetOrSetCacheAsync<IEnumerable<Categories>>("Categories",
             () => _BookManagementServices.GetCategoriesAsync());
            Book.CategoriesId = await GetOrSetCacheAsync<IEnumerable<int>>("CategoriesId",
             () => _BookManagementServices.GetselectedBooksCategoryIdAsync(Book.BookId));
            Book.Languages = await GetOrSetCacheAsync<IEnumerable<BookLanguage>>("Languages",
             () => _BookManagementServices.GetLanguagesAsync());

            if (!ModelState.IsValid)
                return View(Book);

            var result = await _BookManagementServices.UpdateAsync(Book, Categories);
            if (result.IsSuccess)
            {
                ViewBag.UpdateBookStatus = "Successfully";
                _Logger.Log(LogLevel.Information, "Book Updated successfully and cache cleared.");
                return View(Book);
            }

            ViewData["Errors"] = result.ErrorDetails;
            return View(Book);
        }

        // JSON اکشن متد برای حذف کتاب بر اساس شناسه و بازگردانی نتیجه به صورت 
        public async Task<JsonResult> Delete(int bookid)
        {
            var result = await _BookManagementServices.DeleteAsync(bookid);
            if (result.IsSuccess)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = $"{result.ErrorDetails}" });
        }


    }
}
