using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETABAN.CORE.DTOs
{
    public class LoanInfoDTo
    {
        public int Loanid { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string BookTitle { get; set; }
        public int BookId { get; set; }
        public string BookPosition { get; set; }
        public bool Status { get; set; } //Returned or not
        public DateTime DateofLoan { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class ListofLoansDTo
    {
        public List<LoanInfoDTo> Loans { get; set; }

        public int CureentPage { get; set; }

        public int PageCount { get; set; }
    }


    public class LoanDTO
    {
        public int BookId { get; set; }

        [DisplayName("شناسه کتاب  ")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[MaxLength(8, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [RegularExpression(@"^[A-Za-z]{2}[0-9]{6}$", ErrorMessage = " فرمت شناسه نا معتبر است.شناشه باید با 2 حرف انگلیسی شروع و به 6 عدد ختم شود مانند( AB123456).")]
        public string? bookPosition { get; set; }
        public string? BookTitle { get; set; }
        public string? AuthorName { get; set; }


      
        public string? StudentNumber { get; set; }
        public string? StudnentFirstName { get; set; }
        public string? StudentLastName { get; set; }
    }

}