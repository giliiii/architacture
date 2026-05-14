using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        [EmailAddress]
        public string EMail { get; set; }

        [Required,MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }
    }
}