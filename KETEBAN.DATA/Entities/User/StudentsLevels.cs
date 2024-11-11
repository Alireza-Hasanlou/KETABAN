using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Entities.User
{
    public class StudentsLevels
    {
        public int StudentLevelId { get; set; }
        public string LevelName { get; set; }
        public string Describetion { get; set; }



        #region Navigation
        public ICollection<Student>? Students { get; set; }
        #endregion
    }
}
