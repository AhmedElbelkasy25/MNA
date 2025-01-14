using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? ImgUrl { get; set; }
    }
    public class Category 
    {
        public int Id {  get; set; }
        public string Name { get; set; }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
