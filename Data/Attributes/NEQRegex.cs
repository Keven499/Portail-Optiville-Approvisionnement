using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Portail_OptiVille.Data.Attributes
{
    public class NEQRegex : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // À MODIFIER, LE TROISIÈME CHIFFRE PEUT ÊTRE ENTRE 0 À 9 FINALEMENT ET PAS ENTRE 3 À 9
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (!Regex.IsMatch(stringValue, @"^\d{10}$"))
                {
                    return new ValidationResult("10 chiffres exigés", new[] { validationContext.MemberName });
                }

                // Check if the first two characters are 11, 22, 33, or 88
                if (!Regex.IsMatch(stringValue.Substring(0, 2), @"^(11|22|33|88)"))
                {
                    return new ValidationResult("Doit commencer par 11, 22, 33 ou 88", new[] { validationContext.MemberName });
                }

                // Check if the third character is 4, 5, 6, 7, 8, or 9
                if (!"456789".Contains(stringValue[2]))
                {
                    return new ValidationResult("Troisième caractère: 4, 5, 6, 7, 8 ou 9", new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
}
