using KETEBAN.DATA.Entities.BookEN;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Entities.User
{
    public class Student
    {
        //Key
        public string StudentNum { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int StudentLevelId { get; set; }
        public DateTime DateofRegistration { get; set; }
        public string Gender { get; set; }
   



        #region Navigation
        public StudentsLevels? StudentsLevel { get; set; }
        public ICollection<Loan> Loans{ get; set; }
        #endregion
    }

}
