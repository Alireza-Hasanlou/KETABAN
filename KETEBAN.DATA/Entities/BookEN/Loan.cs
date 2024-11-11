using KETEBAN.DATA.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Entities.BookEN
{
    public class Loan
    {
        public int LoanId { get; set; }
        public string StudentNumber { get; set; }
        public int BookId { get; set; }
        public DateTime DateofLoan { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool Status { get; set; } //book is returned or not



        #region Navigation
        public Student Student { get; set; }
        public Book Book { get; set; }
        #endregion

    }
}
