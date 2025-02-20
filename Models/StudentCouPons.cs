using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class StudentCouPons
    {
        public int Id { get; set; }
        public double? AmountOfDiscount { get; set; }
        public ApplicationUser Student { get; set; }
        public string ApplicationUserId { get; set; }
        public int CouponId { get; set; }
        public Coupon Coupon { get; set; }
    }
}
