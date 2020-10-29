using System.Collections.Generic;
using System.Text;

namespace Utility
{
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class StringHelper
    {
        /// <summary>
        ///     Get the string from the left
        /// </summary>
        /// <param name="param"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(string param, int length)
        {
            if (length > param.Length)
                length = param.Length;

            var result = param.Substring(0, length);
            return result;
        }

        /// <summary>
        ///     Get the string from the right
        /// </summary>
        /// <param name="param"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            var result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        /// <summary>
        ///     Get the string in the middle up to a certain amount
        /// </summary>
        /// <param name="param"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Mid(string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            var result = param.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        /// <summary>
        ///     Get the string in the middle till the end
        /// </summary>
        /// <param name="param"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string Mid(string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            var result = param.Substring(startIndex);
            //return the result of the operation
            return result;
        }

        /// <summary>
        ///     Get byte array with UTF8 Encoding
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StrToByteArray(string str)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        ///     Extract text between two strings.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        public static string ExtractBetween(string text, string startString, string endString)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var startIndex = text.IndexOf(startString) + startString.Length;
            var endIndex = text.IndexOf(endString, startIndex);
            var length = endIndex - startIndex;

            if (startIndex >= 0 && endIndex >= startIndex)
                return text.Substring(startIndex, length).Trim();
            return string.Empty;
        }

        /// <summary>
        ///     Extract text between two strings including the search strings.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        public static string ExtractBetweenIncludeSearchString(string text, string startString, string endString)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var startIndex = text.IndexOf(startString);
            var endIndex = text.IndexOf(endString, startIndex) + endString.Length;
            var length = endIndex - startIndex;

            if (startIndex >= 0 && endIndex >= startIndex)
                return text.Substring(startIndex, length).Trim();
            return string.Empty;
        }

        /// <summary>
        ///     Extract all occurrences of match between two strings.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractBetweenAll(string text, string startString, string endString)
        {
            do
            {
                var match = ExtractBetween(text, startString, endString);
                if (!string.IsNullOrWhiteSpace(match))
                {
                    yield return match;
                    text = text.Replace(ExtractBetweenIncludeSearchString(text, startString, endString), string.Empty);
                }
            } while (!string.IsNullOrWhiteSpace(ExtractBetween(text, startString, endString)));
        }
    }
}