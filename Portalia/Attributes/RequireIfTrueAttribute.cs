using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;

namespace Portalia.Attributes
{
    public class RequireIfTrueAttribute : ValidationAttribute,IClientValidatable
    {
        public string FlagProperty { get; set; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(this.ErrorMessage),
                ValidationType = "requireiftrue"
            };

            clientValidationRule.ValidationParameters.Add("flagproperty", FlagProperty);

            return new[] { clientValidationRule };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectInstance.GetType().GetProperty(FlagProperty);

            if (property == null)
                return ValidationResult.Success;

            object valOfProp = property.GetValue(validationContext.ObjectInstance);

            if(valOfProp != null && (bool)valOfProp && (value==null || value.ToString()==string.Empty))
                return new ValidationResult(this.ErrorMessage);

            return ValidationResult.Success;
        }
    }
}