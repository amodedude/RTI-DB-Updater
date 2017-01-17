using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces.Repositories;

namespace RTI.DataBase.Model.Repositories
{
    class WaterDataRepository : Repository<water_data>, IWaterDataRepository
    {
        public WaterDataRepository(RtiContext context) : base(context)
        {
            // Use base class constructor
        }

        public RtiContext RtiContext
        {
            get { return base.Context as RtiContext; }
        }

        /// <summary>
        /// Get the most recent water data
        /// row by ordered measurement date descending.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public water_data GetMostRecentWaterDataValue(string sourceId)
        {
            var result = RtiContext.WaterData.Where(w => w.sourceid == sourceId)
                .OrderByDescending(v => v.measurment_date)
                .FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Return all water_data
        /// values for a given source_id.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public IEnumerable<water_data> GetWaterDataForSource(string sourceId)
        {
            return RtiContext.WaterData.Where(w => w.sourceid == sourceId).ToList();
        }
    }
}
