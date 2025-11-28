using StorageService.Application.Repositories;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;

namespace StorageService.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IRepository<User>
    {
        public UserRepository(StorageDbContext db) : base(db)
        {
        }
    }
}

