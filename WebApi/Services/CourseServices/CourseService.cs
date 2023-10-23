using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WebApi.Services.CourseServices
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public CourseService(ApplicationDbContext dbContext,IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public async Task<Response<Course>> AddNewCourse(Course course)
        {
            await _dbContext.Courses.AddAsync(course);
            var res = await _dbContext.SaveChangesAsync();

            return res == 0 ? new Response<Course>(System.Net.HttpStatusCode.InternalServerError, "Course not added!")
                : new Response<Course>(System.Net.HttpStatusCode.OK, "Course added");
        }

        public async Task<Response<Course>> DeleteCourse(int courseId)
        {
            var course = await _dbContext.Courses.FirstOrDefaultAsync(x=>x.Id==courseId);
            if (course == null) return new Response<Course>(System.Net.HttpStatusCode.NotFound, "Course not found !");

            _dbContext.Courses.Remove(course);
            _memoryCache.Remove(courseId);
            var res = await _dbContext.SaveChangesAsync();

            return res == 0 ? new Response<Course>(System.Net.HttpStatusCode.InternalServerError, "Course not deleted!")
                : new Response<Course>(System.Net.HttpStatusCode.OK, "Course deleted");
        }

        public async Task<List<Course>?> GetAllCourses()
        {
            var key = "courses";

            var courses = await _memoryCache.GetOrCreateAsync(
                key,
                async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    return await _dbContext.Courses.ToListAsync();
                });

            return courses;
        }

        public async Task<Response<Course?>> GetCourseById(int courseId)
        {
            var key = courseId.ToString();

            var course = await _memoryCache.GetOrCreateAsync(
                key,
                async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                    return await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == courseId);
                });

            return course == null? new Response<Course?>(System.Net.HttpStatusCode.NotFound,"Course not found !")
            : new Response<Course?>(System.Net.HttpStatusCode.NotFound, "Course not found !",course);
        }

        public async Task<Response<Course>> UpdateCourse(Course model)
        {
            var course = await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (course == null) return new Response<Course>(System.Net.HttpStatusCode.NotFound, "Course not found !");

            course.Name = model.Name;

            var res = await _dbContext.SaveChangesAsync();

            return res == 0 ? new Response<Course>(System.Net.HttpStatusCode.InternalServerError, "Course not deleted!")
                : new Response<Course>(System.Net.HttpStatusCode.OK, "Course deleted");
        }
    }
}
