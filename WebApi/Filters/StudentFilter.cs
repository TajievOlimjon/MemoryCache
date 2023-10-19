namespace WebApi
{
    public class StudentFilter
    {
        public string? FirstNameOrLastName { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public StudentFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public StudentFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 10 ? 10 : pageSize;
        }
    }
}



