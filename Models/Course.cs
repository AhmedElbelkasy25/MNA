using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImgUrl { get; set; }
        public DateOnly Date { get; set; }     
        public int InstructorId { get; set; }
        public int CategoryId { get; set; }
        public Instructor Instructor { get; set; }
        public Category Category { get; set; }
        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public ICollection<Review> REviews { get; set; } = new List<Review>();
        public ICollection<Degree> Degrees { get; set; } = new List<Degree>();

    }
}
