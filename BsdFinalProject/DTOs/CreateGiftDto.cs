using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace   BsdFinalProject.DTOs
{
    public class CreateGiftDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [Required,DefaultValue(30),Range(10,100)]
        
        public int Cost { get; set; }

        [MaxLength(300)]
        public string Picture { get; set; }

        //[Required]
        [ForeignKey("category")]
        public int CategoryId { get; set; }

        //[Required]
        [ForeignKey("donor")]
        public int DonorId { get; set; }
    }
}