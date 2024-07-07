using TL_Clothing.Models;

namespace TL_Clothing.ViewModels
{
    public class OrderVm
    {
        public Cart Cart { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public bool Deliver { get; set; }
    }
}
