using RTI.DataBase.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.DataBase.Model.Repositories
{
    public class CustomerWaterRepository : Repository<customer_water>, ICustomerWaterRepository
    {
        public CustomerWaterRepository(RtiContext context) : base(context)
        {
            // Use base class constructor
        }

        public RtiContext RtiContext
        {
            get { return base.Context as RtiContext; }
        }

        /// <summary>
        /// Get all customer water
        /// rows associated with 
        /// a given source.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<customer_water> GetAllCustomerWatersWithSouce(source source)
        {
            return
                RtiContext.CustomerWaters.Where(
                    cw => cw.first_sourceID == source.sources_sourceID || cw.second_sourceID == source.sources_sourceID);
        }

        public new void Remove(customer_water entity)
        {
            _entities.Attach(entity);
            _entities.Remove(entity);
        }
    }
}