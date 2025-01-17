﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Section
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
    }
}