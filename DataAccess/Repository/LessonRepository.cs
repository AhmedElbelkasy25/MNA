using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
        {
            public LessonRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
