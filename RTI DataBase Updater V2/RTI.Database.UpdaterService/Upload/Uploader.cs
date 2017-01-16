using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Model;

namespace RTI.Database.UpdaterService.Upload
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
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    List<string> Rows = new List<string>();

                    // Retrieve the timestamp for the last avalible conductivity datapoint.
                    var latest_dataset_date = data.Last().measurment_date;
                    var latest_database_date = RetrieveLatestDate(connection, USGSID).Date;

                    if (latest_database_date < latest_dataset_date)
                    {
                        foreach (var chunk in splitData)
                        {
                            StringBuilder sCommand = new StringBuilder("INSERT INTO water_data (cond, temp, measurment_date, sourceid) VALUES ");
                            foreach (var date in chunk)
                            {
                                if(date.measurment_date > latest_database_date)
                                    Rows.Add(string.Format("({0}, {1}, '{2}', '{3}')", MySqlHelper.EscapeString(date.cond.ToString()), MySqlHelper.EscapeString("NULL"), MySqlHelper.EscapeString(date.measurment_date.GetValueOrDefault().ToString("yyyy-MM-dd HH':'mm':'ss", System.Globalization.CultureInfo.InvariantCulture)), MySqlHelper.EscapeString(date.sourceid)));
                            }
                            if (Rows.Count > 0) 
                            {
                                sCommand.Append(string.Join(",", Rows));
                                sCommand.Append(" ON DUPLICATE KEY UPDATE dataID = dataID;");

                                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), connection))
                                {
                                    myCmd.CommandType = CommandType.Text;
                                    myCmd.ExecuteNonQuery();
                                    data_uploaded = true;
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                catch(MySqlException ex)
                {
                    isError = true;
                    Debugger.Break();
                    LogWriter.WriteMessageToLog("\nERROR: The system encountered an error, no data was uploaded for source " + USGSID);
                    LogWriter.WriteMessageToLog("ERROR: In Uploader(), There was an error connecting to the database, please check that your connection settings are valid.\n" + ex.Message);
                }             
            }
            timer.Stop();

            if (data_uploaded && !isError)
            {
                LogWriter.WriteMessageToLog($"\nUpload Complected in {timer.Elapsed.Milliseconds} ms.");
                LogWriter.WriteMessageToLog("Upload Complected in " + timer.Elapsed.Milliseconds.ToString() + " ms.\r\n");
            }
            else if(!isError && !data_uploaded)
            {
                LogWriter.WriteMessageToLog("\r\nDatabase is up to date for source " + USGSID);
                LogWriter.WriteMessageToLog("No data uploaded, source is up to date for USGSID " + USGSID + "\r\n");
            }
            Thread.Sleep(1200);
            return data_uploaded;
        }     

        /// <summary>
        /// Retrieves the date for the latest
        /// conductivity entry in the RTI database
        /// water_data table. 
        /// </summary>
        internal DateTime RetrieveLatestDate(MySqlConnection connection, string USGSID)
        {
            try
            {
                DateTime date = new DateTime();
                if (connection.State == ConnectionState.Open)
                {
                    StringBuilder sCommand = new StringBuilder("SELECT MAX(measurment_date) FROM water_data WHERE sourceID = ");
                    sCommand.Append((USGSID + ";"));
                    string query = sCommand.ToString();

                    using (MySqlCommand cmd = new MySqlCommand(sCommand.ToString(), connection))
                    {
                        var result = cmd.ExecuteScalar();
                        DateTime.TryParse(result.ToString(),out date);
                    }
                }
                return date;
            }
            catch(Exception ex)
            {
                Debugger.Break();
                LogWriter.WriteMessageToLog("ERROR: In RetrieveLatestDate(), There was an error retrieving data from the database.\n" + ex.Message);
                //EmailService emailAlert= new EmailService();
                //List<string> address = new List<string>();
                //address.Add("amodedude@gmail.com");
                //emailAlert.SendMail(address, "RTI Alert Testing", "This is a test from the RTI database updater application.");
                throw ex;
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
