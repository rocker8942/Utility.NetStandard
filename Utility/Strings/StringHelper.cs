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
        /// Get byte array with UTF8 Encoding
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StrToByteArray(string str)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }
    }
}