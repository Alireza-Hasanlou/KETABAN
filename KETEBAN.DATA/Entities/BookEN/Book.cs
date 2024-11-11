using KETEBAN.DATA.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Entities.BookEN
{
    public class Book
    {
        public int Id { get; set; }
        public string BookPosition { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string publisher { get; set; }
        public bool IsAvailable { get; set; }
        public int Edition { get; set; }
        public int BookLanguageId { get; set; }





        #region Navigation

        public ICollection<Categories> Categories { get; set; }
        public BookDetails bookDetails { get; set; }
        public BookLanguage language { get; set; }
        public ICollection<Loan> Loans { get; set; }
        #endregion
    }
}
