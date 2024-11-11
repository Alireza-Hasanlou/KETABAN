using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Entities.BookEN
{
    public  class BookLanguage
    {
        public int Id { get; set; }
        public string Language { get; set; }

        #region Nvigation
        public ICollection<Book> Book { get; set; }
        #endregion
    }
}
