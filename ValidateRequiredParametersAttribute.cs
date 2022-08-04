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

            if (options.Resize)
            {
                if (options.Width == 0 || options.Height == 0)
                {
                    return new ValidationResult("--resize requires --width and --height");
                }
            }
        
            return ValidationResult.Success;
        }
    }
}