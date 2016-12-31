using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTI.DataBase.Util
{
    public class Logger
    {
        string _logPath;
        DateTime _date = new DateTime();

        /// <summary>
        /// Initialize a new 
        /// Logging session.
        /// </summary>
        public Logger()
        {
            _date = DateTime.UtcNow;
            _logPath = Application.StartupPath;
        }
    }


}
