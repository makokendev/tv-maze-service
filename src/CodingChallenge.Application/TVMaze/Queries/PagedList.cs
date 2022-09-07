namespace CodingChallenge.Application.TVMaze.Queries;

public class PagedList<T>
{
    public string? PaginationToken { get; private set; }
    public int PageSize { get; private set; }
    public List<T>? Items { get; private set; }

    public PagedList(List<T> items, int pageSize, string? paginationToken = null)
    {
        PageSize = pageSize;
        PaginationToken = paginationToken;
        Items = new List<T>(items);
    }
}