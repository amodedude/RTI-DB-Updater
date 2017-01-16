using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.DataBase.Interfaces
{
    public interface IDownloader
    {
        void download_file(string uri, string filePath, bool useCompression);
    }
}
