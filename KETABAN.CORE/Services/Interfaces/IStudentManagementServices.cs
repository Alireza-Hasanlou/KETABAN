using KETABAN.CORE.DTOs;
using Microsoft.AspNetCore;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KETEBAN.DATA.Entities.User;

namespace KETABAN.CORE.Services.Interfaces
{
    public interface IStudentManagementServices
    {
        StudentListDTO GetStudents(string FirstName = "", string LastName = "", string PhoneNumber = "", string Email = "", int CurrentPage = 1);
        Task<OperationResult<StudentOperationDTOs>> GetStudentAsync(string StudentNum);
        Task<OperationResult<Student>> CreateAsync(StudentOperationDTOs student, string gender);
        Task<OperationResult<Student>> UpdateAsync(StudentOperationDTOs student, string gender);
        Task<OperationResult<Student>> DeleteAsync(string StudentNum);
       Task< IEnumerable<SelectListItem>> GetStudentsLevelsAsync();
        Task<int> NumberofStudents();


    }
}
