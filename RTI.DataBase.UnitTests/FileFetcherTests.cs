using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTI.DataBase.UpdaterService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.UpdaterService.Download;
using RTI.DataBase.Interfaces;
using RTI.DataBase.Util;

namespace RTI.DataBase.UpdaterService.Tests
{
    [TestClass()]
    public class FileFetcherTests
    {
        [TestMethod()]
        public string BuildUriTest()
        {
            // USGS station numbers are from 8 to 15 digits long
            string testUSGSID = "000123643";
            Logger logger = new Logger();
            TextFileDownloader downloader = new TextFileDownloader();
            WaterDataFileFetcher fileFetcher = new WaterDataFileFetcher(logger, downloader);
            WaterDataURIBuilder builder = new WaterDataURIBuilder();
            return builder.BuildUri(testUSGSID);
        }
    }
}