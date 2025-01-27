using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string Bio { get; set; }
        public string? PicUrl { get; set; }
        public Double? Rating { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
