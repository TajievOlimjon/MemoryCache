namespace WebApi
{
    public interface IStudentService
    {
        Task<PagedResponse<List<GetStudentDto>>> GetAllStudentsAsync(StudentFilter filter);
        Task<Response<GetStudentDto>> GetStudentByIdAsync(int studentId);
        Task<Response<string>> DeleteStudentAsync(int studentId);
        Task<Response<UpdateStudentDto>> UpdateStudentAsync(UpdateStudentDto model);
        Task<Response<AddStudentDto>> AddStudentAsync(AddStudentDto model);
    }
}

