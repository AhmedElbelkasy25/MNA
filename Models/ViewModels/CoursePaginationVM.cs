using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class CoursePaginationVM
    {
        public int Id { get; set; }
        public int Pages { get; set; }
        public List<Course> Courses { get; set; }
        public int PageNumber { get; set; }
    }
}
