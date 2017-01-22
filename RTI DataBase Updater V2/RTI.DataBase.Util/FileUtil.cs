using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.Util
{

    public class FileUtil
    {
        /// <summary>
        /// Read a CSV file 
        /// into a List.
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <param name="delimiter"></param>
        /// <param name="commentSpecifier">Any line in the file that starts with this will be skipped.</param>
        /// <returns></returns>
        public List<string[]> CsvReader(string fullFilePath, char delimiter = ',', string commentSpecifier = "#")
        {
            List<string[]> csvList = new List<string[]>();
            if (File.Exists(fullFilePath) && fullFilePath.Length > 0)
            {
                using (StreamReader reader = new StreamReader(fullFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line) || line.StartsWith(commentSpecifier)) continue;
                        string[] fields = line.Split(new[] {delimiter}, StringSplitOptions.None);
                        if(fields.Length > 0)
                            csvList.Add(fields);
                    }
                }
            }
            return csvList;
        }

        /// <summary>
        /// Reads a CSV string and 
        /// returns a list of its contents.
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="hasHeader"></param>
        /// <param name="columnDelimiter"></param>
        /// <param name="rowDelimiter"></param>
        /// <returns></returns>
        public List<string[]> CsvStringReader(string contents, bool hasHeader = false, char columnDelimiter = ',', string[] rowDelimiter = null)
        {
            string[] rowDelim = rowDelimiter ?? new[] { "\r\n", "\n" };
            var columList = new List<string[]>();
            string[] rows = contents.Split(rowDelim, StringSplitOptions.None);
            int cnt = 0;
            foreach (string row in rows.AsEnumerable())
            {
                cnt++;
                if (hasHeader && cnt == 1) // Skip header
                    continue;
                else
                    columList.Add(row.Split(columnDelimiter));
            }

            return columList;
        }

        /// <summary>
        /// Reads a CSV string and 
        /// returns a list of its contents for 
        /// a single row.
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="rowNum">Index of the column to read.</param>
        /// <param name="columnDelimiter"></param>
        /// <param name="rowDelimiter"></param>
        /// <returns></returns>
        public List<string> CsvStringColumnReader(string contents, int rowNum, bool hasHeader = false, char columnDelimiter = ',', string[] rowDelimiter = null)
        {
            var result = new List<string>();
            string[] rowDelim = rowDelimiter ?? new[] { "\r\n", "\n" };
            List<string[]> columList = CsvStringReader(contents, hasHeader, columnDelimiter, rowDelim);

            foreach (var row in columList)
            {
                if (row.Length > rowNum)
                {
                    if (!string.IsNullOrEmpty(row[rowNum]))
                        result.Add(row[rowNum]);
                }
            }
            return result;
        }

        /// <summary>
        /// Reads the Company mapping file. 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ReadMappingFile()
        {
            Dictionary<string, string> columnMapping = new Dictionary<string, string>();
            string mappingFile = Objects.Properties.Resources.ColumnMappingXref;
            List<string[]> contents = CsvStringReader(mappingFile, USGS.Settings.ColumnMappingXrefHasHeader);

            foreach (string[] row in contents)
            {
                if (!string.IsNullOrEmpty(row[0]) && !string.IsNullOrEmpty(row[0]))
                    columnMapping.Add(row[0], row[1]);
            }

            return columnMapping;
        }
    }
}