using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TL_Clothing.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("OrderId")]
        public string Id { get; set; }
      
        public string UserId { get; set; }
        //change the user
        [ForeignKey(nameof(UserId))]
        public Customer User { get; set; }

        public string OrderNumber { get; set; }

        public string OrderStatus { get; set; }

        public int TotalPrice { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public bool Deliver {  get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
