namespace Peneluc.Concurseiro.Web.ViewModels;

public class PagedResultViewModel<T>
{
    public List<T> Data { get; set; }
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
