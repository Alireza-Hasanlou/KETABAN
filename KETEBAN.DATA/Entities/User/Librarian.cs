using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Entities.User
{
    public class Librarian : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateofRegistration { get; set; }
        public string NationalCode { get; set; }
        public string Gender { get; set; }
        public string AvatarName{ get; set; }

    }
}
