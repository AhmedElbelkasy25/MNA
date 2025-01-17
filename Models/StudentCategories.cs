using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class StudentCategories
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ApplicationUser Student { get; set; }
    }
}
