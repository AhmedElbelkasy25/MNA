using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AddToRoleVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
