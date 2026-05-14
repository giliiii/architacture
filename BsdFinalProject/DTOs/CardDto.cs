using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BsdFinalProject.DTOs
{
    public class CardDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [ForeignKey("user")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("gift")]
        public int GiftId { get; set; }

        public DateTime BuingDate { get; set; }
    }

    public class GroupedCardDto
    {
        [Required]
        [ForeignKey("gift")]
        public int GiftId { get; set; }
        public int Count { get; set; }
    }

    public class TopPurchasedDto
    {
        [Required]
        public int GiftId { get; set; }
        public int Count { get; set; }
    }

    public class CardWithBuyerDto
    {
        [Required]
        public int CardId { get; set; }
        [Required]
        [ForeignKey("gift")]
        public int GiftId { get; set; }
        public string? GiftName { get; set; }
        public int BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public DateTime BuingDate { get; set; }
    }
}