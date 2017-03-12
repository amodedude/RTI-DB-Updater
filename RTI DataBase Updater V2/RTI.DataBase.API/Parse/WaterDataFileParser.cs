using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RTI.DataBase.UpdaterService.Upload;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.API.Parse
{
    /// <summary>
    /// Handles reading of downloaded files. 
    /// </summary>
    public class WaterDataFileParser
    {
        ILogger LogWriter;
        public WaterDataFileParser(ILogger logger)
        {
            LogWriter = logger;
        }
        /// <summary>
        /// Initializes the file read operation. 
        /// </summary>
        /// <param name="filePath"></param>
        public void ReadFile(string filePath, string USGSID)
        {
            CurrentLineNumber = 0;
            if (File.Exists(filePath))
                OpenFile(filePath, USGSID);
        }

        /// <summary>
        /// Opens the file to be read. 
        /// </summary>
        /// <param name="filePath"></param>
        private void OpenFile(string filePath, string USGSID)
        {
            try
            {
                LogWriter.WriteMessageToLog("Opening File: " + filePath);

                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string, and write the string to the console
                    var data = ExtractData(sr, filePath);
                    Uploader rtiUploader = new Uploader();
                    if(data.Count() > 0) // Only upload if there is data to be uploaded. 
                        rtiUploader.Upload(data, USGSID, LogWriter);
                }
           }
            catch (Exception ex)
            {
                LogWriter.WriteErrorToLog(ex, $"There was an error reading file: {filePath}",true,Priority.Warning);
            }
        }


        /// <summary>
        /// Extracts conductivity and date values from each line.
        /// Returns a list of data records for each day. 
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns></returns>
        private int dateCol = 0, condCol = 0, sourceCol = 0;

        private List<water_data> ExtractData(StreamReader fileContents, string filePath)
        {
            DateTime lastDate = new DateTime();
            List<water_data> data = new List<water_data>();
            List<int> averageCond = new List<int>();
            char[] delimiter = new char[] {'\t'};
            int numHeaders = 2;
            bool isHeaderFound = false;
            bool isFirstRow = true;
            int highestHeaderIndex = 0;

            try
            {
                // Read the file line by line 
                while (!fileContents.EndOfStream)
                {
                    string line = fileContents.ReadLine();

                    // Extract Data
                    if (!line.StartsWith("#"))
                    {
                        if ((!line.Contains("agency_cd") || !line.Contains("site_no") || !line.Contains("datetime")) &&
                            isHeaderFound)
                        {
                            var segments = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                            if(segments.Length-1 < highestHeaderIndex)
                                continue;

                            DateTime currentdate;
                            bool dateFormatOk = DateTime.TryParse(segments[dateCol], out currentdate);
                            int cond;
                            bool condFormatOk = int.TryParse(segments[condCol], out cond);
                            averageCond.Add(cond);

                            if (isFirstRow)
                            {
                                lastDate = currentdate;
                                isFirstRow = false;
                            }

                            if (dateFormatOk && condFormatOk)
                            {
                                if (currentdate.Day != lastDate.Day)
                                {
                                    var todaysData = new water_data();
                                    todaysData.measurment_date = currentdate;
                                    todaysData.cond = Convert.ToInt32(averageCond.Average());
                                    todaysData.sourceid = segments[sourceCol];

                                    data.Add(todaysData);

                                    //DEBUG
                                    //UserInterface.WriteToConsole("SouceID: " + todaysData.sourceid + "    Date: " + currentdate + "    Cond: " + cond);
                                    averageCond.Clear();
                                    lastDate = currentdate;
                                }
                            }
                        }
                        else
                        {
                            var header = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            string paramCode = string.Join("_", "00095", UsgsApi.Settings.StatisticCode);
                            dateCol =
                                header.Select((v, i) => new {Index = i, Value = v})
                                    .Where(p => p.Value == "datetime")
                                    .Select(p => p.Index)
                                    .ToList()
                                    .DefaultIfEmpty(-999)
                                    .FirstOrDefault();
                            condCol =
                                header.Select((v, i) => new {Index = i, Value = v})
                                    .Where(p => p.Value.Contains(paramCode) && !p.Value.Contains("cd"))
                                    .Select(p => p.Index)
                                    .ToList()
                                    .DefaultIfEmpty(-999)
                                    .FirstOrDefault();
                            sourceCol =
                                header.Select((v, i) => new {Index = i, Value = v})
                                    .Where(p => p.Value == "site_no")
                                    .Select(p => p.Index)
                                    .ToList()
                                    .DefaultIfEmpty(-999)
                                    .FirstOrDefault();

                            if (dateCol != -999 && condCol != -999 && sourceCol != -999)
                                // If -999, then parameters do not exist in this line. 
                            {
                                isHeaderFound = true;
                                highestHeaderIndex = new[] {dateCol, condCol, sourceCol}.Max();

                                for (int i = 0; i < numHeaders - 1; i++)
                                    fileContents.ReadLine();
                            }
                            else
                            {
                                LogWriter.WriteMessageToLog("\r\n"+filePath +
                                                            $" is not formated properly. \r\nCould not find data for parameter code {paramCode}.\r\nThis file and it's contents will not be parsed from line " +
                                                            Convert.ToString(CurrentLineNumber) + ".\r\n", Priority.Error);
                                break; // Stop reading the file upon incorrect text format detection
                            }
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessageToLog("Error: " + ex.Message + " Inner" + ex.InnerException);
                System.Diagnostics.Debugger.Break();
                throw;
            }
            finally
            {
                fileContents.Dispose();
            }
        }

        /// <summary>
        /// Stores the current line number being read. 
        /// </summary>
        private long CurrentLineNumber { get; set; }

        /// <summary>
        /// Reads the next line in the file stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string ReadNextLine (StreamReader fileStream)
        {
            CurrentLineNumber++; // Advance to the next line
            return fileStream.ReadLine();
        }

    }   
}
