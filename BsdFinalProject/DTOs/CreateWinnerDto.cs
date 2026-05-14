using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BsdFinalProject.DTOs
{
    public class CreateWinnerDto
    {
        [Required]
        [ForeignKey("user")]
        public int IdUser { get; set; }

        [Required]
        [ForeignKey("gift")]
        public int IdGift { get; set; }
    }
}