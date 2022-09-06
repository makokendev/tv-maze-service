namespace CodingChallenge.Application.TVMaze.Queries;

public class PagedList<T>
{
    public string? PaginationToken { get; private set; }
    //public int CurrentPage { get; private set; }
    //public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public List<T>? Items { get; private set; }
    // public bool HasPrevious => CurrentPage > 1;
    // public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> items, int pageSize, string? paginationToken = null)
    {
        //TotalCount = count;
        PageSize = pageSize;
        PaginationToken = paginationToken;
        //CurrentPage = pageNumber;
        //TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        items = new List<T>(items);
    }
    // public async static Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source,  int pageSize,string? paginationToken=null)
    // {
    //     //await Task.FromResult("");
    //     var count = source.Count();
    //     //var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    //     return new PagedList<T>(items, count, pageNumber,paginationToken);
    // }
}