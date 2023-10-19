using System.Net;

namespace WebApi
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public PagedResponse(T data,HttpStatusCode statusCode,string message, int totalRecords, int pageNumber, int pageSize) : base(statusCode,message,data)
        {
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public PagedResponse(HttpStatusCode statusCode, string message, int totalRecords, int pageNumber, int pageSize) : base(statusCode, message)
        {
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
