using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class CreatingCouponVM
    {
        public int Id { get; set; }
        [Required]
        public DateOnly ExpireDate { get; set; }
        [Required]
        [Range(0,100)]
        public double Discount { get; set; }
        public string? Serial { get; set; }
        [Required]
        public int CourseId { get; set; }
        
        public int? NumOfCoupon { get; set; } = 1;
    }
}
