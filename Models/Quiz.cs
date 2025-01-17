using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public Boolean CorrectAnswer { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
