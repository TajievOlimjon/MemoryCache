using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace WebApi
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<StudentService> _logger;
        public StudentService(
            ApplicationDbContext dbContext,
            IMemoryCache memoryCache,
            ILogger<StudentService> logger)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        public async Task<Response<AddStudentDto>> AddStudentAsync(AddStudentDto model)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s=>s.PhoneNumber==model.PhoneNumber);
            if (student != null) return new Response<AddStudentDto>(HttpStatusCode.BadRequest, "A student with this number already exists");

            var newStudent = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            await _dbContext.Students.AddAsync(newStudent);
            var result = await _dbContext.SaveChangesAsync();

            return result > 0 
                ? new Response<AddStudentDto>(HttpStatusCode.OK, "Student data successfully added !")
                : new Response<AddStudentDto>(HttpStatusCode.OK, "Student data not added !");
        }

        public async Task<Response<string>> DeleteStudentAsync(int studentId)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return new Response<string>(HttpStatusCode.NotFound, "Student not found !");

            _dbContext.Students.Remove(student);
            var result = await _dbContext.SaveChangesAsync();

            return result > 0
                ? new Response<string>(HttpStatusCode.OK, "Student data successfully deleted !")
                : new Response<string>(HttpStatusCode.OK, "Student data not deleted !");
        }

        public async Task<PagedResponse<List<GetStudentDto>>> GetAllStudentsAsync(StudentFilter filter)
        {
            filter = new StudentFilter(filter.PageNumber,filter.PageSize);
            var query = _dbContext.Students.OrderBy(x=>x.Id).AsQueryable();

            if (filter.FirstNameOrLastName != null)
            {
                query = query.Where(x=>x.FirstName.ToLower().Contains(filter.FirstNameOrLastName.ToLower()) ||
                                       x.LastName.ToLower().Contains(filter.FirstNameOrLastName.ToLower()));
            }
            if (filter.PhoneNumber != null)
            {
                query = query.Where(x => x.PhoneNumber == filter.PhoneNumber);
            }

            var allTotalRecord = await query.CountAsync();

            var students = await query.Select(student => new GetStudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber
            }).Skip(filter.PageSize * (filter.PageNumber - 1))
              .Take(filter.PageSize)
              .ToListAsync();

            return students.Count < 0 ? new PagedResponse<List<GetStudentDto>>(HttpStatusCode.NoContent, "No students yet", allTotalRecord, filter.PageNumber, filter.PageSize)
                : new PagedResponse<List<GetStudentDto>>(students,HttpStatusCode.OK,"All students", allTotalRecord, filter.PageNumber, filter.PageSize);
        }

        public async Task<Response<GetStudentDto>> GetStudentByIdAsync(int studentId)
        {
            Student? student;
            _memoryCache.TryGetValue(studentId,out student);
            if (student == null)
            {
                _logger.LogInformation("Student is not in cache");
                 student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
                if (student == null) return new Response<GetStudentDto>(HttpStatusCode.NotFound, "Student not found !");
                _memoryCache.Set(student.Id, student, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            else
            {
                _logger.LogInformation("Student found from cache");
            }
            var model = new GetStudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber
            };

            return new Response<GetStudentDto>(HttpStatusCode.OK, "Student data", model);
        }

        public async Task<Response<UpdateStudentDto>> UpdateStudentAsync(UpdateStudentDto model)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (student == null) return new Response<UpdateStudentDto>(HttpStatusCode.NotFound, "Student not found !");

            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Age = model.Age;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;

            var result = await _dbContext.SaveChangesAsync();

            return result > 0
                ? new Response<UpdateStudentDto>(HttpStatusCode.OK, "Student data successfully updated !")
                : new Response<UpdateStudentDto>(HttpStatusCode.OK, "Student data not updated !");

        }
    }
}
