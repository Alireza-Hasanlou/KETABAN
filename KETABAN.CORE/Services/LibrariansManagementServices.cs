using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Context;
using KETEBAN.DATA.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETABAN.CORE.Services
{

    public class LibrariansManagementServices : ILibrariansManagementServices
    {
        private readonly UserManager<Librarian> _UserManager;

        private readonly _KetabanContext _context;
        public LibrariansManagementServices(UserManager<Librarian> userManager, _KetabanContext context)
        {
            _UserManager = userManager;
            _context = context;
        }
        //`AddLibrarianDTO` متد برای ایجاد کاربر جدید (کتابدار) بر اساس 
        public async Task<IdentityResult> CreateAsync(AddLibrarianDTO librarianDTO, string Gender)
        {
            // بررسی وجود کاربر با کد ملی وارد شده
            var existingUser = await _UserManager.FindByNameAsync(librarianDTO.NationalCode);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "NationalCodeExists", Description = "کد ملی از قبل موجود است" });
            }

            // ایجاد کاربر جدید
            var newUser = new Librarian
            {
                AvatarName = "pro.jpg",
                UserName = librarianDTO.NationalCode,
                FirstName = librarianDTO.FirstName,
                LastName = librarianDTO.LastName,
                NationalCode = librarianDTO.NationalCode,
                Email = librarianDTO.Email,
                DateOfBirth = librarianDTO.DateOfBirth,
                Gender = Gender,
                PhoneNumber = librarianDTO.PhoneNumber,
                DateofRegistration = DateTime.Now
            };

            // ذخیره کاربر در دیتابیس با استفاده از کد ملی به عنوان پسورد
            var result = await _UserManager.CreateAsync(newUser, librarianDTO.NationalCode);
            if (result.Succeeded)
            {
                return IdentityResult.Success;
            }

            // بازگردانی خطاها در صورت بروز مشکل
            return IdentityResult.Failed(result.Errors.ToArray());
        }

        // متد برای حذف کاربر بر اساس نام کاربری
        public async Task<IdentityResult> DeleteAsync(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return IdentityResult.Failed(new IdentityError { Code = "UserNameIsEmpty", Description = "نام کاربری نمی‌تواند خالی باشد" });
            }

            var user = await _UserManager.FindByNameAsync(UserName);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = "کاربر یافت نشد" });
            }

            var result = await _UserManager.DeleteAsync(user);
            return result;
        }

        // متد برای دریافت اطلاعات کتابدار بر اساس نام کاربری
        public async Task<LibrarianDto> GetLibraianByUserNameAsync(string UserName)
        {
            var result = await _UserManager.FindByNameAsync(UserName);

            return new LibrarianDto
            {
                UserName = result.UserName,
                Email = result.Email,
                Id = result.Id,
                DateOfBirth = result.DateOfBirth,
                FirstName = result.FirstName,
                LastName = result.LastName,
                NationalCode = result.NationalCode,
                PhoneNumber = result.PhoneNumber
            };
        }

        // متد برای دریافت لیست کتابداران با امکان جستجو و فیلتر
        public LiberaianListDTO GetLibrarians(string FirstName = "", string LastName = "", string PhoneNumber = "", string Email = "", int CurrentPage = 1)
        {
            IQueryable<LiberaianInfoDTOs> result = _UserManager.Users.Select(a => new LiberaianInfoDTOs
            {
                Email = a.Email,
                EmailComfrim = a.EmailConfirmed,
                FullName = a.FirstName + " " + a.LastName,
                PhoneNumber = a.PhoneNumber,
                UserName = a.UserName
                
                
            });

            // فیلتر کردن لیست بر اساس پارامترهای ورودی
            if (!string.IsNullOrEmpty(FirstName))
                result = result.Where(f => f.FullName.Contains(FirstName));
            if (!string.IsNullOrEmpty(LastName))
                result = result.Where(f => f.FullName.Contains(LastName));
            if (!string.IsNullOrEmpty(PhoneNumber))
                result = result.Where(f => f.PhoneNumber.Contains(PhoneNumber));
            if (!string.IsNullOrEmpty(Email))
                result = result.Where(f => f.Email.Contains(Email));

            // ایجاد و تنظیم `LiberaianListDTO`
            LiberaianListDTO liberaianList = new LiberaianListDTO();

            liberaianList.PageCount = (result.Count() / 10) + 1;
            liberaianList.CureentPage = CurrentPage;
            liberaianList.Librarians = result
                .OrderByDescending(f => f.FullName)
                .Skip((CurrentPage - 1) * 10)
                .Take(10)
                .ToList();

            return liberaianList;
        }

        // متد برای بروزرسانی اطلاعات کتابدار
        public async Task<IdentityResult> UpdateAsync(LibrarianDto librarianDtos)
        {
            if (librarianDtos == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "UserIsEmpty", Description = "اطلاعات کاربر نمی‌تواند خالی باشد" });
            }

            var user = await _UserManager.FindByIdAsync(librarianDtos.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = "کاربر یافت نشد" });
            }

            // بروزرسانی اطلاعات کاربر
            user.FirstName = librarianDtos.FirstName;
            user.LastName = librarianDtos.LastName;
            user.UserName = librarianDtos.UserName;
            user.PhoneNumber = librarianDtos.PhoneNumber;
            user.NationalCode = librarianDtos.NationalCode;
            user.Email = librarianDtos.Email;
            user.DateOfBirth = librarianDtos.DateOfBirth;

            var result = await _UserManager.UpdateAsync(user);



            return result.Succeeded ? IdentityResult.Success : IdentityResult.Failed(result.Errors.ToArray());
        }

        // متد برای دریافت شناسه کتابدار بر اساس ایمیل
        public async Task<string?> GetlibrarianIdbyEmailAsync(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return null;
            }

            var result = await _UserManager.FindByEmailAsync(Email);
            return result?.Id;
        }

        // متد برای دریافت اطلاعات کتابدار بر اساس شناسه
        public async Task<Librarian> GetLibraianByidAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return await _UserManager.FindByIdAsync(id);
        }

        public async Task<string> GetAvatarName(string UserName)
        {
           var librarian= await _UserManager.FindByNameAsync(UserName);
            return librarian.AvatarName;
        }
    }
}
