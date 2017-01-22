using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTI.Database.UpdaterService.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Updater.Config;

namespace RTI.Database.UpdaterService.Download.Tests
{
    [TestClass()]
    public class SourcesURIBuilderTests
    {
        [TestMethod()]
        public void BuildUriTest()
        {
            SourcesURIBuilder builder = new SourcesURIBuilder();
            string result = builder.BuildUri();
        }
    }
}