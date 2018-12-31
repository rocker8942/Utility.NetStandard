using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utility
{
    public interface IIOHelper
    {
        /// <summary>
        ///     Reads the text content from a file
        /// </summary>
        /// <returns>text content</returns>
        string ReadTextFile(string fileLocation);

        /// <summary>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileLocation"></param>
        void AttatchToTextFile(string content, string fileLocation);

        /// <summary>
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fileLocation"></param>
        void AppendLineToTextFile(string line, string fileLocation);

        /// <summary>
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        byte[] ReadFileStream(string fileLocation);

        /// <summary>
        /// Read a file being used by another process safely
        /// </summary>
        /// <param name="fileLocation"></param>
        IEnumerable<string> ReadFileSafe(string fileLocation);

        /// <summary>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        string RunExternalProcess(string command, string args);

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void CopyAll(DirectoryInfo source, DirectoryInfo target);

        /// <summary>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        bool StringExists(string file, string searchText);

        /// <summary>
        ///     Convert a byte array to an Object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        Object ByteArrayToObject(byte[] arrBytes);

        /// <summary>
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        Object ByteArrayStrToObject(string arrBytes);

        void LogToFile(string logFilePath, string logMsg);

        /// <summary>
        /// Add date at the end of original file. If exist, overwrites
        /// </summary>
        /// <param name="fullpath"></param>
        string ChangeFileToNameWithDate(string fullpath);

        /// <summary>
        /// Get the filename with date from full path. File needs to exist in the specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        string GetNameWithDate(string filePath);

        /// <summary>
        /// Get the filename with date and time from full path. File needs to exist in the specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        string GetNameWithDateAndTime(string filePath);

        /// <summary>
        /// Replace date time place holder with today's data time value
        /// </summary>
        /// <param name="filePath">YYYY-MM-DD hh:mm:ss</param>
        /// <param name="dateOffSet">use this to get the data that is not today providing by +/- days</param>
        /// <returns></returns>
        string ReplaceFileNameWithDateTime(string filePath, int dateOffSet = 0);

        /// <summary>
        ///     Get file info data from a directory
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        List<FileInfo> GetFileInfos(string inputPath, string pattern);

        Stream GetStream(string stringData);
        byte[] GetByte(Stream sourceStream);
        string GetString(Stream inputStream);
        void WriteStream(Stream inputStream, ref byte[] dataBytes);
        bool IsDirectory(string pathToLog);

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false);

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        T ReadFromBinaryFile<T>(string filePath);

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
        void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new();

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        T ReadFromXmlFile<T>(string filePath) where T : new();

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
        void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new();

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        T ReadFromJsonFile<T>(string filePath) where T : new();

        /// <summary>
        /// Overwrite file from A to B
        /// </summary>
        /// <param name="from">source file's full path</param>
        /// <param name="to">destination file's full path</param>
        void FileOverwrite(string from, string to);
    }
}