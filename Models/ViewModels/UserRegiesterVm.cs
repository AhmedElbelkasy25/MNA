using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UserRegiesterVm
    {
        public int Id { get; set; }
        [Required]
        [Length(5, 50)]
        public string UserName { get; set; }
        
        [Required]
        [Length(6, 50)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Length(6, 50)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "ConfirmPassword")]
        public string conPassword { get; set; }
        
    }
}
