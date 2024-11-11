using KETABAN.CORE.DTOs;
using KETEBAN.DATA.Entities.BookEN;
using KETEBAN.DATA.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETABAN.CORE.Services.Interfaces
{
    public interface ILoanManagementServices
    {
        ListofLoansDTo ListofLoans(string BookPosition = "", string BookTitle = "", string StudentName = "", string StudnetNumber = "", int CurrentPage = 1);
        Task<Book> GetBookForLoanAsync(string Bookposition);
        Task<Student> GetStudentForLoanAsync(string StudnetNumber);
        Task<OperationResult<int>> LendingbookAsync(string studentNumber, int Bookid);
        Task<OperationResult<Loan>> ReturnBookAsync(int loanId);
        Task<int> NumberOfLoans();
    }
}
