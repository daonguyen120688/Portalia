using System;
using System.ComponentModel.DataAnnotations;
using Portalia.Core.Extensions;

namespace Portalia.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password is required");
            }

            var isValid = password.HasContainedNumber() && password.HasContainedLowerCase() &&
                          password.HasContainedUpperCase() && password.HasContainedSpecialCharacter();

            return isValid ? ValidationResult.Success : new ValidationResult("Password is in wrong format");
        }
    }
}