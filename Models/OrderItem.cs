using static NuGet.Packaging.PackagingConstants;
using System.ComponentModel.DataAnnotations.Schema;

namespace TL_Clothing.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Column("ProductId")]
        public string ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public int ProductQuantity { get; set; }
        public int Total { get; set; }

        [Column("OrderId")]
        public string OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
