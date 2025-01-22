using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository ApplicationUsers { get; }
        ICartRepository Carts { get; }
        ICategoryRepository Categories { get; }
        ICouponRepository Coupons { get; }
        ICourseRepository Courses { get; }
        IDegreeRepository Degrees { get; }
        IEnrollmentRepository Enrollments { get; }
        IFavouriteRepository Favourites { get; }
        IInstructorRepository Instructors { get; }
        ILessonRepository Lessons { get; }
        IPaymentRepository Payments { get; }
        IQuizRepository Quizs { get; }
        IReviewRepository Reviews { get; }
        ISectionRepository Sections { get; }
        IStudentCategoriesRepository StudentCategories { get; }
        IStudentCouponsRepository StudentCoupons { get; }

        public void Commit();

    }
}
