using Newtonsoft.Json;
using Template.Utility.Dto;

namespace Template.Service.Dto;

public class PagedDto<TEntity> where TEntity : class
{
    [JsonProperty("page")]
    public int Page { get; set; }

    [JsonProperty("pageSize")]
    public int PageSize { get; set; }

    [JsonProperty("totalPage")]
    public int TotalPage { get; set; }

    [JsonProperty("totalItem")]
    public int TotalItem { get; set; }

    [JsonProperty("items")]
    public IEnumerable<TEntity> Items { get; set; } = [];
}