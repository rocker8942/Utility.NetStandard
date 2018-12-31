using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Utility
{
    /// <summary>
    /// Helper class to manage files
    /// </summary>
    public class IOHelper : IIOHelper
    {
        public static readonly string BasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        ///     Reads the text content from a file
        /// </summary>
        /// <returns>text content</returns>
        public string ReadTextFile(string fileLocation)
        {
            string result = null;

            if (File.Exists(fileLocation))
            {
                StreamReader sr = null;

                try
                {
                    sr = File.OpenText(fileLocation);
                    result = sr.ReadToEnd();
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileLocation"></param>
        public void AttatchToTextFile(string content, string fileLocation)
        {
            StreamWriter sw = null;

            try
            {
                sw = File.AppendText(fileLocation);
                sw.Write(content);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fileLocation"></param>
        public void AppendLineToTextFile(string line, string fileLocation)
        {
            StreamWriter sw = null;

            try
            {
                sw = File.AppendText(fileLocation);
                sw.WriteLine(line);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public byte[] ReadFileStream(string fileLocation)
        {
            byte[] result = null;

            if (File.Exists(fileLocation))
            {
                FileStream fs = null;

                try
                {
                    fs = new FileStream(fileLocation, FileMode.Open);
                    result = new byte[fs.Length];
                    fs.Read(result, 0, result.Length);
                }
                catch (Exception)
                {
                    result = null;
                    // System_Reporter.ReportException(ex);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Read a file being used by another process safely
        /// </summary>
        /// <param name="fileLocation"></param>
        public IEnumerable<string> ReadFileSafe(string fileLocation)
        {
            using (FileStream fileStream = new FileStream(
                fileLocation,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string RunExternalProcess(string command, string args)
        {
            var process = new Process();

            process.EnableRaisingEvents = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = args;

            process.Start();

            string result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public bool StringExists(string file, string searchText)
        {
            //Declare reader as a new StreamReader with file as the file to use
            var reader = new StreamReader(file);
            //Declare text as the reader reading to the end
            String text = reader.ReadLine();
            bool result = false;
            while (text == string.Empty)
            {
                if (Regex.IsMatch(text, searchText))
                {
                    result = true;
                    break;
                }
                text = reader.ReadLine();
            }
            //If the searchText is a match
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        ///     Convert a byte array to an Object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public Object ByteArrayToObject(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = binForm.Deserialize(memStream);
            return obj;
        }

        /// <summary>
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public Object ByteArrayStrToObject(string arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(StringHelper.StrToByteArray(arrBytes), 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = binForm.Deserialize(memStream);
            return obj;
        }

        public void LogToFile(string logFilePath, string logMsg)
        {
            StreamWriter stream;
            string logFormat = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " ==> ";

            var f = new FileInfo(logFilePath);
            Directory.CreateDirectory(f.DirectoryName);
            if (!File.Exists(logFilePath))
            {
                stream = File.CreateText(logFilePath);
            } 
            else
            {
                stream = File.AppendText(logFilePath);
            }

            try
            {
                stream.WriteLine(logFormat + logMsg);
                stream.Flush();
                stream.Close();
            }
            catch (Exception)
            {
                // string errMsg = "Fail to write log file.";
                // System_Reporter.ReportException(e, errMsg, "Webstats", "");
            }
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// Add date at the end of original file. If exist, overwrites
        /// </summary>
        /// <param name="fullpath"></param>
        public string ChangeFileToNameWithDate(string fullpath)
        {
            var fileInfo = new FileInfo(fullpath);
            string filenameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.Name);
            string extension = fileInfo.Name.Split('.').ToList().Last();
            string fullPath4NewName = fileInfo.DirectoryName + @"\" + filenameWithoutExt + "_" +
                                      DateTime.Now.ToString("yyyyMMdd") + "." + extension;

            if (File.Exists(fullPath4NewName))
                File.Delete(fullPath4NewName);

            fileInfo.MoveTo(fullPath4NewName);

            return fullPath4NewName;
        }

        /// <summary>
        /// Get the filename with date from full path. File needs to exist in the specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string GetNameWithDate(string filePath)
        {
            string filename = Path.GetFileNameWithoutExtension(filePath);
            Debug.Assert(filename != null, "filename != null");
            return filePath.Replace(filename, filename + "_" + DateTime.Now.ToString("yyyyMMdd"));
        }

        /// <summary>
        /// Get the filename with date and time from full path. File needs to exist in the specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string GetNameWithDateAndTime(string filePath)
        {
            string filename = Path.GetFileNameWithoutExtension(filePath);
            Debug.Assert(filename != null, "filename != null");
            return filePath.Replace(filename, filename + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        }

        /// <summary>
        /// Replace date time place holder with today's data time value
        /// </summary>
        /// <param name="filePath">YYYY-MM-DD hh:mm:ss</param>
        /// <param name="dateOffSet">use this to get the data that is not today providing by +/- days</param>
        /// <returns></returns>
        public string ReplaceFileNameWithDateTime(string filePath, int dateOffSet = 0)
        {
            DateTime now = DateTime.Now.AddDays(dateOffSet);
            var fileName = Path.GetFileName(filePath);

            if (fileName != null)
            {
                fileName = fileName.Replace("YYYY", now.Year.ToString("0000"));
                fileName = fileName.Replace("yyyy", now.Year.ToString("0000"));
                fileName = fileName.Replace("MM", now.Month.ToString("00"));
                fileName = fileName.Replace("DD", now.Day.ToString("00"));
                fileName = fileName.Replace("dd", now.Day.ToString("00"));
                fileName = fileName.Replace("hh", now.Hour.ToString("00"));
                fileName = fileName.Replace("mm", now.Minute.ToString("00"));
                fileName = fileName.Replace("ss", now.Second.ToString("00"));
            }

            Debug.Assert(fileName != null, "fileName != null");
            return Path.Combine(Path.GetDirectoryName(filePath), fileName);
        }

        /// <summary>
        ///     Get file info data from a directory
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<FileInfo> GetFileInfos(string inputPath, string pattern)
        {
            IEnumerable<string> inputFiles = Directory.EnumerateFiles(inputPath, pattern);
            var fileinfos = new List<FileInfo>();
            foreach (string item in inputFiles)
            {
                var fileinfo = new FileInfo(item);
                fileinfos.Add(fileinfo);
            }
            return fileinfos;
        }

        public Stream GetStream(string stringData)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringData);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public byte[] GetByte(Stream sourceStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                sourceStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public string GetString(Stream inputStream)
        {
            string output;
            using (StreamReader reader = new StreamReader(inputStream))
            {
                output = reader.ReadToEnd();
            }
            return output;
        }

        public void WriteStream(Stream inputStream, ref byte[] dataBytes)
        {
            using (Stream outputStream = inputStream)
            {
                outputStream.Write(dataBytes, 0, dataBytes.Length);
            }
        }

        public bool IsDirectory(string pathToLog)
        {
            if (Directory.Exists(pathToLog))
                return true;
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(filePath, append))
            {
                serializer.Serialize(writer, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(filePath))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
            using (var writer = new StreamWriter(filePath, append))
            {
                writer.WriteLine(contentsToWriteToFile);
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            // read stream safe
            using (FileStream fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            {
                using (TextReader reader = new StreamReader(fileStream))
                {
                    var fileContents = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(fileContents);
                }
            }
        }

        /// <summary>
        /// Overwrite file from A to B
        /// </summary>
        /// <param name="from">source file's full path</param>
        /// <param name="to">destination file's full path</param>
        public void FileOverwrite(string from, string to)
        {
            if (File.Exists(to))
                File.Delete(to);
            File.Move(from, to);
        }
    }
}