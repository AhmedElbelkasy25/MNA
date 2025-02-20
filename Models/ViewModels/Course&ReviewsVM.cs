using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class Course_ReviewsVM
    {
        public Course Course { get; set; }
        public List<Review> Reviews { get; set; }
        public int StdNums { get; set; }
        public int LsnNums { get; set; }
    }
}
