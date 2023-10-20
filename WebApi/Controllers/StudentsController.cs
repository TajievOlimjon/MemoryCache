using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Services.CacheServices;

namespace WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ICacheService _cacheService;
        public StudentsController(IStudentService studentService, ICacheService cacheService)
        {
            _studentService = studentService;
            _cacheService = cacheService;
        }

        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents([FromQuery]StudentFilter filter)
        {
            var studentsInCache = _cacheService.GetData<List<Student>>(DefaultStudentCacheKey.Students);
            if (studentsInCache != null)
            {
                return StatusCode((int)HttpStatusCode.OK, studentsInCache);
            }
            var students = await _studentService.GetAllStudentsAsync(filter);
            if (students != null)
            {
                var expirationTime = DateTimeOffset.UtcNow.AddMinutes(2);
                _cacheService.SetData(DefaultStudentCacheKey.Students, students, expirationTime);

                return StatusCode((int)HttpStatusCode.OK, students);
            }
            return StatusCode((int)HttpStatusCode.NoContent, students);
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
