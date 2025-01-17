using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
        {
            public QuizRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
