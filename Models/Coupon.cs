using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public DateOnly ExpireDate { get; set; }
        public double Discount { get; set; }
        public string Serial { get; set; }
        public String CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<StudentCouPons> Coupons { get; set; } = new List<StudentCouPons>();
    }
}
