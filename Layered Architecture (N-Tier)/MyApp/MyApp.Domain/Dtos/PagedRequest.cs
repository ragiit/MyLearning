namespace MyApp.Domain.Dtos;

public class PagedRequest
{
    public int PageNumber { get; set; } = 1;   // default page 1
    public int PageSize { get; set; } = 10;    // default size 10
}