using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces.Repositories;

namespace RTI.DataBase.Model.Repositories
{
    public class SourceRepository : Repository<source>, ISourceRepository
    {
        public SourceRepository(RtiContext context) : base(context)
        {
            // Use base class constructor
        }

        public RtiContext RtiContext
        {
            get { return base.Context as RtiContext;}
        }

        /// <summary>
        /// Get all water sources
        /// </summary>
        /// <returns></returns>
        public IEnumerable<source> GetAllSources()
        {
            return RtiContext.Sources.OrderByDescending(s => s.agency_id).ToList();
        }

        /// <summary>
        /// Get only water sources that have data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<source> GetAllSourcesWithData()
        {
            return RtiContext.Sources.Where(s => s.has_data != null && s.has_data == 1).ToList();
        }

        /// <summary>
        /// Get all sources without data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<source> GetAllSourcesWithOutData()
        {
            return RtiContext.Sources.Where(s => s.has_data != null && s.has_data == 0).ToList();
        }

        public new void Remove(source entity)
        {
            _entities.Attach(entity);
            _entities.Remove(entity);
        }
    }
}
