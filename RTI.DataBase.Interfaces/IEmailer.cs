using System.Collections.Generic;

namespace RTI.DataBase.Interfaces
{
    public interface IEmailer
    {
        List<string> To { get; }
        List<string> Cc { get; }
        List<string> Bcc { get; }
        string From { get; }

        void FireEmailAlerts(string subject = null);
    }
}
