using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TinifyConsole
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidateRequiredParametersAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (!(value is Program options))
            {
                return ValidationResult.Success;
            }

            if (string.IsNullOrEmpty(options.ApiKey))
            {
                return new ValidationResult("missing --api-key");
            }
            
            if (string.IsNullOrEmpty(options.FilePattern))
            {
                return new ValidationResult("missing --file-pattern");
            }
        
            return ValidationResult.Success;
        }
    }
}