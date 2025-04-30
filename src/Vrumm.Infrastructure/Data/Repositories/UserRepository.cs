using Vrumm.Domain.Entities;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;

namespace Vrumm.Infrastructure.Data.Repositories;
public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(VrummDbContext context) : base(context)
    {
    }
}