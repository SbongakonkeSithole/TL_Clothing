using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TL_Clothing.Models
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? CategoryImageUrl { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
