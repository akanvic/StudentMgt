using StudentMgt.Core.DTOS;
using StudentMgt.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMgt.Service.Interface
{
    public interface IStudentService
    {
        Task<ResponseModel> AddStudent(StudentDTO studentDTO);
        Task<ResponseModel> UpdateStudent(StudentDTO studentDTO);
        Task<ResponseModel> ListOfAllStudents();
        Task<ResponseModel> GetStudentsCount();
    }
}
