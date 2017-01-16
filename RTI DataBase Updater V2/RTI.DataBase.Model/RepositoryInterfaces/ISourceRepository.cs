using System.Collections.Generic;
using RTI.DataBase.Model;

namespace RTI.DataBase.Interfaces.Repositories
{
    public interface ISourceRepository
    {
        IEnumerable<source> GetAllSources();
        IEnumerable<source> GetAllSourcesWithData();
        IEnumerable<source> GetAllSourcesWithOutData();
    }
}
