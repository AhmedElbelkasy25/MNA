using Microsoft.AspNetCore.Identity;

namespace MNA.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? ImgUrl { get; set; }
    }
}
