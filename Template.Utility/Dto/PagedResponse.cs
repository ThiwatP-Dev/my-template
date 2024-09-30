namespace Template.Utility.Dto;

public class PagedResponse<TEntity> where TEntity : class
{
    public int TotalPage { get; set; }

    public int TotalItem { get; set; }

    public List<TEntity> Items { get; set; } = [];
}