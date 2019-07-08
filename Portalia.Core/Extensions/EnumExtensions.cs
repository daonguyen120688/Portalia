using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Portalia.Core.Enum;

namespace Portalia.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this WorkContractStatusEnum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())?
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
        }

        public static Dictionary<int, string> GetWorkContractStatuses()
        {
            var workContractStatuses = new Dictionary<int, string>();

            // Get display name of each enum value
            foreach (WorkContractStatusEnum workContractStatusEnum in System.Enum.GetValues(typeof(WorkContractStatusEnum)))
            {
                string displayName = workContractStatusEnum.GetDisplayName();
                if (!workContractStatuses.Any(x => x.Value == displayName))
                {
                    workContractStatuses.Add((int)workContractStatusEnum, displayName);
                }
            }

            return workContractStatuses;
        }

        public static List<int> GetWorkContractValidatedStatuses()
        {
            return new List<int>
            {
                (int)WorkContractStatusEnum.Validated
            };
        }
    }
}
