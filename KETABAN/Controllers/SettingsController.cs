using KETABAN.CORE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KETABAN.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ISettingsManagementServices _SettingsManagementServices;
        private readonly IBookManagementServices _BookManagementServices;
        public SettingsController(ISettingsManagementServices settingsManagementServices, IBookManagementServices bookManagementServices)
        {
            _BookManagementServices = bookManagementServices;
            _SettingsManagementServices = settingsManagementServices;
        }

        public async Task<IActionResult> Categories()
        {
            ViewData["Categories"] = await _BookManagementServices.GetCategoriesAsync();
            return View();
        }


        public async Task<IActionResult> CreateCategory(string Genre)
        {
            var Result = await _SettingsManagementServices.CreateCategory(Genre);
            if (Result.IsSuccess)
            {
                TempData["CreateStatus"] = "Successfully";
                return RedirectToAction("Categories");
            }

            TempData["Errors"] = Result.ErrorDetails;
            return RedirectToAction("Categories");

        }

        public async Task<IActionResult> DeleteCategory(int CategoryId)
        {
            var Result = await _SettingsManagementServices.DeleteCategory(CategoryId);
            if (Result.IsSuccess)
            {
                TempData["DeleteStatus"] = "Successfully";
                return RedirectToAction("Categories");
            }

            TempData["Errors"] = Result.ErrorDetails;
            return RedirectToAction("Categories");

        }

        public async Task<IActionResult> Languages()
        {
            ViewData["Languages"] = await _BookManagementServices.GetLanguagesAsync();
            return View();
        }

        public async Task<IActionResult> CreateLanguage(string language)
        {
            var Result = await _SettingsManagementServices.CreateLanguage(language);
            if (Result.IsSuccess)
            {
                TempData["CreateStatus"] = "Successfully";
                return RedirectToAction("Languages");
            }

            TempData["Errors"] = Result.ErrorDetails;
            return RedirectToAction("Languages");

        }

        public async Task<IActionResult> Deletelanguage(int languageId)
        {
            var Result = await _SettingsManagementServices.DeleteLanguage(languageId);
            if (Result.IsSuccess)
            {
                TempData["DeleteStatus"] = "Successfully";
                return RedirectToAction("Languages");
            }

            TempData["Errors"] = Result.ErrorDetails;
            return RedirectToAction("Languages");

        }
    }
}
