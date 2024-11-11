using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Context;
using KETEBAN.DATA.Entities.BookEN;
using KETEBAN.DATA.Entities.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace KETABAN.CORE.Services
{

    public class LoanManagementServices : ILoanManagementServices
    {
        private readonly _KetabanContext _Context;
        public LoanManagementServices(_KetabanContext context)
        {
            _Context = context;
        }

        //  `Bookposition` برای دریافت اطلاعات کتاب بر اساس 
        public async Task<Book> GetBookForLoanAsync(string Bookposition)
        {

            return await _Context.Books.FirstOrDefaultAsync(i => i.BookPosition == Bookposition);
        }

        //  برای نمایش لیست امانت‌ها با امکان فیلتر و صفحه‌بندی
        public ListofLoansDTo ListofLoans(string BookPosition = "", string BookTitle = "", string StudentName = "", string StudentNumber = "", int CurrentPage = 1)
        {
            // انتخاب و آماده‌سازی داده‌های مورد نیاز امانت‌ها
            IQueryable<LoanInfoDTo> result = _Context.loans
                .Include(b => b.Book)
                .Include(s => s.Student)
                .Select(b => new LoanInfoDTo
                {
                    Loanid = b.LoanId,
                    BookId = b.BookId,
                    BookPosition = b.Book.BookPosition,
                    BookTitle = b.Book.Title,
                    StudentName = b.Student.FirstName + " " + b.Student.LastName,
                    StudentNumber = b.StudentNumber,
                    DateofLoan = b.DateofLoan,
                    ReturnDate = b.ReturnDate,
                    Status = b.Status
                });

            // فیلتر ها
            if (!string.IsNullOrEmpty(BookPosition))
                result = result.Where(b => b.BookPosition.Contains(BookPosition));

            if (!string.IsNullOrEmpty(BookTitle))
                result = result.Where(b => b.BookTitle.Contains(BookTitle));

            if (!string.IsNullOrEmpty(StudentName))
                result = result.Where(b => b.StudentName.Contains(StudentName));

            if (!string.IsNullOrEmpty(StudentNumber))
                result = result.Where(b => b.StudentNumber.Contains(StudentNumber));

            // تنظیم صفحه‌بندی و تعداد موارد در هر صفحه
            int take = 10;
            int pageCount = (result.Count() / take) + 1;
            int skip = (CurrentPage - 1) * take;

            ListofLoansDTo borrowingList = new ListofLoansDTo()
            {
                Loans = result.OrderByDescending(f => f.DateofLoan).Skip(skip).Take(take).ToList(),
                CureentPage = CurrentPage,
                PageCount = pageCount
            };

            return borrowingList;
        }

        //  برای دریافت اطلاعات دانشجو بر اساس شماره دانشجویی
        public async Task<Student> GetStudentForLoanAsync(string studentNumber)
        {
            return await _Context.Students.FirstOrDefaultAsync(s => s.StudentNum == studentNumber);
        }

        //  برای امانت دادن کتاب به دانشجو با بررسی محدودیت‌ها
        public async Task<OperationResult<int>> LendingbookAsync(string? studentNumber, int bookId)
        {
            var result = new OperationResult<int>();

            // بررسی داده‌های ورودی
            if (studentNumber == null || bookId < 0)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "مقادیر کتاب یا دانشجو نمی‌توانند خالی باشند";
                return result;
            }

            var book = _Context.Books.Find(bookId);
            if (book==null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "کتاب در دسترس نیست";
                return result;
            }

            var bookLoan = await _Context.loans
                .Where(s => s.StudentNumber == studentNumber && !s.Status)  // status check book is available or not
                .ToListAsync();
            var student = await _Context.Students
                .Include(l => l.StudentsLevel)
                .FirstAsync(i => i.StudentNum == studentNumber);
            int bookLoanCount = bookLoan.Count();

            // بررسی سطح دانشجو و محدودیت تعداد امانت‌ها
            switch (student.StudentsLevel.LevelName)
            {
                case "برنز":
                    if (bookLoanCount >= 2)
                    {
                        result.IsSuccess = false;
                        result.ErrorDetails = "سطح دانشجو برنز است و فقط تا سقف دو کتاب می‌تواند امانت داشته باشد";
                        return result;
                    }
                    break;

                case "نقره‌ای":
                    if (bookLoanCount >= 4)
                    {
                        result.IsSuccess = false;
                        result.ErrorDetails = "سطح دانشجو نقره‌ای است و فقط تا سقف چهار کتاب می‌تواند امانت داشته باشد";
                        return result;
                    }
                    break;

                case "طلایی":
                    if (bookLoanCount >= 6)
                    {
                        result.IsSuccess = false;
                        result.ErrorDetails = "سطح دانشجو طلایی است و فقط تا سقف شش کتاب می‌تواند امانت داشته باشد";
                        return result;
                    }
                    break;
            }

            try
            {
                // ایجاد رکورد جدید امانت و به‌روزرسانی وضعیت کتاب
                Loan loan = new Loan
                {
                    BookId = bookId,
                    StudentNumber = studentNumber,
                    DateofLoan = DateTime.Now,
                    Status = false
                };

                await _Context.loans.AddAsync(loan);
                book.IsAvailable = false;
                await _Context.SaveChangesAsync();

                // افزایش تعداد کتاب‌های امانتی دانشجو
                await _Context.Students
                    .Where(i => i.StudentNum == studentNumber)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.BooksBorrowedCount, d => d.BooksBorrowedCount + 1));

                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = $"خطا در عملیات: {ex.Message}";
                return result;
            }
        }

        //  برای محاسبه تعداد کل امانت‌های فعال
        public async Task<int> NumberOfLoans()
        {
            return await _Context.loans.CountAsync(l => !l.Status);
        }

        // متد `ReturnBookAsync` برای بازگرداندن کتاب و به‌روزرسانی وضعیت امانت
        public async Task<OperationResult<Loan>> ReturnBookAsync(int loanId)
        {
            var result = new OperationResult<Loan>();

            // بررسی شناسه ورودی
            if (loanId <= 0)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "شناسه نمی‌تواند خالی باشد";
                return result;
            }

            var loan = _Context.loans
                .Include(b => b.Book)
                .FirstOrDefault(l => l.LoanId == loanId);

            if (loan == null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "داده‌ای با شناسه وارد شده یافت نشد";
                return result;
            }

            try
            {
                // به‌روزرسانی وضعیت امانت به بازگشت
                loan.Status = true;
                loan.ReturnDate = DateTime.Now;
                loan.Book.IsAvailable = true;
                await _Context.SaveChangesAsync();

                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = $"خطا در عملیات: {ex.Message}";
                return result;
            }
        }

    }
}
