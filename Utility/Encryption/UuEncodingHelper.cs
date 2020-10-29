using System;
using System.IO;
using System.Text;

namespace Utility.Encryption
{
    public class UuEncodingHelper
    {
        /// <summary>
        ///     UuDecode an uuEncoded text and write to a file
        /// </summary>
        /// <param name="text">UuEncoded test</param>
        /// <param name="filename">full path</param>
        public static void UuDecode(string text, string filename)
        {
            text = text.Trim();

            using var output = new MemoryStream();

            var lines = text.Split("\n");

            // skip the 'begin' and 'end' line
            for (var i = 1; i < lines.Length - 2; i++)
            {
                // padding a line to be 61
                if (lines[i].Length < 61)
                    lines[i] = lines[i].PadRight(61, ' ');

                try
                {
                    var decodedBytes = UuDecodeLine(lines[i]);
                    output.Write(decodedBytes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            File.WriteAllBytes(filename, output.ToArray());
        }


        private static byte[] UuDecodeLine(string line)
        {
            // Create an output array
            var outBuffer = new byte[(line.Length - 1) / 4 * 3];
            var outIdx = 0;

            // Get the string as an array of ASCII bytes
            var asciiBytes = Encoding.ASCII.GetBytes(line);

            for (var i = 0; i < asciiBytes.Length; i++) asciiBytes[i] = (byte) ((asciiBytes[i] - 0x20) & 0x3f);

            // Convert each block of 4 input bytes into 3
            for (var i = 1; i < asciiBytes.Length - 1; i += 4)
            {
                outBuffer[outIdx++] = (byte) ((asciiBytes[i] << 2) | (asciiBytes[i + 1] >> 4));
                outBuffer[outIdx++] = (byte) ((asciiBytes[i + 1] << 4) | (asciiBytes[i + 2] >> 2));
                outBuffer[outIdx++] = (byte) ((asciiBytes[i + 2] << 6) | asciiBytes[i + 3]);
            }

            return outBuffer;
        }
    }
}