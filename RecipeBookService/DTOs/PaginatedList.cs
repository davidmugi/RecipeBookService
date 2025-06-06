namespace RecipeBookService.DTOs;

public class PaginatedList<T>
{
    public PaginatedList(List<T> items, int pageIndex, int totalPages, int totalSize)
    {
        Items = items;
        PageIndex = pageIndex;
        TotalPages = totalPages;
        TotalSize = totalSize;
    }

    public List<T> Items { get; set; }

    public int PageIndex { get; set; }

    public int TotalPages { get; set; }

    public int TotalSize { get; set; }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;
}