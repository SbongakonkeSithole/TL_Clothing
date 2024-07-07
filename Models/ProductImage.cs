using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TL_Clothing.Models
{
    public class ProductImage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ImageId { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; }
        [NotMapped]
        public Product Product { get; set; }
    }
}
