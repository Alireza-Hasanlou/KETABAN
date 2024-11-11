using KETABAN.CORE.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETABAN.CORE.Services.Interfaces
{
    public interface ISettingsManagementServices
    {
        Task<OperationResult<int>> CreateCategory(string Genre);
        Task<OperationResult<int>> DeleteCategory(int CategoryId);
        Task<OperationResult<int>> CreateLanguage(string Languages);
        Task<OperationResult<int>> DeleteLanguage(int LanguageId);

    }
}
