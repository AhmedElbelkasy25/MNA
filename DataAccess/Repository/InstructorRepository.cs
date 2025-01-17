using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class InstructorRepository : Repository<Instructor>, IInstructorRepository
        {
            public InstructorRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
