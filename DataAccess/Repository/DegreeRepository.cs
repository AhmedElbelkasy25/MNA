using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class DegreeRepository : Repository<Degree>, IDegreeRepository
        {
            public DegreeRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
