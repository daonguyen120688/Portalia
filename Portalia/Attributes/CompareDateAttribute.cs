using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;
using System;

namespace Portalia.Attributes
{
    public class CompareDateAttribute : ValidationAttribute, IClientValidatable
    {
        public string RefProperty { get; set; }
        public ComparisonOperator Operator { get; set; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var clientValidationRule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(this.ErrorMessage),
                ValidationType = "comparedate"
            };

            clientValidationRule.ValidationParameters.Add("refproperty", RefProperty);
            clientValidationRule.ValidationParameters.Add("operator", (int)Operator);

            return new[] { clientValidationRule };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectInstance.GetType().GetProperty(RefProperty);

            if (property == null)
                return ValidationResult.Success;

            object valOfProp = property.GetValue(validationContext.ObjectInstance);

            if(valOfProp==null || value==null)
                return ValidationResult.Success;

            DateTime? valOfPropByDate = (DateTime?)valOfProp;
            DateTime? valByDate = (DateTime?)value;
            bool IsFailure = false;

            switch(Operator)
            {
                case ComparisonOperator.GreaterThan:
                    IsFailure = valByDate.Value > valOfPropByDate.Value;
                    break;
                case ComparisonOperator.LessThan:
                    IsFailure= valByDate.Value < valOfPropByDate.Value;
                    break;
                case ComparisonOperator.Equal:
                default:
                    IsFailure= valByDate.Value == valOfPropByDate.Value; break;
            }

            if(!IsFailure)
                return new ValidationResult(this.ErrorMessage);

            return ValidationResult.Success;
        }
    }

    public enum ComparisonOperator
    {
        GreaterThan=1,
        LessThan=2,
        Equal=3
    }
}