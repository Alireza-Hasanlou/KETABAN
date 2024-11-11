using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KETEBAN.DATA.Entities.BookEN;
using System.Web.Mvc;

namespace KETABAN.CORE.DTOs
{
    public class BookInfoDto
    {
        public int Bookid { get; set; }
        public string BookPosition { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public bool IsAvailable { get; set; }
        public int Edition { get; set; }

    }

    public class BookListDto
    {
        public List<BookInfoDto> Books { get; set; }
        public int CureentPage { get; set; }
        public int PageCount { get; set; }
    }

    public class BookOperationDTOs
    {
        [DisplayName("شناسه کتاب  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int BookId { get; set; }

        [DisplayName("موقعیت کتاب  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(8, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [RegularExpression(@"^[A-Za-z]{2}[0-9]{6}$", ErrorMessage = " فرمت شناسه نا معتبر است.شناشه باید با 2 حرف انگلیسی شروع و به 6 عدد ختم شود مانند( AB123456).")]
        public string BookPosition { get; set; }

        [DisplayName("عنوان کتاب  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string BookTitle { get; set; }

        [DisplayName("نام نویسنده  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Author { get; set; }

        [DisplayName("ناشر  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [DataType(DataType.EmailAddress)]
        public string Publisher { get; set; }

        [DisplayName("نسخه ویرایش ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Edition { get; set; }

        [DisplayName("خلاصه کتاب  ")]
        [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string? Summary { get; set; }

        [DisplayName("تاریخ انتشار")]
        public DateTime year_of_publication { get; set; }

        [DisplayName("تعداد صفحات  ")]
        public int Pages { get; set; }
        public int LaguageId { get; set; }
        public IEnumerable<Categories>?  categories{ get; set; }
        public IEnumerable<BookLanguage>? Languages{ get; set; }

        //For update 

        [DisplayName("دسته بندی ")]
       
        public IEnumerable<int>? CategoriesId { get; set; }

    }
}
