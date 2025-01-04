using System.ComponentModel.DataAnnotations;

namespace ecommerce_final.Models
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

    }
}
