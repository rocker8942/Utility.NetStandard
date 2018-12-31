using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.IO.Compression;

namespace Utility
{
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class ZipHelper
    {
        public void CompressFile(string filename, string password = null)
        {
            FileStream fsOut = File.Create(filename + ".zip");
            var zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            zipStream.Password = password; // optional. Null is the same as not setting. Required if using AES.

            var fileInfo = new FileInfo(filename);

            string entryName = fileInfo.Name;
            entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
            var newEntry = new ZipEntry(entryName)
            {
                DateTime = fileInfo.LastWriteTime,
                Size = fileInfo.Length
            };

            // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
            // A password on the ZipOutputStream is required if using AES.
            // newEntry.AESKeySize = 256;

            // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
            // you need to do one of the following: Specify UseZip64.Off, or set the Size.
            // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
            // but the zip will be in Zip64 format which not all utilities can understand.
            //   zipStream.UseZip64 = UseZip64.Off;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            var buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(filename))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        /// <summary>
        /// Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
        /// </summary>
        /// <param name="outPathname">Folder name where zipped file will be created</param>
        /// <param name="password">Password can be null, if not required</param>
        /// <param name="folderName">Folder name to archive</param>
        public void CompressFolder(string outPathname, string password, string folderName)
        {
            FileStream fsOut = File.Create(outPathname);
            var zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            zipStream.Password = password; // optional. Null is the same as not setting. Required if using AES.

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

            DoCompressFolder(folderName, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        /// <summary>
        /// Recurses down the folder structure
        /// </summary>
        /// <param name="path"></param>
        /// <param name="zipStream"></param>
        /// <param name="folderOffset"></param>
        private void DoCompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {
                var fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                var newEntry = new ZipEntry(entryName)
                {
                    DateTime = fi.LastWriteTime,
                    Size = fi.Length
                };

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                var buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                DoCompressFolder(folder, zipStream, folderOffset);
            }
        }

        public static byte[] GetGZipByte(byte[] byteData)
        {
            var compressedChunkHolder = new MemoryStream();
            var gzOut = new GZipStream(compressedChunkHolder, CompressionMode.Compress);

            gzOut.Write(byteData, 0, byteData.Length);
            gzOut.Close();

            return compressedChunkHolder.ToArray();
        }
    }
}