using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RazorEngine;

namespace Portalia.Extentions
{
    public static class StringExtentions
    {
        public static string RemoveSpace(this string input)
        {
            return input.Replace(" ", string.Empty);
        }

        public static string RenderRazorViewToString(string viewPath, object model)
        {
            var viewSource = File.ReadAllText(viewPath);
            string renderedText = Razor.Parse(viewSource, model);
            return renderedText;
        }

        public static bool ConvertToBool(this string input)
        {
            try
            {
                return bool.Parse(input);
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}