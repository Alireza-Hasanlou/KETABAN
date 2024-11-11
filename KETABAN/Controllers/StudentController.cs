using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services;
using KETABAN.CORE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static System.Runtime.InteropServices.JavaScript.JSType;





namespace KETABAN.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentManagementServices _studentManagementServices;
        private readonly IMemoryCache _MemoryCach;
        private readonly ILogger<BookController> _Logger;
        public StudentController(IStudentManagementServices studentManagementServices)
        {
            _studentManagementServices = studentManagementServices;
      
        }

        // متد نمایش صفحه اصلی و فهرست دانشجویان با قابلیت جستجو و صفحه‌بندی
        public IActionResult Index(string FirstName = "", string LastName = "", string PhoneNumber = "", string Email = "", int CurrentPage = 1)
        {
            return View(_studentManagementServices.GetStudents(FirstName, LastName, PhoneNumber, Email, CurrentPage));
        }

        // متد نمایش فرم ایجاد دانشجوی جدید  
        public async Task<IActionResult> Create()
        {
            var levels = await _studentManagementServices.GetStudentsLevelsAsync();
            ViewData["Studentslevels"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(levels, "Value", "Text", 4);
            return View();
        }

        // متد ثبت اطلاعات دانشجوی جدید در سیستم
        [HttpPost]
        public async Task<IActionResult> Create(StudentOperationDTOs student, string Gender, List<System.Web.Mvc.SelectListItem> StudentLevels)
        {
            var levels = await _studentManagementServices.GetStudentsLevelsAsync();
            ViewData["Studentslevels"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(levels, "Value", "Text", 4);
            if (!ModelState.IsValid)
            {
                return View(student);
            }


            var result = await _studentManagementServices.CreateAsync(student, Gender);
            if (result.IsSuccess == true)
            {
                ViewBag.CreateStudentStatus = "SuccessFully";
                return View(student);
            }
            else
            {

                return View(student);
            }

        }

      
        public async Task<IActionResult> Update(string StudentNum)
        {
            var Student = await _studentManagementServices.GetStudentAsync(StudentNum);
            if (Student.IsSuccess == true)
            {
                var studentlevel =await _studentManagementServices.GetStudentsLevelsAsync();
                ViewData["Studentslevels"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(studentlevel, "Value", "Text");
                return View(Student.Data);
            }

            ViewData["Errors"] = Student.ErrorDetails;
            return View(Student.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(StudentOperationDTOs student, string Gender)
        {
            var levels = await _studentManagementServices.GetStudentsLevelsAsync();
            ViewData["Studentslevels"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(levels, "Value", "Text");
            if (!ModelState.IsValid)
                return View(student);

            var result = await _studentManagementServices.UpdateAsync(student, Gender);
            if (result.IsSuccess == true)
            {
                ViewBag.CreateStudentStatus = "SuccessFully";
                return View(student);
            }
            else
            {
                ViewData["Studentslevels"] = TempData["Studentslevels"];
                ViewData["Errors"] = result.ErrorDetails;
                return View(student);
            }


        }

        [HttpPost]
        public async Task<JsonResult> Delete(string UserName)
        {

           var result= await _studentManagementServices.DeleteAsync(UserName);
            if (result.IsSuccess == true)
            {
                return  Json(new { success = true });
            }

            return Json(new { success = false, message = $"{result.ErrorDetails}" });
        }

    }
}
