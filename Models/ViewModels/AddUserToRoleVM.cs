using Microsoft.AspNetCore.Identity;

namespace Models.ViewModels
{
    public class AddUserToRoleVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role {  get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
