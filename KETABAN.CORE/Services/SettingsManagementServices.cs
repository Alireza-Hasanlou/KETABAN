using KETABAN.CORE.DTOs;
using KETABAN.CORE.Services.Interfaces;
using KETEBAN.DATA.Context;
using KETEBAN.DATA.Entities.BookEN;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETABAN.CORE.Services
{
    public class SettingsManagementServices : ISettingsManagementServices
    {
        private readonly _KetabanContext _Context;
        public SettingsManagementServices(_KetabanContext Context)
        {
            _Context = Context;
        }
        public async Task<OperationResult<int>> CreateCategory(string Genre)
        {
            var result = new OperationResult<int>();
            if (string.IsNullOrEmpty(Genre))
            {
                result.IsSuccess = false;
                result.ErrorDetails = "لطفا یک دسته بندی وارد کنید";
                return result;
            }
            var category=_Context.Categories.FirstOrDefault(c=>c.Genre.Equals(Genre));
            if (category != null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "دسته بندی از قبل موجود است";
                return result;
            }

            try
            {
                Categories NewCategory = new Categories() { Genre = Genre };
                await _Context.Categories.AddAsync(NewCategory);
                await _Context.SaveChangesAsync();
                result.IsSuccess = true;
                return result;

            }
            catch (Exception ex)
            {

                result.ErrorDetails = $"خطا در افزودن دسته بندی: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult<int>> CreateLanguage(string Language)
        {
            var result = new OperationResult<int>();
            if (string.IsNullOrEmpty(Language))
            {
                result.IsSuccess = false;
                result.ErrorDetails = "لطفا یک زبان وارد کنید";
                return result;
            }
            var lang = _Context.languages.FirstOrDefault(l => l.Language.Equals(Language));
            if (lang != null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "زیان از قبل موجود است";
                return result;
            }

            try
            {
                BookLanguage Newlanguage = new BookLanguage() { Language = Language };
                await _Context.languages.AddAsync(Newlanguage);
                await _Context.SaveChangesAsync();
                result.IsSuccess = true;
                return result;

            }
            catch (Exception ex)
            {

                result.ErrorDetails = $"خطا در افزودن زیان: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult<int>> DeleteCategory(int CategoryId)
        {
            var result = new OperationResult<int>();
            if (CategoryId <= 0)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "شناسه دسته بندی نمیتواند خالی باشد";
                return result;
            }

            var Category = _Context.Categories.Find(CategoryId);
            if (Category == null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "دسته بندی با شناسه ارسال شده یافت نشد";
                return result;
            }

            try
            {
                _Context.Categories.Remove(Category);
                await _Context.SaveChangesAsync();
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {

                result.ErrorDetails = $"خطا در حذف دسته بندی: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult<int>> DeleteLanguage(int LanguageId)
        {

            var result = new OperationResult<int>();
            if (LanguageId <= 0)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "شناسه زیان نمیتواند خالی باشد";
                return result;
            }

            var Lang = _Context.languages.Find(LanguageId);
            if (Lang == null)
            {
                result.IsSuccess = false;
                result.ErrorDetails = "زیان با شناسه ارسال شده یافت نشد";
                return result;
            }

            try
            {
                _Context.languages.Remove(Lang);
                await _Context.SaveChangesAsync();
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {

                result.ErrorDetails = $"خطا در حذف زیان: {ex.Message}";
                return result;
            }
        }
    }
}
