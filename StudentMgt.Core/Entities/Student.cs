using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMgt.Core.Entities
{
    public class Students
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string SchoolName { get; set; }
        public string YearOfGraduation { get; set; }
    }
}
