using System.IO;
using System.IO.Compression;
using System.Text;

namespace Utility.Strings
{
    /// <summary>
    /// Useful for storing big strings in a database or files
    /// </summary>
    public static class StringZip
    {
        /// <summary>
        /// Compress the string
        /// </summary>
        /// <param name="input">The desired text for compressing</param>
        /// <returns>A byte array</returns>
        public static byte[] ZipString(this string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);

            var memoryStream = new MemoryStream();
            using (var gzipOut = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gzipOut.Write(bytes, 0, bytes.Length);
            }
            
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Decompress the string
        /// </summary>
        /// <param name="input">Compressed byte array</param>
        /// <returns>Uncompressed string</returns>
        public static string UnzipString(this byte[] input)
        {
            using var memoryStream = new MemoryStream(input);
            using var outputStream = new MemoryStream();
            using var gzipOut = new GZipStream(memoryStream, CompressionMode.Decompress);
            gzipOut.CopyTo(outputStream);
            return Encoding.UTF8.GetString(outputStream.ToArray());
        }
    }
}
