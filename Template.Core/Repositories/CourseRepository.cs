using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Database;
using Template.Database.Models;

namespace Template.Core.Repositories
{
    public class CourseRepository(DatabaseContext dbContext) : GenericRepository<Course>(dbContext), ICourseRepository
    {
        public async Task<bool> CodeExistsAsync(string code, Guid? id = null)
        {
            var query = _dbSet.Where(x => x.Code.Equals(code)).AsNoTracking();

            if (id.HasValue)
            {
                query = query.Where(x => x.Id != id.Value);
            }

            return await query.AnyAsync();
        }
    }
}