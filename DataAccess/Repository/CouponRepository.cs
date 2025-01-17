using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
        {
            public CouponRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
