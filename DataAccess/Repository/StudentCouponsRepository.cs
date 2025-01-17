using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class StudentCouponsRepository : Repository<StudentCouPons>, IStudentCouponsRepository
        {
            public StudentCouponsRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
