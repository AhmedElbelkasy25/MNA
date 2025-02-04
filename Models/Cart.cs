using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int CourseId { get; set; }
        public ApplicationUser Student { get; set; }
        public Course Course { get; set; }
        public int Count { get; set; }

    }
}
