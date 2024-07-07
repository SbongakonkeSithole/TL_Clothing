using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TL_Clothing.Models
{
    public class Customer:IdentityUser
    {
        [Display(Name = " Name")]
        public string? Name { get; set; }
        [Display(Name = "Surname")]
        public string? Surname { get; set; }
        [Display(Name = "Learner Email")]
        [Required]
        public bool TermsConfirmed { get; set; }
    }
}
