using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class InstructorApplication
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Bio { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}
