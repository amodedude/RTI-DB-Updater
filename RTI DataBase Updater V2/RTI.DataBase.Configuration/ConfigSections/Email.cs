﻿using System.Configuration;

namespace RTI.DataBase.Updater.Config
{
    public class Email : ConfigurationSection
    {
        private static Email _settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection("Email") as Email;

        public static Email Settings
        {
            get { return _settings; }
        }

        [ConfigurationProperty("SendEmails", IsRequired = true)]
        public bool SendEmails
        {
            get { return (bool)this["SendEmails"]; }
        }

        [ConfigurationProperty("SendOnStatusOk", IsRequired = true)]
        public bool SendOnStatusOk
        {
            get { return (bool)this["SendOnStatusOk"]; }
        }

        [ConfigurationProperty("Subject", IsRequired = true)]
        public string Subject
        {
            get { return (string)this["Subject"]; }
        }

        [ConfigurationProperty("From", IsRequired = true)]
        public string From
        {
            get { return (string)this["From"]; }
        }

        [ConfigurationProperty("To", IsRequired = true)]
        public string To
        {
            get { return (string)this["To"]; }
        }

        [ConfigurationProperty("Cc", IsRequired = true)]
        public string Cc
        {
            get { return (string)this["Cc"]; }
        }

        [ConfigurationProperty("Bcc", IsRequired = true)]
        public string Bcc
        {
            get { return (string)this["Bcc"]; }
        }

        [ConfigurationProperty("EmailIntervalMin", IsRequired = true)]
        public int EmailIntervalMin
        {
            get { return (int)this["EmailIntervalMin"]; }
        }

        [ConfigurationProperty("AttachLogFile", IsRequired = true)]
        public bool AttachLogFile
        {
            get { return (bool)this["AttachLogFile"]; }
        }

        [ConfigurationProperty("CompressAttachments", IsRequired = true)]
        public bool CompressAttachments
        {
            get { return (bool)this["CompressAttachments"]; }
        }
    }
}