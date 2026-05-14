using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress, MaxLength(50)]
        public string EMail { get; set; }

        [Required, MaxLength(100)]
        public string Password { get; set; }
    }
}