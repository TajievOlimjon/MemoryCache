namespace WebApi
{
    public interface ICourseService
    {
        Task<List<Course>?> GetAllCourses();
        Task<Response<Course?>> GetCourseById(int courseId);
        Task<Response<Course>> AddNewCourse(Course course);
        Task<Response<Course>> UpdateCourse(Course course);
        Task<Response<Course>> DeleteCourse(int courseId);
    }
}
