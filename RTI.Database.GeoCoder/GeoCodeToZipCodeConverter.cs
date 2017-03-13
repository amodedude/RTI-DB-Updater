using RTI.Database.GeoCoder.Objects;
using RTI.DataBase.Interfaces.Util;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI.DataBase.GeoCoder
{
    /// <summary>
    /// Handles the conversion of GeoCode
    /// lat/lng data into the nearest zip 
    /// code data.
    /// </summary>
    public class GeoCodeToZipCodeConverter
    {
        ILogger LogWriter;
        private List<ZipCodeTabulation> _zipTabulations { get; set; }

        public GeoCodeToZipCodeConverter(ILogger _logger)
        {
            LogWriter = _logger;

            // Read the Zip Code file
            FileUtil util = new FileUtil();
            List<string[]> tabulationFile = util.CsvReader(Application.Settings.ZipCodeTabulationFile, Convert.ToChar(9));
            _zipTabulations = ListStringToZipCodeTabulation(tabulationFile);
        }

        /// <summary>
        /// Appends Zip Code 
        /// information to a SourceCollection
        /// using decimal latitude and longitude.
        /// </summary>
        public SourceCollection AppendZipCodes(SourceCollection sources)
        {
            List<source> sourcesWithZip = new List<source>();
            foreach(source src in sources)
            {
                source zipSource = src;
                var lat = src.exact_lat;
                var lng = src.exact_lng;
                ZipCodeTabulation zipCode = ExtractZipCode(lat, lng);

                // Add the closest zip code.
                if (zipCode != null)
                    zipSource.post_code = zipCode.GeoId.ToString();

                sourcesWithZip.Add(zipSource);
            }

            return new SourceCollection(sourcesWithZip);
        }


        /// <summary>
        /// Parses the US Census 
        /// </summary>
        /// <returns></returns>
        public ZipCodeTabulation ExtractZipCode(string lat, string lng)
        {
            ReverseGeoCoder coder = new ReverseGeoCoder(LogWriter);
            Dictionary<string, double> distanceToPoint = new Dictionary<string, double>();
            foreach(ZipCodeTabulation zip in _zipTabulations)
            {
                // Calculate the distance between the input lat-long and each zip code.
                if (zip.IntPtLat != -999 && zip.IntPtLong != -999)
                {
                    var distance = coder.MilesBetweenCoordinates(lat,lng, zip.IntPtLat.ToString(), zip.IntPtLong.ToString());
                    distanceToPoint.Add(zip.GeoId,distance);
                }
                else
                {
                    LogWriter.WriteMessageToLog($"Unable to determine decimal Latitude and Longitude for GeoID {zip.GeoId}.");
                }
            }

            // Find the closest zip code by distance.
            double minimumDistance = distanceToPoint.Min(r => r.Value);
            string closestZipCode = distanceToPoint.Where(r=>r.Value == minimumDistance).FirstOrDefault().Key;

            if (closestZipCode != null)
                return _zipTabulations.Where(r => r.GeoId == closestZipCode).FirstOrDefault();
            else
                return null;
        }


        /// <summary>
        /// Parse the input 
        /// CSV into a list of 
        /// ZipCodeTabulations. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<ZipCodeTabulation> ListStringToZipCodeTabulation(List<string[]> input)
        {
            List<ZipCodeTabulation> zipList = new List<ZipCodeTabulation>();
            if (input.Count > 0)
            {
                Dictionary<int, string> fileColumnMapping = new Dictionary<int, string>();
                for (int i = 0; i < input.Count; i++)
                {
                    string[] elements = input.ElementAt(i);
                    if (elements.Count() == 7)
                    {
                        ZipCodeTabulation zip = new ZipCodeTabulation();
                        int col = 0;
                        foreach (var element in elements)
                        {
                            if (i == 0)
                                fileColumnMapping.Add(col, element);
                            else
                            {
                                bool ok = false;
                                decimal aland, awater, alandSqmi, awaterSqmi, intPtLat, intPtLong;
                                string column = fileColumnMapping.ElementAt(col).Value;
                                switch (column.ToUpper().Trim())
                                {
                                    case "GEOID":
                                        zip.GeoId = element;
                                        break;
                                    case "ALAND":
                                        ok = decimal.TryParse(element, out aland);
                                        if (ok) zip.ALand = aland; else zip.ALand = -999;
                                        break;
                                    case "AWATER":
                                        ok = decimal.TryParse(element, out awater);
                                        if (ok) zip.AWater = awater; else zip.AWater = -999;
                                        break;
                                    case "ALAND_SQMI":
                                        ok = decimal.TryParse(element, out alandSqmi);
                                        if (ok) zip.ALand_Sqmi = alandSqmi; else zip.ALand_Sqmi = -999;
                                        break;
                                    case "AWATER_SQMI":
                                        ok = decimal.TryParse(element, out awaterSqmi);
                                        if (ok) zip.AWater_Sqmi = awaterSqmi; else zip.AWater_Sqmi = -999;
                                        break;
                                    case "INTPTLAT":
                                        ok = decimal.TryParse(element, out intPtLat);
                                        if (ok) zip.IntPtLat = intPtLat; else zip.IntPtLat = -999;
                                        break;
                                    case "INTPTLONG":
                                        ok = decimal.TryParse(element, out intPtLong);
                                        if (ok) zip.IntPtLong = intPtLong; else zip.IntPtLong = -999;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            col++;
                        }
                        if(i>0)
                            zipList.Add(zip);
                    }
                    else
                    {
                        LogWriter.WriteErrorToLog(new Exception($"Incorrect number of parameters. Unable to convert List to ZipCodeTabulation at row {i+1}."));
                        if (i == 0)
                            break;
                        else
                            continue;
                    }
                }
            }
            else
            {
                LogWriter.WriteErrorToLog(new Exception($"Input list is empty. Unable to convert List to ZipCodeTabulation, input file may be empty. \r\nZipCodeInputFile:{Application.Settings.ZipCodeTabulationFile}"));
            }
            return zipList;
        }
    }
}
