using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ecommerce_final.Helpers.Validators
{
    public class CustomEmailValidator : ValidationAttribute
    {
        private const string _emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            string emailAddress = value.ToString();
            if (Regex.IsMatch(emailAddress, _emailPattern))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
