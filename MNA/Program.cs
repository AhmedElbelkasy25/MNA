using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

using Microsoft.AspNetCore.Identity.UI.Services;
using MNA.Utility;
using DataAccess.DbIntializer;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllersWithViews();


// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});


// Add Identity with ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add repository services

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbIntializer, DbIntializer>();


//builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<ICartRepository, CartRepository>();
//builder.Services.AddScoped<ICouponRepository, CouponRepository>();
//builder.Services.AddScoped<ICourseRepository, CourseRepository>();
//builder.Services.AddScoped<IDegreeRepository, DegreeRepository>();
//builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
//builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
//builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
//builder.Services.AddScoped<ILessonRepository, LessonRepository>();
//builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
//builder.Services.AddScoped<IQuizRepository, QuizRepository>();
//builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
//builder.Services.AddScoped<ISectionRepository, SectionRepository>();
//builder.Services.AddScoped<IStudentCategoriesRepository, StudentCategoriesRepository>();
//builder.Services.AddScoped<IStudentCouponsRepository, StudentCouponsRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

var scope = app.Services.CreateScope();
var service = scope.ServiceProvider.GetService<IDbIntializer>();
service.Intial();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Student}/{controller=Home}/{action=Index}/{id?}");

app.Run();