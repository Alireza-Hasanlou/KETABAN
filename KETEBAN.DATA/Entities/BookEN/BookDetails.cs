using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Entities.BookEN
{
    public class BookDetails
    {
        public int BookId { get; set; }

        public string? Summary { get; set; }
        public DateTime year_of_publication { get; set; }
        public int Pages { get; set; }
        public DateTime AddedDate { get; set; }

        #region Navigation
        public Book Book { get; set; }
        #endregion
    }
}
