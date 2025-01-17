using Microsoft.EntityFrameworkCore;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Models;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
namespace MNA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            //add connections
            builder.Services.AddDbContext<ApplicationDbContext>(
               option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
               );

            // Add Identity services.
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            //Add connection 

            
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IDegreeRepository, DegreeRepository>();
            builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
            builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
            builder.Services.AddScoped<ILessonRepository, LessonRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<ISectionRepository, SectionRepository>();
            builder.Services.AddScoped<IStudentCategoriesRepository, StudentCategoriesRepository>();
            builder.Services.AddScoped<IStudentCouponsRepository, StudentCouponsRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, IApplicationUserRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.MapRazorPages();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
