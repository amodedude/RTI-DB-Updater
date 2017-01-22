using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Model;
using RTI.DataBase.Objects;
using RTI.DataBase.Util;
using RTI.DataBase.Updater.Config;

namespace RTI.Database.UpdaterService.Parse
{
    public class SourcesFileParser
    {
        /// <summary>
        /// Reads a USGS sources
        /// csv file.
        /// </summary>
        /// <param name="sourcesFile"></param>
        /// <returns></returns>
        public SourceCollection ReadFile(string sourcesFile)
        {

            // Read the input source list file.
            FileUtil util = new FileUtil();
            List<string[]> csvList = util.CsvReader(sourcesFile, '\t');

            // Parse contents
            SourceCollection sources = new SourceCollection();
            Dictionary<string,int> mapping = new Dictionary<string, int>();
            foreach (string[] row in csvList)
            {
                if (row.Length > 0)
                {
                    if (csvList.First() == row)
                        mapping = GetCoulumnMapping(row);
                    else
                        sources.Add(ConvertToSource(row, mapping));
                }
            }

            return sources;
        }

        /// <summary>
        /// Convert the current USGS
        /// sources file row to an 
        /// RTI database source using 
        /// the given mapping.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        private source ConvertToSource(string[] row, Dictionary<string, int> mapping)
        {
            source convertedSource = new source();
            foreach (PropertyInfo prop in typeof(source).GetProperties())
            {
                if (mapping.ContainsKey(prop.Name))
                {
                    int column = mapping[prop.Name];
                    typeof(source).GetProperty(prop.Name).SetValue(convertedSource,row[column]);
                }
            }
            return convertedSource;
        }

        /// <summary>
        /// Get the column mapping 
        /// between USGS parameters 
        /// and RTI Database columns.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Dictionary<string,int> GetCoulumnMapping(string[] row)
        {
            FileUtil util = new FileUtil();
            Dictionary<string, string> columnMappingFile = util.ReadMappingFile();
            Dictionary<string,int> mapping = new Dictionary<string, int>();

            foreach (PropertyInfo prop in typeof(source).GetProperties())
            {
                if (columnMappingFile.ContainsValue(prop.Name))
                {
                    string mappedName = columnMappingFile.FirstOrDefault(kvp => kvp.Value == prop.Name).Key;
                    if (!string.IsNullOrEmpty(mappedName))
                    {
                        int column = row.ToList().IndexOf(mappedName);
                        mapping.Add(prop.Name, column);
                    }
                }
            }

            return mapping;
        }

        
    }
}