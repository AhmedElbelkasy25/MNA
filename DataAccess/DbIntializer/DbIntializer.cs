using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DbIntializer
{
    public class DbIntializer : IDbIntializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public DbIntializer(UserManager<ApplicationUser> userManager ,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._dbContext = dbContext;
        }
        public void Intial()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                    _dbContext.Database.Migrate();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }


            if (!_roleManager.Roles.Any())
            {
                _roleManager.CreateAsync(new("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new("Instructor")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new("Student")).GetAwaiter().GetResult();


                _userManager.CreateAsync(new()
                {
                    UserName="Admin123",
                    Name="Admin",
                    Email="Admin123@gmail.com"
                },"@Admin123").GetAwaiter().GetResult();

                _userManager.AddToRoleAsync(
                    _userManager.FindByEmailAsync("Admin123@gmail.com")
                    .GetAwaiter().GetResult(), "Admin");

            }
        }
    }
}
