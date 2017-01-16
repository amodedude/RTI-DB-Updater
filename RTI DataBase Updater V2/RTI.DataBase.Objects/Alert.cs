using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.DataBase.Objects
{
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
