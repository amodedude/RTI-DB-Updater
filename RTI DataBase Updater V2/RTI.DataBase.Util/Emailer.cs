using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using RTI.DataBase.Updater.Config;
using System.IO;
using System.IO.Compression;
using System.Net.Mime;
using System.Configuration;

namespace RTI.DataBase.Util
{
    /// <summary>
    /// Sends email messages to recipients. 
    /// </summary>
    public static class Emailer
    {
        public static List<Alert> EmailAlertList {get; set; }
        public static List<string> To { get { return _To; } private set { } }
        public static List<string> Cc { get { return _Cc; } private set { } }
        public static List<string> Bcc { get { return _Bcc; } private set { } }
        public static string From { get { return From; } private set { } }
        public static bool SendOnStatusOk { get { return _SendOnStatusOk; } private set { } }
        public static bool SendEmails { get { return _SendEmails; } private set { } }

        private static List<string> _To;
        private static List<string> _Cc;
        private static List<string> _Bcc;
        private static string _From;
        private static bool _SendEmails;
        private static bool _SendOnStatusOk;

        static Emailer()
        {
            _To = Email.Settings.To.Split(',').ToList();
            _Cc = Email.Settings.Cc.Split(',').ToList();
            _Bcc = Email.Settings.Bcc.Split(',').ToList();
            _From = Email.Settings.From;
            _SendEmails = Email.Settings.SendEmails;
            EmailAlertList = new List<Alert>();
        }


        /// <summary>
        /// Trigger EmailAlerts
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void FireEmailAlerts(string subject = null)
        {
            if (_SendEmails)
            {
                string emailSubject = (string.IsNullOrEmpty(subject)) ? (Email.Settings.Subject ?? "RTI Database Updater Error Alert.") : subject;
                string body = BuildHTMLBody();
                SendMail(emailSubject, body);
            }
        }

        /// <summary>
        /// Builds the Email HTML body 
        /// from the EmailAlertList.
        /// </summary>
        /// <returns></returns>
        private static string BuildHTMLBody()
        {
            string HTML = string.Empty;
            HTML = string.Empty;
            HTML = "<h2><strong>RTI Database Updater Error Alert</strong></h2>";

            if (EmailAlertList.Count > 0)
            {
                HTML += "<br /><p>The following errors have been detected:</p>";

                // Detected Error(s) Table
                HTML += @"<table border=""1"" cellpadding=""1"" cellspacing=""1"" style=""table-layout:fixed; width:2000; word-wrap:break-word;"">";
                HTML += @"<thead><tr><th scope = ""col""> Priority </th><th scope = ""col""> Message </th><th scope = ""col""> Exception Message </th><th scope = ""col""> Inner Exception </th><th scope = ""col""> Stack Trace </th><th scope=""col""> Detection Timestamps </th></tr></thead><tbody>";
                foreach (var alert in EmailAlertList)
                {
                    HTML += "<tr>";
                    HTML += @"<td>" + alert.Priority + "</td>";
                    HTML += @"<td>" + alert.Message + "</td>";
                    HTML += @"<td>" + alert.ExceptionMessage + "</td>";
                    HTML += @"<td>" + alert.InnerExceptionMessage + "</td>";
                    HTML += @"<td>" + alert.StackTrace.Replace("\r\n", "<br />") + "</td>";
                    HTML += @"<td align=""center"" nowrap>" + alert.DetectionTimeStamp.ToString("dd/MM/yyyy hh:mm:ss tt") + "</td>";
                    HTML += "</tr>";
                }
                HTML += "</tbody></table>";
                // Detected Error(s) Table END


                // Application Settings table
                HTML += "<br /><br />";
                HTML += @"<table border=""1"" cellpadding=""1"" cellspacing=""1"" style=""table-layout:fixed; width:350; word-wrap:break-word;"">";
                HTML += "<tr>";
                HTML += @"<td colspan=""2"" align=""center""><b><i>Monitoring application settings:</b></i></td>";
                HTML += "<tr>";
                HTML += "<td>Updater Mode:</td>" + "<td>" + Schedule.Settings.Mode + "</td>";
                HTML += "</tr>";
                HTML += "<tr>";
                if (Schedule.Settings.Mode == "Daily")
                {
                    HTML += "<td>Scheduled Check Time:</td> " +
                            "<td>" + Schedule.Settings.ScheduledTime + "</td>";
                }
                else if (Schedule.Settings.Mode == "Interval")
                {
                    HTML += "<td>Update Interval(Min):</td>" +
                            "<td>" + Schedule.Settings.IntervalMinutes + "</td>";
                }
                HTML += "</tr>";
                HTML += "</table>";

                //Application Settings table END
                string emailsIncluded = (Email.Settings.AttachLogFile) ? ((Email.Settings.CompressAttachments) ? "Compressed " : "") + "WebMonitor error logs are attached." : "";
                HTML += $@"<br /><br /><p><font color =""red"">Please contact your administrator immediately. {emailsIncluded}</font></p>";
            }
            else
            {
                HTML += "<p>No server errors have been detected.</p>";
            }

            return HTML;
        }

        /// <summary>
        /// Sends an email alert to the user via the SmtpClient.
        /// </summary>
        /// <param name="adressList"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        private static void SendMail(string subject, string body)
        {
            if (_To == null || _To.Count == 0)
                throw new ArgumentException("Emailer.SendMail(), Bad Recipients List");
            else
            {
                
                ConnectionStringsSection config =  Crypto.GetEncryptedConnectionStringsSection(System.Reflection.Assembly.GetEntryAssembly().Location);
                ConnectionStringSettings connectionStringSection = config.ConnectionStrings["RTIEmailServer"];
                string connection = connectionStringSection.ConnectionString;
                List<string> connectionList = connection.Split(new char[] { '=', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Dictionary<string, string> connectionKVP = connectionList.Select((v, i) => new { value = v, index = i })
                 .Where(o => o.index % 2 == 0)
                 .ToDictionary(o => o.value, o => connectionList[o.index + 1]);

                using (var client = new SmtpClient())
                {
                    client.Host = connectionKVP["server"];
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(connectionKVP["user id"], connectionKVP["password"]);

                    using (var message = new MailMessage())
                    {
                        message.Body = body;
                        message.Subject = subject;
                        message.IsBodyHtml = true;
                        message.From = new MailAddress(_From);

                        foreach (var recipient in _To)
                            message.To.Add(recipient);

                        if (Email.Settings.AttachLogFile)
                        {
                            if (!Email.Settings.CompressAttachments)
                            {
                                message.Attachments.Add(new Attachment(Logger.LogFileFullPath));
                                client.Send(message);
                            }
                            else
                                SendWithCompressedAttachments(Logger.LogFileFullPath, client, message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sends an email with a
        /// compressed file attachment.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private static void SendWithCompressedAttachments(string attachmentFilePath, SmtpClient client, MailMessage message)
        {
            string line;
            Attachment attachment;
            using (MemoryStream stream = new MemoryStream())
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, false))
            {
                ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(attachmentFilePath));
                using (Stream entryStream = entry.Open())
                using (StreamWriter writer = new StreamWriter(entryStream))
                using (StreamReader reader = new StreamReader(attachmentFilePath))
                {
                    while ((line = reader.ReadLine()) != null)
                        writer.WriteLine(line);

                    writer.Dispose();
                    writer.Close();
                    archive.Dispose();
                    using (MemoryStream attachmentStream = new MemoryStream(stream.ToArray()))
                    {
                        string zipFileName = Path.GetFileNameWithoutExtension(Email.Settings.CompressedFileName) + ".zip";
                        attachment = new Attachment(attachmentStream, zipFileName, MediaTypeNames.Application.Zip);
                        message.Attachments.Add(attachment);
                        client.Send(message);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new Alert
        /// using data from an Exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static void ProcessExcption(Exception ex, string message = null, Priority priority = Priority.Error)
        {
            Alert alert = new Alert();
            alert.Priority = priority;
            alert.Message = message ?? "";
            alert.ExceptionMessage = ex?.Message ?? "";
            alert.InnerExceptionMessage = ex?.InnerException?.Message ?? "";
            alert.StackTrace = ex?.StackTrace ?? "";
            alert.DetectionTimeStamp = DateTime.Now;
            alert.Exception = ex;
            EmailAlertList.Add(alert);
        }
    }

    /// <summary>
    /// Represents an 
    /// EmailAlert type.
    /// </summary>
    public class Alert
    {
        public Priority Priority { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public DateTime DetectionTimeStamp { get; set; }
        public Exception Exception { get; set; }
    }
}
