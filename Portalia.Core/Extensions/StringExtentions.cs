using System.Linq;
using System.Text.RegularExpressions;

namespace Portalia.Core.Extensions
{
    public static class StringExtentions
    {
        public static string RemoveSpace(this string input)
        {
            return input.Replace(" ", string.Empty);
        }

        public static bool HasContainedNumber(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var hasContainedNumberpattern = @"^(?=.*\d)";

            return Regex.IsMatch(input, hasContainedNumberpattern);
        }

        public static bool HasContainedLowerCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var hasContainedLowerCasePattern = @"(?=.*[a-z])";

            return Regex.IsMatch(input, hasContainedLowerCasePattern);
        }

        public static bool HasContainedUpperCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var hasContainedUpperCasePattern = @"(?=.*[A-Z])";

            return Regex.IsMatch(input, hasContainedUpperCasePattern);
        }

        public static bool HasContainedSpecialCharacter(this string input)
        {
            return !string.IsNullOrEmpty(input) && input.Any(ch => !char.IsLetterOrDigit(ch));
        }
    }
}