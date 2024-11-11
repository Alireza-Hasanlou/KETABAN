using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Context;
using KETEBAN.DATA.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KETABAN.CORE.Services
{
    public class StudentManagementServices : IStudentManagementServices
    {
        private readonly _KetabanContext _Context;
        public StudentManagementServices(_KetabanContext context)
        {
            _Context = context;
        }

        // متد ایجاد دانشجو جدید در سیستم
        public async Task<OperationResult<Student>> CreateAsync(StudentOperationDTOs student, string gender)
        {
            var result = new OperationResult<Student>();

            try
            {
                var newStudent = new Student
                {
                    DateOfBirth = student.DateOfBirth,
                    Email = student.Email,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    DateofRegistration = DateTime.Now,
                    Gender = gender,
                    PhoneNumber = student.PhoneNumber,
                    StudentNum = student.studentNum,
                    StudentLevelId = student.StudentLevelId
                };

                await _Context.Students.AddAsync(newStudent);
                await _Context.SaveChangesAsync();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = ex.Message;
            }

            return result;
        }

        // متد حذف دانشجو از سیستم با استفاده از شماره دانشجویی
        public async Task<OperationResult<Student>> DeleteAsync(string StudentNum)
        {
            var result = new OperationResult<Student>();

            if (string.IsNullOrEmpty(StudentNum))
            {
                result.IsSuccess = false;
                result.ErrorDetails = "شماره دانشجویی نمی‌تواند خالی باشد.";
                return result;
            }

            var student = await _Context.Students.FindAsync(StudentNum);

            if (student == null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "دانشجو یافت نشد.";
                return result;
            }

            try
            {
                _Context.Students.Remove(student);
                await _Context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Data = student;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = $"خطا در حذف دانشجو: {ex.Message}";
            }

            return result;
        }

        // متد دریافت اطلاعات دانشجو با شماره دانشجویی
        public async Task<OperationResult<StudentOperationDTOs>> GetStudentAsync(string StudentNum)
        {
            var result = new OperationResult<StudentOperationDTOs>();
            if (string.IsNullOrEmpty(StudentNum))
            {
                result.IsSuccess = false;
                result.ErrorDetails = "شماره دانشجویی نمی‌تواند خالی باشد.";
                return result;
            }

            var student = await _Context.Students.Where(i => i.StudentNum == StudentNum)
                .Select(s => new StudentOperationDTOs
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    DateOfBirth = s.DateOfBirth,
                    StudentLevelId = s.StudentLevelId,
                    studentNum = s.StudentNum,
                    Gender = s.Gender
                }).FirstOrDefaultAsync();

            if (student == null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "دانشجو یافت نشد.";
            }
            else
            {
                result.IsSuccess = true;
                result.Data = student;
            }

            return result;
        }

        // متد دریافت لیست دانشجویان به همراه جستجو و فیلتر کردن
        public StudentListDTO GetStudents(string FirstName = "", string LastName = "", string PhoneNumber = "", string Email = "", int CurrentPage = 1)
        {
            IQueryable<StudentInfoDTO> Result = _Context.Students.Include(l => l.StudentsLevel)
                .Select(a => new StudentInfoDTO
                {
                    StudentLevel = a.StudentsLevel.LevelName,
                    Email = a.Email,
                    FullName = a.FirstName + " " + a.LastName,
                    PhoneNumber = a.PhoneNumber,
                    StudentNum = a.StudentNum,
                    Time_of_Regesteration = a.DateofRegistration
                });

            if (!string.IsNullOrEmpty(FirstName))
                Result = Result.Where(f => f.FullName.Contains(FirstName));

            if (!string.IsNullOrEmpty(LastName))
                Result = Result.Where(f => f.FullName.Contains(LastName));

            if (!string.IsNullOrEmpty(PhoneNumber))
                Result = Result.Where(f => f.PhoneNumber.Contains(PhoneNumber));

            if (!string.IsNullOrEmpty(Email))
                Result = Result.Where(f => f.Email.Contains(Email));

            StudentListDTO studentList = new StudentListDTO();
            int take = 5;
            int pageCount = (Result.Count() + take - 1) / take;
            int skip = (CurrentPage - 1) * take;

            studentList.Students = Result.OrderByDescending(f => f.FullName).Skip(skip).Take(take).ToList();
            studentList.CureentPage = CurrentPage;
            studentList.PageCount = pageCount;

            return studentList;
        }

        //Dropdown متد دریافت لیست سطوح دانشجویان برای استفاده در 
        public async Task<IEnumerable<SelectListItem>> GetStudentsLevelsAsync()
        {
            return await _Context.StudentsLevels.Select(l => new SelectListItem
            {
                Text = l.LevelName,
                Value = l.StudentLevelId.ToString()
            }).ToListAsync();
        }

        // متد شمارش تعداد دانشجویان ثبت شده
        public async Task<int> NumberofStudents()
        {
            return await _Context.Students.CountAsync();
        }

        // متد به‌روزرسانی اطلاعات دانشجو با استفاده از شماره دانشجویی
        public async Task<OperationResult<Student>> UpdateAsync(StudentOperationDTOs student, string gender)
        {
            var result = new OperationResult<Student>();

            try
            {
                await _Context.Students.Where(s => s.StudentNum == student.studentNum)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.PhoneNumber, student.PhoneNumber)
                        .SetProperty(p => p.FirstName, student.FirstName)
                        .SetProperty(p => p.LastName, student.LastName)
                        .SetProperty(p => p.DateOfBirth, student.DateOfBirth)
                        .SetProperty(p => p.Email, student.Email)
                        .SetProperty(p => p.StudentLevelId, student.StudentLevelId)
                        .SetProperty(p => p.Gender, gender)
                    );

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorDetails = $"خطا در ویرایش دانشجو: {ex.Message}";
            }

            return result;
        }

    }
}

