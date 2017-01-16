using System;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;

namespace RTI.DataBase.Interfaces
{
    public interface IExceptionProcessor
    {
        void ProcessExcption(Exception ex, string message = null, Priority priority = Priority.Error);
    }
}
