using System.Collections.Generic;
using RTI.DataBase.Model;

namespace RTI.DataBase.Interfaces.Repositories
{
    public interface IWaterDataRepository
    {
        IEnumerable<water_data> GetWaterDataForSource(string usgsid);
        water_data GetMostRecentWaterDataValue(string usgsid);
    }
}
