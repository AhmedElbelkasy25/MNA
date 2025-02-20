using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int CourseId { get; set; }
        public DateOnly ExpireDate { get; set; }
        public ApplicationUser Student { get; set; }
        public Course Course { get; set; }
        public string? PaymentIntentId { get; set; }
    }
}