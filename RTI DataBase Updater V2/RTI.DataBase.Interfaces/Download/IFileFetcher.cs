using RTI.DataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Objects;

namespace RTI.DataBase.Interfaces.Download
{
    public interface IFileFetcher
    {
        SourceCollection FetchFiles();
    }
}
