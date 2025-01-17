using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
        {
            public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
            }
        }





    
}
