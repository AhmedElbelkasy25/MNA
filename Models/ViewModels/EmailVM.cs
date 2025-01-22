using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class EmailVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "UserName Or Email")]
        public string Account { get; set; }
        
    }
}
