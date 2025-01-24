using Template.Database.Models;

namespace Template.Core.Repositories.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<bool> CodeExistsAsync(string code, Guid? id = null);
    }
}