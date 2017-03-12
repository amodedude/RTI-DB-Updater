using Microsoft.VisualStudio.TestTools.UnitTesting;
using RTI.DataBase.UpdaterService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Util;
using RTI.DataBase.GeoCoder;

namespace RTI.DataBase.UpdaterService.Tests
{
    [TestClass()]
    public class ReverseGeoCoderTests
    {
        [TestMethod()]
        public void CalculateDistanceFromSiteTest()
        {
            ILogger loger = new Logger();
            var coder = new ReverseGeoCoder(loger);
            //double result = coder.CalculateDistanceFromSite("38.898556", "-77.037852", "38.897147", "-77.043934");
        }
    }
}