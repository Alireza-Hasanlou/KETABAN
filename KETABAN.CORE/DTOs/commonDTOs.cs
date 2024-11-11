using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETABAN.CORE.DTOs
{
 public class OperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorDetails { get; set; }
        public T Data { get; set; }
    }
}
