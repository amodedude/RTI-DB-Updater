using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using RTI.DataBase.Updater.Config;

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
            _SendOnStatusOk = Email.Settings.SendOnStatusOk;
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
                string body = BuildHTMLBody();
                SendMail(subject, body);
            }
        }

        /// <summary>
        /// Builds the Email HTML body 
        /// from the EmailAlertList.
        /// </summary>
        /// <returns></returns>
        private static string BuildHTMLBody()
        {
            throw new NotImplementedException();
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

                using (var client = new SmtpClient())
                {
                    client.Host = "smtp.Gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("rti.notificationservice", "R#cir720!");

                    using (var message = new MailMessage())
                    {
                        message.Body = body;
                        message.Subject = subject;
                        message.From = new MailAddress(_From);

                        foreach (var recipient in _To)
                            message.To.Add(recipient);

                        if (Email.Settings.AttachLogFile)
                        {
                            if (!Email.Settings.CompressAttachments)
                            {
                                message.Attachments.Add(new Attachment(Logger.LogFileFullPath));
                            }
                            else
                            {

                            }
                        }

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
