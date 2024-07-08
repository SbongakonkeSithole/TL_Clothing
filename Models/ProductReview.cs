using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TL_Clothing.Models
{
    public class ProductReview
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ReviewId { get; set; }
        public string Review {  get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; }
        [ForeignKey("Customer")]
        public string UserId { get; set; }
        public Customer User { get; set; }
        public int Rating { get; set; }
    }
}
