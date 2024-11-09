using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Portail_OptiVille.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NomPrenomAttribute : ValidationAttribute
    {
        private readonly string _fieldName;

        public NomPrenomAttribute(string fieldName)
        {
            _fieldName = fieldName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = value?.ToString();
            if (string.IsNullOrWhiteSpace(currentValue))
            {
                return new ValidationResult($"{_fieldName} requis");
            }

            if (Regex.IsMatch(currentValue, @"\d"))
            {
                return new ValidationResult($"Aucun chiffre permis");
            }

            if (Regex.IsMatch(currentValue, @"[^a-zA-Z'-,]"))
            {
                return new ValidationResult($"{_fieldName} ne doit contenir que des lettres et les caractères autorisés: ', - ,");
            }

            return ValidationResult.Success;
        }
    }
}
