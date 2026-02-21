namespace Peneluc.Concurseiro.Web.Backend.Infrastructure.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; }
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

