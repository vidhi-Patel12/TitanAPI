using System.Text.RegularExpressions;

namespace Internal_Portal.Utilities
{
    public class FileNameHelper
    {
        private static readonly Regex InvalidChars = new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", RegexOptions.Compiled);

        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;
            var withoutInvalid = InvalidChars.Replace(fileName, "_");
            // Replace whitespace sequences with underscore
            withoutInvalid = Regex.Replace(withoutInvalid, @"\s+", "_");
            return withoutInvalid.Length > 200 ? withoutInvalid.Substring(withoutInvalid.Length - 200) : withoutInvalid;
        }
    }
}
