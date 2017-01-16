using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces.Repositories;

namespace RTI.DataBase.Interfaces
{
    interface IUnitOfWork : IDisposable
    {
        IWaterDataRepository WaterData { get; }
        ISourceRepository Sources { get; }
        int Complete();
    }
}
