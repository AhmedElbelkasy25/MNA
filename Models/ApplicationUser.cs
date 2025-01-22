using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ImgUrl { get; set; }
        public int? NumOfCourses { get; set; }
        public Boolean IsLocked { get; set; } = false;
        public ICollection<Degree> Degrees { get; set; } = new List<Degree>();
        public ICollection<StudentCouPons> StudentCouPons { get; set; } = new List<StudentCouPons>();
        public ICollection<StudentCategories> StudentCategories { get; set; } = new List<StudentCategories>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
