namespace MyApp.Common.Responses;

public class PagedResponse<T> : BaseResponse<IEnumerable<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }

    public static PagedResponse<T> Create(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        return new PagedResponse<T>
        {
            Success = true,
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            Message = "Success"
        };
    }
}