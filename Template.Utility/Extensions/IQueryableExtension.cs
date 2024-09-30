using Microsoft.EntityFrameworkCore;
using Template.Utility.Dto;

namespace Template.Utility.Extensions;

public static class IQueryableExtension
{
    public static async Task<PagedResponse<TEntity>> GetPagedAsync<TEntity>(this IQueryable<TEntity> query, 
        int page, int pageSize) where TEntity : class
    {
        if (page < 1)
        {
            return new PagedResponse<TEntity>
            {
                TotalPage = 0,
                TotalItem = 0,
                Items = []
            };
        }

        var totalItem = await query.CountAsync();
        var totalPage = Convert.ToInt32(Math.Ceiling(totalItem / (double)pageSize));

        var skipCount = (page - 1) * pageSize;

        var response = new PagedResponse<TEntity>
        {
            TotalPage = totalPage,
            TotalItem = totalItem,
            Items = await query.Skip(skipCount)
                               .Take(pageSize)
                               .ToListAsync()
        };

        return response;
    }
}