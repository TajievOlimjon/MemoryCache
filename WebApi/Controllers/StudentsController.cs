using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents([FromQuery]StudentFilter filter)
        {
            var students = await _studentService.GetAllStudentsAsync(filter);
            return StatusCode(students.StatusCode,students);
        }
        [HttpGet("GetStudentById")]
        public async Task<IActionResult> GetStudentById([FromQuery]int studentId)
        {
            var student = await _studentService.GetStudentByIdAsync(studentId);
            return StatusCode(student.StatusCode, student);
        }
        [HttpPost("AddStudent")]
        public async Task<IActionResult> AddStudent([FromBody]AddStudentDto model)
        {
            var response = await _studentService.AddStudentAsync(model);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPut("UpdateStudent")]
        public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentDto model)
        {
            var response = await _studentService.UpdateStudentAsync(model);
            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete("DeleteStudent")]
        public async Task<IActionResult> DeleteStudent([FromQuery] int studentId)
        {
            var response = await _studentService.DeleteStudentAsync(studentId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
