using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BsdFinalProject.DTOs
{
    public class CreateBasketDto
    {
        [Required]
        [ForeignKey("user")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("gift")]
        public int GiftId { get; set; }
    }
}