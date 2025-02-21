using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int CourseId { get; set; }
        public double Amount { get; set; }
        public DateOnly Date { get; set; }
        public ApplicationUser Student { get; set; }
        public Course Course { get; set; }
    }
}
