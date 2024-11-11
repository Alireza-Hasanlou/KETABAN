using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KETABAN.CORE.DTOs
{
    public class StudentInfoDTO
    {

        public string StudentNum { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string StudentLevel { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Time_of_Regesteration { get; set; }
    }

    public class StudentListDTO
    {
        public List<StudentInfoDTO> Students { get; set; }

        public int CureentPage { get; set; }

        public int PageCount { get; set; }
    }
    public class StudentOperationDTOs
    {

        [DisplayName("نام  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string FirstName { get; set; }


        [DisplayName("نام  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string LastName { get; set; }



        [DisplayName("ایمیل  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [DisplayName("شماره دانشجویی  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string studentNum { get; set; }


        [DisplayName("شماره همراه  ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }


        public int StudentLevelId { get; set; }

        public string? Gender { get; set; }
    }


}
