using System.ComponentModel.DataAnnotations;

namespace Portail_OptiVille.Data.Attributes 
{
    public class NothingSelectedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is Dictionary<string, bool> sousProduits)
            {
                if (!sousProduits.Values.Any(isSelected => isSelected))
                {
                    return new ValidationResult(ErrorMessage ?? "Aucune case n'a été cochée", new[] { validationContext.MemberName });
                }
            }

            return ValidationResult.Success;
        }
    }
}