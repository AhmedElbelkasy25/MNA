using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
        {
            public EnrollmentRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
