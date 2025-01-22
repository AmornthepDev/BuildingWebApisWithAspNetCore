using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MyBGList.Attributes
{
    public class LettersOnlyValidatorAttribute : ValidationAttribute
    {
        public bool UseRegex { get; set; } = false;
        public LettersOnlyValidatorAttribute() : base("Value must contain only letters (no spaces, digits, or other chars)") { }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var strValue = value as string;
            if (!string.IsNullOrEmpty(strValue) 
                &&  
                ((UseRegex && Regex.IsMatch(strValue, @"^[a-zA-Z]+$")) // Using Regular Expression 
                || 
                strValue.All(Char.IsLetter) // Using string manipulation methods
                ))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
