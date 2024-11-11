using KETABAN.CORE.DTOs;
using KETEBAN.DATA.Entities.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETABAN.CORE.Services.Interfaces
{
    public interface ILibrariansManagementServices
    {
        Task<IdentityResult> CreateAsync(AddLibrarianDTO librarianDTO, string Gender);
        Task<IdentityResult> DeleteAsync(string UserName);
        Task<IdentityResult> UpdateAsync(LibrarianDto librarianDtos );
        Task<string> GetAvatarName(string UserName);
        Task<LibrarianDto> GetLibraianByUserNameAsync(string UserName);
        Task<string> GetlibrarianIdbyEmailAsync(string Email);
        Task<Librarian> GetLibraianByidAsync(string id);
       LiberaianListDTO GetLibrarians(string FirstName = "", string LastName = "", string PhoneNumber = "", string Email = "", int CurrentPage = 1);

    }
}
