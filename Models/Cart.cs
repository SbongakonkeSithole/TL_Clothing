using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TL_Clothing.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public Customer User { get; set; }

        public ICollection<CartItem> CartItem { get; set; }
        public int Subtotal {  get; set; }
        public int Total { get; set; }
    }
}
