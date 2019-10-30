using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using LumenWorks.Framework.IO.Csv;

namespace Utility.CSV
{
    /// <summary>
    ///     CSV Helper class
    /// </summary>
    public class CsvHelper
    {
        private const char Separator = ',';
        private const char Qualifier = '"';

        public string CsvFile { get; }
        public DataTable CsvTable { get; set; }

        /// <summary>
        ///     Gets the list of columns of the csv file.
        /// </summary>
        public Collection<string> ColumnList
        {
            get
            {
                var result = new Collection<string>();

                if (CsvTable != null)
                    foreach (DataColumn column in CsvTable.Columns)
                        result.Add(column.ColumnName);

                return result;
            }
        }

        /// <summary>
        ///     Gets the rows of the csv file.
        /// </summary>
        public DataRowCollection Rows => CsvTable.Rows;

        /// <summary>
        ///     Helper class make CSV handling easy
        /// </summary>
        public CsvHelper(string csvFile)
        {
            CsvFile = csvFile;
            CsvTable = new DataTable();
        }

        /// <summary>
        ///     write the data table content into a csv file at the specified location.
        /// </summary>
        /// <param name="csvLocation">csv file output location</param>
        /// <param name="hasHeader"></param>
        /// <param name="encoding"></param>
        public void Write(string csvLocation, bool hasHeader, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            if (CsvTable != null)
                Write(CsvTable.CreateDataReader(), csvLocation, ColumnList, hasHeader);
        }

        /// <summary>
        ///     Output the data in the DataReader to the specified location according to the output columns structure.
        /// </summary>
        /// <param name="r">data reader</param>
        /// <param name="csvLocation">outpur csv location</param>
        /// <param name="columns">output columns</param>
        public void Write(IDataReader r, string csvLocation, Collection<string> columns, bool includeHeaders = true, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            File.WriteAllText(csvLocation, GetCsvRawData(r, columns, includeHeaders), encoding);
        }

        public string GetCsvData(bool includeHeaders = true)
        {
            return GetCsvRawData(CsvTable.CreateDataReader(), ColumnList, includeHeaders);
        }

        private static string GetCsvRawData(IDataReader r, Collection<string> columns, bool includeHeaders)
        {
            var sb = new StringBuilder();

            if (includeHeaders)
                for (var i = 0; i < columns.Count; i++)
                {
                    sb.Append(columns[i].IndexOf(",") > -1 ? Qualifier + columns[i] + Qualifier : columns[i]);

                    if (i < columns.Count - 1)
                        sb.Append(Separator);
                    else
                        sb.Append(SystemConstants.LineBreak);
                }

            while (r.Read())
                for (var i = 0; i < columns.Count; i++)
                {
                    sb.Append(r[columns[i]].ToString().IndexOf(",") > -1
                        ? Qualifier + r[columns[i]].ToString() + Qualifier
                        : r[columns[i]].ToString());

                    if (i < columns.Count - 1)
                        sb.Append(Separator);
                    else
                        sb.Append(SystemConstants.LineBreak);
                }

            return sb.ToString();
        }

        public byte[] ToBinary()
        {
            byte[] result = null;

            if (CsvTable != null)
            {
                var rawData = GetCsvRawData(CsvTable.CreateDataReader(), ColumnList, true);

                var encoder = new ASCIIEncoding();
                result = encoder.GetBytes(rawData);
            }

            return result;
        }

        public override string ToString()
        {
            string result = null;

            if (CsvTable != null)
                result = GetCsvRawData(CsvTable.CreateDataReader(), ColumnList, true);

            return result;
        }

        public void Append(string csvLocation)
        {
            if (CsvTable != null)
            {
                //Gets the existing data in the csv file
                var data = File.ReadAllText(csvLocation);
                //Add the data to string builder
                var sb = new StringBuilder(data);
                //Gets the data from the actual datareader
                var dataToAppend = GetCsvRawData(CsvTable.CreateDataReader(), ColumnList, false);
                //Appends the new data to string builder
                sb.Append(dataToAppend);
                //Writes the file
                File.WriteAllText(csvLocation, sb.ToString());
            }
        }

        /// <summary>
        ///     Read CSV into database from file.
        ///     This method will terminate when CSV format is corrupted.
        /// </summary>
        /// <param name="delimiter"></param>
        /// <param name="hasHeader"></param>
        public void ReadCsvFast(char delimiter, bool hasHeader = true)
        {
            ReadCsvFast(new StreamReader(CsvFile), delimiter, hasHeader);
        }

        /// <summary>
        ///     Read CSV into datatable from StreamReader : TextReader
        ///     This method will terminate when CSV format is corrupted.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        /// <param name="hasHeader"></param>
        public void ReadCsvFast(StreamReader stream, char delimiter, bool hasHeader)
        {
            // open the file which is a CSV file with headers
            using (var csv = new CsvReader(stream, hasHeader, delimiter))
            {
                var fieldCount = csv.FieldCount;

                // Get header
                if (hasHeader)
                {
                    var headers = csv.GetFieldHeaders().ToList();
                    foreach (var header in headers)
                        if (!CsvTable.Columns.Contains(header))
                            CsvTable.Columns.Add(header);
                        else
                            CsvTable.Columns.Add(header + DateTime.Now.Second);
                }
                else
                {
                    for (var i = 0; i < fieldCount; i++)
                        CsvTable.Columns.Add(i.ToString());
                }

                // Get records
                while (csv.ReadNextRecord())
                {
                    var row = CsvTable.NewRow();
                    for (var i = 0; i < fieldCount; i++)
                        row[i] = csv[i];

                    CsvTable.Rows.Add(row);
                }
            }
        }

        /// <summary>
        ///     Set Columns Order by name
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnNames"></param>
        public static void SetColumnsOrder(DataTable table, string[] columnNames)
        {
            for (var columnIndex = 0; columnIndex < columnNames.Length; columnIndex++)
                table.Columns[columnNames[columnIndex]].SetOrdinal(columnIndex);
        }
    }
}