using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public ICartRepository Carts { get; private set; }

        public ICategoryRepository Categories { get; private set; }

        public ICouponRepository Coupons { get; private set; }

        public ICourseRepository Courses { get; private set; }

        public IDegreeRepository Degrees {  get; private set; }

        public IEnrollmentRepository Enrollments { get; private set; }

        public IFavouriteRepository Favourites { get; private set; }

        public IInstructorRepository Instructors { get; private set; }

        public ILessonRepository Lessons { get; private set; }

        public IPaymentRepository Payments { get; private set; }

        public IQuizRepository Quizs { get; private set; }

        public IReviewRepository Reviews { get; private set; }

        public ISectionRepository Sections { get; private set; }
        public IQuestionRepository Questions { get; private set; }

        public IStudentCategoriesRepository StudentCategories { get; private set; }

        public IStudentCouponsRepository StudentCoupons { get; private set; }
        public IInstructorApplicationRepository InstructorApplications { get; private set; }

        public UnitOfWork(ApplicationDbContext dbContext ) {
            this._dbContext = dbContext;
            ApplicationUsers = new ApplicationUserRepository(_dbContext);
            Carts = new CartRepository(_dbContext);
            Categories = new CategoryRepository(_dbContext);
            Coupons = new CouponRepository(_dbContext);
            Courses = new CourseRepository(_dbContext);
            Degrees = new DegreeRepository(_dbContext);
            Enrollments = new EnrollmentRepository(_dbContext);
            Favourites = new FavouriteRepository(_dbContext);
            Instructors = new InstructorRepository(_dbContext);
            Lessons = new LessonRepository(_dbContext);
            Payments = new PaymentRepository(_dbContext);
            Quizs = new QuizRepository(_dbContext);
            Reviews = new ReviewRepository(_dbContext);
            Sections = new SectionRepository(_dbContext);
            Questions = new QuestionRepository(_dbContext);
            StudentCategories = new StudentCategoriesRepository(_dbContext);
            StudentCoupons = new StudentCouponsRepository(_dbContext);
            InstructorApplications = new InstructorApplicationRepository(_dbContext);
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void ExecuteRawSql(string sql)
        {
            _dbContext.Database.ExecuteSqlRaw(sql);
        }
    }
}
