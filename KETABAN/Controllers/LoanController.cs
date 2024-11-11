using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Entities.BookEN;
using KETEBAN.DATA.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace KETABAN.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanManagementServices _LoanManagementServices;
        private readonly IMemoryCache _MemoryCach;
        private readonly ILogger<BookController> _Logger;
        public LoanController(ILoanManagementServices loanManagementServices, IMemoryCache memoryCache, ILogger<BookController> logger)
        {
            _LoanManagementServices = loanManagementServices;
            _MemoryCach = memoryCache;
            _Logger = logger;

        }
        //  برای دریافت داده‌ها از کش یا فراخوانی از دیتابیس در صورت نبودن در کش
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

        //] برای نمایش لیست امانت‌ها در صفحه اصلی
        public IActionResult Index(string BookPosition = "", string BookTitle = "", string StudentName = "", string StudentNumber = "", int CurrentPage = 1)
        {
    
            return View(_LoanManagementServices.ListofLoans(BookPosition, BookTitle, StudentName, StudentNumber, CurrentPage));
        }

        // فراخوانی صفحه امانت دادن کتاب
        public async Task<IActionResult> Borrow(LoanDTO? loanDTO, string? studentNumber, string BookPosition)
        {
            // بررسی وجود شماره دانشجویی و بازیابی اطلاعات دانشجو از کش یا دیتابیس
            if (studentNumber != null)
            {
                var student = await GetOrSetCacheAsync<Student>(studentNumber,
                    () => _LoanManagementServices.GetStudentForLoanAsync(studentNumber));
                if (student == null)  
                {
                    ViewData["StudentsError"] = "دانشجویی با شماره دانشجویی وارد شده یافت نشد";
                    return View(loanDTO);
                }

               
                loanDTO.StudentNumber = student.StudentNum;
                loanDTO.StudnentFirstName = student.FirstName;
                loanDTO.StudentLastName = student.LastName;
            }

            // بررسی وجود موقعیت کتاب و بازیابی اطلاعات کتاب از کش یا دیتابیس
            if (BookPosition != null)
            {
                var book = await GetOrSetCacheAsync<Book>(BookPosition,
                    () => _LoanManagementServices.GetBookForLoanAsync(BookPosition));
                if (book == null)  
                {
                    ViewData["BookErrors"] = "کتابی با شناسه وارد شده یافت نشد";
                    return View(loanDTO);
                }

              
                loanDTO.BookId = book.Id;
                loanDTO.bookPosition = book.BookPosition;
                loanDTO.BookTitle = book.Title;
                loanDTO.AuthorName = book.Author;
            }

            return View(loanDTO); 
        }

        //  برای امانت دادن کتاب به دانشجو
        [HttpPost]
        public async Task<IActionResult> Borrow(LoanDTO loanDTO)
        {
          
            var result = await _LoanManagementServices.LendingbookAsync(loanDTO.StudentNumber, loanDTO.BookId);

            if (result.IsSuccess)
            {
               
                _MemoryCach.Remove(loanDTO.bookPosition);
                _MemoryCach.Remove(loanDTO.StudentNumber);
                ViewBag.LoanStatus = "Successfully";  
                return View(loanDTO);
            }

            ViewData["BookErrors"] = result.ErrorDetails;  
            return View(loanDTO);
        }




        [HttpPost]
        public async Task<JsonResult> BookReturn(int LoanId)
        {

            var result = await _LoanManagementServices.ReturnBookAsync(LoanId);
            if (result.IsSuccess == true)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = $"{result.ErrorDetails}" });
        }

    }
}
