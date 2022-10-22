using StudentMgt.Core.DTOS;
using StudentMgt.Core.Entities;
using StudentMgt.Core.Responses;
using StudentMgt.Core.ViewModels;
using StudentMgt.Repo.Implementation;
using StudentMgt.Repo.Infrastructure;
using StudentMgt.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StudentMgt.Repo.Infrastructure.Connectionfactory;

namespace StudentMgt.Service.Implemetation
{
    public class StudentService : IStudentService
    {
        private readonly GenericRepository<Students> _studentrepo;

        public StudentService(IConnectionFactory connectionFactory)
        {
            _studentrepo = new GenericRepository<Students>(connectionFactory);
        }
        public async Task<ResponseModel> AddStudent(StudentDTO studentDTO)
        {
            var ret = await _studentrepo.ExecuteAsyncSp(StoredProcedures.uspAddStudents, CommandType.StoredProcedure, new
            {
                StudentName = studentDTO.StudentName,
                SchoolName = studentDTO.SchoolName,
                YearOfGraduation = studentDTO.YearOfGraduation
            });
            if (ret < 1)
                return new ResponseModel { State = 0, Msg = "Error adding student to the db", Data = ret };

            return new ResponseModel { State = 1, Msg = "Student added successfully", Data = ret };
        }

        public async Task<ResponseModel> GetStudentsCount()
        {
            var ret = await _studentrepo.QueryFirstOrDefaultAsyncWithTypeSp<StudentViewModel>(StoredProcedures.uspGetAllStudentCount, CommandType.StoredProcedure);
            if (ret == null)
                return new ResponseModel { State = 0, Msg = "Student Count", Data = ret };
            return new ResponseModel { State = 1, Msg = "Student Count", Data = ret };
        }

        public async Task<ResponseModel> ListOfAllStudents()
        {
            var ret = await _studentrepo.QueryAsyncSp(StoredProcedures.uspGetAllStudents, CommandType.StoredProcedure);
            if (ret == null)
                return new ResponseModel { State = 0, Msg = "Student list is empty", Data = ret };
            return new ResponseModel { State = 1, Msg = "Student loaded successfully", Data = ret };
        }

        public async Task<ResponseModel> UpdateStudent(StudentDTO studentDTO)
        {
            var ret = await _studentrepo.ExecuteAsyncSp(StoredProcedures.uspUpdateStudents, CommandType.StoredProcedure, new
            {
                StudentId = studentDTO.StudentId,
                StudentName = studentDTO.StudentName,
                SchoolName = studentDTO.SchoolName,
                YearOfGraduation = studentDTO.YearOfGraduation
            });
            if (ret < 1)
                return new ResponseModel { State = 0, Msg = "Error updating student to the db", Data = ret };

            return new ResponseModel { State = 1, Msg = "Student updated successfully", Data = ret };
        }
    }
}
