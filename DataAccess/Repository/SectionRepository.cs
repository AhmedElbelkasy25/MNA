using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class SectionRepository : Repository<Section>, ISectionRepository
        {
            public SectionRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
