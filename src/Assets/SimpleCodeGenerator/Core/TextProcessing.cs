using System.Text.RegularExpressions;

namespace SimpleCodeGenerator.Core
{
    public static class TextProcessing
    {
        public static string SanitizeStringForVariableName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            string variableName = Regex.Replace(input, "[^0-9A-Za-z_]", string.Empty);

            if (int.TryParse(variableName[0].ToString(), out int _))
            {
                variableName = $"_{variableName}";
            }

            return variableName;
        }

        public static string EscapeSummaryText(string text)
        {
            return $"<![CDATA[{text}]]>";
        }
    }
}