using System.ComponentModel.DataAnnotations.Schema;

namespace TL_Clothing.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int ProductQuantity { get; set; }

        [Column("CartId")]
        public int CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Column("ProductId")]
        public string ProductId { get; set; }
        public int Total { get; set; }
    }
}
