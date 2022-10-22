using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentMgt.Core.DTOS;
using StudentMgt.Core.Responses;
using StudentMgt.Service.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace StudentMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost("AddStudent")]
        [SwaggerOperation(Summary = "Student Creation", Description = "Add a Student")]
        [SwaggerResponse(200, "Student Created", typeof(ResponseModel))]
        public async Task<IActionResult> AddStudent(StudentDTO studentDTO)
        {
            try
            {
                var response = await _studentService.AddStudent(studentDTO);

                if (response.State == 0)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex?.InnerException?.InnerException?.Message ?? ex?.InnerException?.Message ?? ex?.Message);
            }
        }

        [HttpPost("UpdateStudent")]
        [SwaggerOperation(Summary = "Student Update", Description = "Update a Student")]
        [SwaggerResponse(200, "Student Updated", typeof(ResponseModel))]
        public async Task<IActionResult> UpdateStudent(StudentDTO studentDTO)
        {
            try
            {
                var response = await _studentService.UpdateStudent(studentDTO);

                if (response.State == 0)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex?.InnerException?.InnerException?.Message ?? ex?.InnerException?.Message ?? ex?.Message);
            }
        }

        [HttpGet("ListOfAllStudents")]
        [SwaggerOperation(Summary = "List of all students", Description = "Returns a list of all students")]
        [SwaggerResponse(200, "Student List Loaded", typeof(ResponseModel))]
        public async Task<IActionResult> ListOfAllStudents()
        {
            try
            {
                var response = await _studentService.ListOfAllStudents();

                if (response.State == 0)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex?.InnerException?.InnerException?.Message ?? ex?.InnerException?.Message ?? ex?.Message);
            }
        }

        [HttpGet("GetStudentsCount")]
        [SwaggerOperation(Summary = "Count of students", Description = "Returns the number of students")]
        [SwaggerResponse(200, "Student Count Loaded", typeof(ResponseModel))]
        public async Task<IActionResult> GetStudentsCount()
        {
            try
            {
                var response = await _studentService.GetStudentsCount();

                if (response.State == 0)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex?.InnerException?.InnerException?.Message ?? ex?.InnerException?.Message ?? ex?.Message);
            }
        }
    }
}
