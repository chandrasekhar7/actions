using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Humanizer;

namespace Npa.Accounting.Common.Text
{
    public static class StringExtensions
    {
        private static readonly Regex digitsRegex = new Regex(@"\d", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex nonDigitsRegex = new Regex(@"\D", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex nonWordRegex = new Regex(@"[\s\W]+", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex whitespaceAndDashesRegex = new Regex(@"[\s\-]+", RegexOptions.Compiled | RegexOptions.Singleline);

        public static string ReplaceDigits(this string text, string replacement) => digitsRegex.Replace(text ?? "", replacement);

        public static string Sluggify(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(nameof(str));
            }
            var slug = str.Humanize(LetterCasing.Title);
            slug = whitespaceAndDashesRegex.Replace(slug, "-").Trim('-').ToLower();
            return slug;
        }
        
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }

        public static string StripNonDigits(this string text) => nonDigitsRegex.Replace(text ?? "", "");

        public static string StripNonWord(this string str) => nonWordRegex.Replace(str ?? "", "");
    }
}