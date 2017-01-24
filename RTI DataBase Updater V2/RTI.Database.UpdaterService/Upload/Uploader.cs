using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.UpdaterService.Upload
{
    /// <summary>
    /// Uploads retrieved data into the RTI database. 
    /// </summary>
    class Uploader
    {
        ILogger LogWriter;
        public bool Upload(List<water_data> data, string USGSID, ILogger logger)
        {
            LogWriter = logger;
            bool data_uploaded = false;
            bool isError = false;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            // Split the data-list into chucks to significantly increase insert performance. 
            var splitData =  SplitList.Chunk(data,1000);
            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RTIDBModel"].ConnectionString;
            //var result = from Match match in Regex.Matches(ConnectionString, "\"([^\"]*)\"")
            //             select match.ToString();

            //ConnectionString = result.First().ToString().TrimEnd('"').TrimStart('"');
            
            // Manually commit to the database (no EF due to speed issues)
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    List<string> Rows = new List<string>();

                    // Retrieve the timestamp for the last avalible conductivity datapoint.
                    var latestDatasetDate = data.Last().measurment_date;

                    DateTime? latestDatabaseDate;
                    using (UnitOfWork uoa = new UnitOfWork())
                        latestDatabaseDate = uoa.WaterData.GetMostRecentWaterDataValue(USGSID)?.measurment_date;

                    if (latestDatabaseDate < latestDatasetDate || latestDatabaseDate == null)
                    {
                        foreach (var chunk in splitData)
                        {
                            StringBuilder sCommand =
                                new StringBuilder(
                                    "INSERT INTO water_data (cond, temp, measurment_date, sourceid) VALUES ");
                            foreach (var date in chunk)
                            {
                                if (date.measurment_date > latestDatabaseDate)
                                    Rows.Add(string.Format("({0}, {1}, '{2}', '{3}')",
                                        MySqlHelper.EscapeString(date.cond.ToString()),
                                        MySqlHelper.EscapeString("NULL"),
                                        MySqlHelper.EscapeString(
                                            date.measurment_date.GetValueOrDefault().ToString("yyyy-MM-dd HH':'mm':'ss",
                                                System.Globalization.CultureInfo.InvariantCulture)),
                                        MySqlHelper.EscapeString(date.sourceid)));
                            }
                            if (Rows.Count > 0)
                            {
                                sCommand.Append(string.Join(",", Rows));
                                sCommand.Append(" ON DUPLICATE KEY UPDATE dataID = dataID;");
                                data_uploaded = ExecuteMySqlCommand(sCommand.ToString(), connection);
                                ExecuteMySqlCommand(
                                    $"UPDATE rtidev.sources s set s.has_data = 1 where s.agency_id = {USGSID};",
                                    connection);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                isError = true;
                Debugger.Break();
                LogWriter.WriteMessageToLog(
                    "\r\nERROR: The system encountered an error, no data was uploaded for source " + USGSID);
                LogWriter.WriteMessageToLog(
                    "ERROR: In Uploader(), There was an error connecting to the database, please check that your connection settings are valid.\n" +
                    ex.Message);
            }

            timer.Stop();

            if (data_uploaded && !isError)
            {
                LogWriter.WriteMessageToLog($"Upload Complected in {timer.Elapsed.Milliseconds} ms.\r\n");
            }
            else if (!isError && !data_uploaded)
            {
                if (Application.Settings.DebugMode)
                {
                    LogWriter.WriteMessageToLog($"Debug Mode is on - No data uploaded for source {USGSID}");
                }
                else
                {
                    LogWriter.WriteMessageToLog("\r\nDatabase is up to date for source " + USGSID);
                    LogWriter.WriteMessageToLog("No data uploaded, source is up to date for USGSID " + USGSID + "\r\n");
                }
            }
            Thread.Sleep(Application.Settings.UploadTimeOutMilliseconds);
            return data_uploaded;
        }

        /// <summary>
        /// Executes a MySql Command
        /// </summary>
        /// <param name="commandString"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private bool ExecuteMySqlCommand(string commandString, MySqlConnection connection)
        {
            if (!Application.Settings.DebugMode)
            {
                using (MySqlCommand myCmd = new MySqlCommand(commandString, connection))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

    static class SplitList
    {
        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }
    }
}
