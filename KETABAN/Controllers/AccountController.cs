
using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace KETABAN.Controllers
{
	public class AccountController : Controller
	{
		private readonly IAccountManagementServices _AccountManagementServices;
		private readonly ILibrariansManagementServices _LibrariansManagementServices;
		public AccountController(IAccountManagementServices accountManagementServices, ILibrariansManagementServices librariansManagementServices)
		{
			_AccountManagementServices = accountManagementServices;
			_LibrariansManagementServices = librariansManagementServices;
		}

        // نمایش صفحه ورود به سیستم
      
        public IActionResult Login()
        {
            return View();
        }

        // انجام عملیات ورود به سیستم
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return View(loginDTO);

            // خروج کاربر در صورتی که قبلاً وارد سیستم شده باشد
            if (User.Identity.IsAuthenticated)
                _AccountManagementServices.Logout();

            var result = await _AccountManagementServices.Login(loginDTO);

            if (result.Succeeded)
            {
                ViewBag.LoginStatus = "Successfully";
                return View(loginDTO);
            }
            else
            {
                // جمع‌آوری و نمایش خطاهای ورود
                ViewData["Errors"] = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                return View(loginDTO);
            }
        }

        // خروج از سیستم و بازگشت به صفحه ورود
        [Route("/Logout")]
        public IActionResult Logout()
        {
            _AccountManagementServices.Logout();
            return RedirectToAction("Login", "Account");
        }

        // نمایش صفحه فراموشی رمز عبور
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // ارسال ایمیل بازیابی رمز عبور
        [HttpPost]
        [Route("/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgtoPasswordDTO forgtoPasswordDTO)
        {
            if (!ModelState.IsValid)
                return View(forgtoPasswordDTO);

            var librarianId = await _LibrariansManagementServices.GetlibrarianIdbyEmailAsync(forgtoPasswordDTO.Email);
            if (librarianId == null)
            {
                ViewData["Errors"] = "کاربری با این ایمیل یافت نشد";
                return View(forgtoPasswordDTO);
            }

            var librarian = await _LibrariansManagementServices.GetLibraianByidAsync(librarianId);
            var token = await _AccountManagementServices.GeneratePasswordResetToken(librarian);
            var callBackUrl = $"https://localhost:44380" + Url.Action(nameof(ResetPassword), new { UserId = librarianId, Token = token });
            string body = $"لطفا برای تغییر کلمه عبور روی لینک زیر کلیک کنید!  <br/> <a href={callBackUrl}> Link </a>";

            var result = await _AccountManagementServices.SendResetPasswordEmail(forgtoPasswordDTO.Email, body);

            if (result.Succeeded)
            {
                ViewBag.SendingEmailStatus = "Successfully";
                return View();
            }

            // جمع‌آوری و نمایش خطاهای ارسال ایمیل
            ViewData["Errors"] = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            return View();
        }

        // نمایش صفحه تغییر رمز عبور پس از کلیک روی لینک ارسال شده به ایمیل
        [Route("/ResetPassword")]
        public IActionResult ResetPassword(string UserId, string Token)
        {
            return View(new ResetPasswordDTO { UserId = UserId, Token = Token });
        }

        // انجام عملیات تغییر رمز عبور
        [Route("/ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordDTO);

            var result = await _AccountManagementServices.ResetPassword(resetPasswordDTO);
            if (result.Succeeded)
            {
                ViewBag.ResetPasswordStatus = "Successfully";
                return View();
            }

            // جمع‌آوری و نمایش خطاهای تغییر رمز عبور
            ViewData["Errors"] = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            return View(resetPasswordDTO);
        }

        // نمایش فرم ویرایش پروفایل کاربری
        public async Task<IActionResult> EditProfile()
        {
            var name = User.Identity.Name;
            var librarian = await _AccountManagementServices.GetLiberarianInfo(name);
            return View(librarian.Data);
        }

        // ویرایش اطلاعات پروفایل کاربری
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDto editProfileDto)
        {
            var result = await _AccountManagementServices.UpdateLiberarianInfo(editProfileDto);
            if (!result.IsSuccess)
            {
                ViewData["Errors"] = result.ErrorDetails;
                return View(editProfileDto);
            }

            _AccountManagementServices.Logout();
            return RedirectToAction(nameof(Logout));
        }

        // نمایش صفحه دسترسی غیرمجاز
        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
