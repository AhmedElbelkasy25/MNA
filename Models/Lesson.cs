using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public ICollection<Quiz> Quizs { get; set; } = new List<Quiz>();
        public ICollection<Degree> Degrees { get; set; } = new List<Degree>();
    }
}
