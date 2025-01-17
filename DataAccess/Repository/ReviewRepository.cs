using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
        {
            public ReviewRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
