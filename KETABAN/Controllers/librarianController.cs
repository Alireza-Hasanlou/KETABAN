using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace KETABAN.Controllers
{
    [Authorize(Roles = "administrator")]
    public class librarianController : Controller
    {
        private readonly ILibrariansManagementServices _LibrariansManagementServices;
        public librarianController(ILibrariansManagementServices librariansManagementServices)
        {
            _LibrariansManagementServices = librariansManagementServices;
        }

        // ` برای نمایش لیست کتابداران با امکان جستجو و فیلتر بر اساس پارامترها
        [HttpGet]
        public IActionResult Index(string FirstName = "", string LastName = "", string PhoneNumber = "", string Email = "", int CurrentPage = 1)
        {
            var librarianList = _LibrariansManagementServices.GetLibrarians(FirstName, LastName, PhoneNumber, Email, CurrentPage);
            return View(librarianList);
        }

        // برای نمایش فرم ایجاد کتابدار جدید
        public IActionResult Create()
        {
            return View();
        }

        //   برای پردازش داده‌های وارد شده و ایجاد کتابدار جدید
        [HttpPost]
        public async Task<IActionResult> Create(AddLibrarianDTO librarianDTO, string gender)
        {
            // بررسی صحت داده‌های ورودی
            if (!ModelState.IsValid)
                return View();

            // فراخوانی متد ایجاد کتابدار
            var result = await _LibrariansManagementServices.CreateAsync(librarianDTO, gender);

            if (result.Succeeded)
            {
                ViewBag.CreateLibrarianStatus = "Successfully";
                return View(librarianDTO);
            }
            else
            {
                // در صورت بروز خطا، نمایش پیام‌های خطا
                string message = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                ViewData["Errors"] = message;
                return View();
            }
        }

        // `Json` برای حذف کتابدار بر اساس نام کاربری با پاسخ به‌صورت 
        public JsonResult Delete(string UserName)
        {
            var result = _LibrariansManagementServices.DeleteAsync(UserName).Result;

            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            else
            {
                //`Json` در صورت بروز خطا، نمایش پیام‌های خطا به‌صورت 
                string errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = errors });
            }
        }

        //  برای نمایش اطلاعات کتابدار جهت ویرایش
        public async Task<IActionResult> Update(string UserName)
        {
            var user = await _LibrariansManagementServices.GetLibraianByUserNameAsync(UserName);
            return View(user);
        }

        //  برای ذخیره تغییرات اعمال‌شده در اطلاعات کتابدار
        [HttpPost]
        public async Task<IActionResult> Update(LibrarianDto librarianDtos)
        {
            // بررسی صحت داده‌های ورودی
            if (!ModelState.IsValid)
                return View(librarianDtos);

            // فراخوانی متد بروزرسانی کتابدار
            var result = await _LibrariansManagementServices.UpdateAsync(librarianDtos);

            if (result.Succeeded)
            {
                ViewBag.UpdateLibrarianStatus = "Successfully";
                return View(librarianDtos);
            }
            else
            {
                // در صورت بروز خطا، نمایش پیام‌های خطا
                string message = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                ViewData["Errors"] = message;
                return View();
            }
        }

    }
}

