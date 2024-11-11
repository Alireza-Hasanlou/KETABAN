using KETABAN.CORE.DTOs;
using KETABAN.CORE.Helper;
using KETABAN.CORE.Sender.EmailSender;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Context;
using KETEBAN.DATA.Entities.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Security.Policy;






namespace KETABAN.CORE.Services
{
    public class AccountManagementServices : IAccountManagementServices
    {
        private readonly UserManager<Librarian> _UserManager;
        private readonly SignInManager<Librarian> _SignInManager;


        public AccountManagementServices(SignInManager<Librarian> signInManager, UserManager<Librarian> userManager)
        {
            _SignInManager = signInManager;
            _UserManager = userManager;

        }
        // تولید توکن بازنشانی رمز عبور
        public async Task<string> GeneratePasswordResetToken(Librarian librarian)
        {
            return await _UserManager.GeneratePasswordResetTokenAsync(librarian);
        }

        // ارسال ایمیل برای بازنشانی رمز عبور
        public async Task<IdentityResult> SendResetPasswordEmail(string Email, string body)
        {
            if (Email == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "Email was null", Description = "ایمیل نمیتواند خالی باشد" });
            }

            if (body == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "Callback URL was null", Description = "محتوای ایمیل نمی‌تواند خالی باشد" });
            }

            try
            {
                // ارسال ایمیل با استفاده از سرویس ارسال ایمیل
                SendingEmailService sendingEmail = new SendingEmailService();
                await sendingEmail.SendEmailAsync(Email, "بازنشانی کلمه عبور", body);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Code = "Sending Email Failed", Description = ex.Message });
            }
        }

        // ورود به سیستم
        public async Task<IdentityResult> Login(LoginDTO loginDTO)
        {
            var user = await _UserManager.FindByNameAsync(loginDTO.UserName);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "User Not Found", Description = "نام کاربری یافت نشد" });
            }

            // افزودن ادعا برای نمایش آواتار
            Claim claim = new Claim("AvatarName", user.AvatarName, ClaimValueTypes.String);
            await _UserManager.AddClaimAsync(user, claim);

            var result = await _SignInManager.PasswordSignInAsync(user, loginDTO.Password, false, true);

            if (result.Succeeded)
            {
                return IdentityResult.Success;
            }
            else if (result.IsLockedOut)
            {
                return IdentityResult.Failed(new IdentityError { Code = "Is Locked Out", Description = "حساب کاربری شما قفل شده لطفا یک ساعت صبر کنید یا با مدیر سایت تماس بگیرید" });
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Code = "UserName or Password is Not Correct", Description = "نام کاربری یا رمز عبور صحیح نمی‌باشد" });
            }
        }

        // خروج از سیستم
        public async void Logout()
        {
            await _SignInManager.SignOutAsync();
        }

        // بازنشانی رمز عبور
        public async Task<IdentityResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _UserManager.FindByIdAsync(resetPasswordDTO.UserId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "Reset Password Failed", Description = "کاربر یافت نشد" });
            }

            var result = await _UserManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.Password);
            if (result.Succeeded)
            {
                return IdentityResult.Success;
            }

            // ترکیب پیام‌های خطا در صورت بروز خطا
            string message = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            return IdentityResult.Failed(new IdentityError { Code = "Reset Password Failed", Description = message });
        }

        // دریافت اطلاعات کتابدار
        public async Task<OperationResult<EditProfileDto>> GetLiberarianInfo(string LiberarianUserName)
        {
            var result = new OperationResult<EditProfileDto>();

            if (string.IsNullOrEmpty(LiberarianUserName))
            {
                result.IsSuccess = false;
                result.ErrorDetails = "شناسه کتابدار نمی‌تواند خالی باشد";
                return result;
            }

            var liberarian = await _UserManager.Users
                .Where(i => i.UserName == LiberarianUserName)
                .Select(l => new EditProfileDto
                {
                    Id = l.Id,
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    Email = l.Email,
                    PhoneNumber = l.PhoneNumber,
                    NationalCode = l.NationalCode,
                    UserName = l.UserName,
                    DateOfBirth = l.DateOfBirth,
                    AvatarName = l.AvatarName
                })
                .FirstOrDefaultAsync();

            if (liberarian == null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "کتابداری با شناسه ارسال شده یافت نشد";
                return result;
            }

            result.IsSuccess = true;
            result.Data = liberarian;
            return result;
        }

        // ویرایش اطلاعات کتابدار
        public async Task<OperationResult<int>> UpdateLiberarianInfo(EditProfileDto updateProfileDTO)
        {
            var result = new OperationResult<int>();
            try
            {
                var liberarian = await _UserManager.FindByIdAsync(updateProfileDTO.Id);
                // تغییر نام کاربری در صورت نیاز
                if (updateProfileDTO.UserName != liberarian.UserName)
                {
                    await _UserManager.SetUserNameAsync(liberarian, updateProfileDTO.UserName);
                }

                liberarian.FirstName = updateProfileDTO.FirstName;
                liberarian.LastName = updateProfileDTO.LastName;
                liberarian.Email = updateProfileDTO.Email;
                liberarian.PhoneNumber = updateProfileDTO.PhoneNumber;

                // بارگذاری آواتار جدید در صورت وجود
                if (updateProfileDTO.Avatar != null)
                {
                    string avatarName = NameGenerator.nameGenerator() + Path.GetExtension(updateProfileDTO.Avatar.FileName);
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Avatars", avatarName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        updateProfileDTO.Avatar.CopyTo(stream);
                    }

                    liberarian.AvatarName = avatarName;
                }

                var updateResult = await _UserManager.UpdateAsync(liberarian);
                
                
                    
                    var currentClaims = await _UserManager.GetClaimsAsync(liberarian);

                  
                    var existingAvatarClaim = currentClaims.FirstOrDefault(c => c.Type == "AvatarName");
                    if (existingAvatarClaim != null)
                    {
                        await _UserManager.RemoveClaimAsync(liberarian, existingAvatarClaim);
                    }

                  
                    var newAvatarClaim = new Claim("AvatarName", liberarian.AvatarName);
                    await _UserManager.AddClaimAsync(liberarian, newAvatarClaim);
                    result.IsSuccess = true;
                    return result;
                

               
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "ویرایش پروفایل با خطا مواجه شد: " + ex.Message;
                return result;
            }
        }

    }
}
