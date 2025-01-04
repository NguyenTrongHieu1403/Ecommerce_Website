using ecommerce_final.Helpers.Validators;
using System.ComponentModel.DataAnnotations;

namespace ecommerce_final.Models
{
    public class LoginVM
    {
        [Key]
        [Required(ErrorMessage = "Email/Username is required")]
        public string UsernameOrEmail { get; set; } 


        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
    }
}
