using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class FavouriteRepository : Repository<Favourite>, IFavouriteRepository
        {
            public FavouriteRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
