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
    public interface IAccountManagementServices
    {
        Task<IdentityResult> Login(LoginDTO loginDTO);
        void Logout();
        Task<IdentityResult> SendResetPasswordEmail(string Email, string body);
        Task< string> GeneratePasswordResetToken(Librarian librarian);
        Task<IdentityResult> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        Task<OperationResult<EditProfileDto>> GetLiberarianInfo(string LiberarianUserName);
        Task<OperationResult<int>> UpdateLiberarianInfo(EditProfileDto updateProfileDTO);
    }
}
