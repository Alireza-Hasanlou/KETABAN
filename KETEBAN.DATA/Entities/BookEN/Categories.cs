using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Entities.BookEN
{
    public class Categories
    {
        public int Id { get; set; }
        public string Genre { get; set; }
     

        #region Navigation
        public ICollection<Book> books { get; set; }
        #endregion
    }
}
