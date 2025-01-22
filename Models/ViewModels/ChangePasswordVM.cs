using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ChangePasswordVM
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [Length(6, 50)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassord { get; set; }
    }
}
