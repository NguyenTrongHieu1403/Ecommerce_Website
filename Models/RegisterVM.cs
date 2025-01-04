using ecommerce_final.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ecommerce_final.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 50 characters.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string PasswordHash { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        // Thêm thông tin role
        public int RoleId { get; set; }
        public virtual ICollection<Otp> Otprequests { get; set; } = new List<Otp>();
    }
}
