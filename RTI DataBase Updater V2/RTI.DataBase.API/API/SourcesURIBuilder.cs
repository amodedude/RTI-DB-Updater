using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using RTI.DataBase.Interfaces.Download;
using System.Threading.Tasks;
using RTI.DataBase.Objects;
using RTI.DataBase.Updater.Config;
using RTI.DataBase.Util;

namespace RTI.DataBase.API
{ 
    public class SourcesURIBuilder : IUriBuilder
    {
        public string BuildUri(string paramCode = "00095")
        {
            var columnCodeList = GetColumnCodes();
            var format = "format=sitefile_output";
            var dateFormat = UsgsApi.Settings.DateFormat;
            var fileFormat = $"sitefile_output_format={UsgsApi.Settings.FileFormatSpecifier.Trim()}";
            var outType = UsgsApi.Settings.OutputDataType.Trim();
            var uri = UsgsApi.Settings.ApiUri.TrimEnd('/') + $"/{outType}?refferred_module={outType}";
            var columnCodes = string.Join("&", columnCodeList.ToArray());
            uri = string.Join("&", uri, fileFormat, format, dateFormat, $"index_pmcode_{paramCode}=1", "group_key = NONE", columnCodes);
            return uri;
        }


        /// <summary>
        /// Read the column code
        /// file.
        /// </summary>
        /// <returns></returns>
        public List<string> GetColumnCodes()
        {
            FileUtil util = new FileUtil();
            List<string> codesList = new List<string>();
            var columnCodeList = util.CsvStringColumnReader(RTI.DataBase.Objects.Properties.Resources.ColumnMappingXref, 0, UsgsApi.Settings.ColumnMappingXrefHasHeader);

            foreach (var row in columnCodeList)
                codesList.Add("column_name=" + row);

            return codesList;
        }
    }
}
