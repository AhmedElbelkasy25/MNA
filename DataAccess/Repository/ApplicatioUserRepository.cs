using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class ApplicatioUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicatioUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}