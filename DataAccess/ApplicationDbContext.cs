using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUser> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<StudentCategories> StudentCategories { get; set; }
        public DbSet<StudentCouPons> StudentCouPons { get; set; }
        
        
        


        

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Review>()
                .ToTable(tb => tb.HasTrigger("triggerUpdateCourseRating"));

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
            });
        }

      
    }
}
