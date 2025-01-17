using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class StudentCategoriesRepository : Repository<StudentCategories>, IStudentCategoriesRepository
        {
            public StudentCategoriesRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
