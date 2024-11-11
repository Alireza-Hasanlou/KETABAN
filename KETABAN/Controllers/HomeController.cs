using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services;
using KETABAN.CORE.Services.Interfaces;
using KETABAN.Models;
using KETEBAN.DATA.Entities.BookEN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Security.Claims;

namespace KETABAN.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentManagementServices _studentManagementServices;
        private readonly ILoanManagementServices _loanManagementServices;
        private readonly IBookManagementServices _bookManagementServices;
        private readonly ILibrariansManagementServices _librariansManagementServices;
        private readonly IMemoryCache _MemoryCach;
        public HomeController(ILogger<HomeController> logger,IStudentManagementServices studentManagementServices,
               ILoanManagementServices loanManagementServices,IBookManagementServices bookManagementServices )
        {
            _bookManagementServices = bookManagementServices;
            _studentManagementServices = studentManagementServices;
            _loanManagementServices = loanManagementServices; 
      
       
        }

        //[Authorize]
        public async Task< IActionResult> Index()
        {

            ChartdataDto chartdataDto = new ChartdataDto()
            {
                BooksCount   =  await  _bookManagementServices.NumberofBooks(),
                StudentCount =  await  _studentManagementServices.NumberofStudents(),
                LoanCount    =  await  _loanManagementServices.NumberOfLoans()
            };
            return View(chartdataDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
		[Route("Home/NotFound")]
		public IActionResult NotFoundPage(int code)
		{
			Response.StatusCode = 404;
			return View();
		}
	}
}
