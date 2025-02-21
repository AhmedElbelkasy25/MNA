using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    internal class InstructorApplicationRepository : Repository<InstructorApplication>, IInstructorApplicationRepository
    {
        public InstructorApplicationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
