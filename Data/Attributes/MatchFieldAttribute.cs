using System.ComponentModel.DataAnnotations;

namespace Portail_OptiVille.Data.Attributes
{
    public class MatchFieldAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public MatchFieldAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = value?.ToString();
            var comparisonValue = validationContext.ObjectType.GetProperty(_comparisonProperty)
                ?.GetValue(validationContext.ObjectInstance)?.ToString();

            if (string.IsNullOrWhiteSpace(currentValue))
            {
                return new ValidationResult("Confirmation requise", new[] { validationContext.MemberName });
            }

            if (!currentValue.Equals(comparisonValue))
            {
                return new ValidationResult(ErrorMessage ?? "Les champs ne correspondent pas.", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
