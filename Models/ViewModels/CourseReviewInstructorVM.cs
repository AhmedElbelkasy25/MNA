using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class CourseReviewInstructorVM
    {
        public int Id { get; set; }
        public List<Course> Courses { get; set; }
        public List<Course> FeatureCourse { get; set; }
        public List<Instructor> Instructors { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
