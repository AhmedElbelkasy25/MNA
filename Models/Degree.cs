using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Degree
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public int Marks { get; set; }
        public DateOnly Date { get; set; }
        public Course Course { get; set; }
        public Lesson Lesson { get; set; }
        public ApplicationUser Student { get; set; }
    }
}
