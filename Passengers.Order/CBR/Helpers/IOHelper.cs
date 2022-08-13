using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Order.CBR.Helpers
{
    public static class IOHelper
    {
        public static List<T> ReadCsvFile<T>(string webRootPath)
        {
            List<T> list = new List<T>();
            try
            {
                var path = Path.Combine(webRootPath, "data_set.csv");
                using (var reader = new StreamReader(path, Encoding.Default))
                using (var csv = new CsvReader(reader, CultureInfo.CreateSpecificCulture("en-us")))
                {
                    list = csv.GetRecords<T>().ToList();
                }
            }
            catch
            {
            }
            return list;
        }
        public static void InsertRecordToCsvFile<T>(T newItem, string webRootPath)
        {
            bool append = true;
            var path = Path.Combine(webRootPath, "data_set.csv");
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.HasHeaderRecord = !append;
            using (var writer = new StreamWriter(path, append))
            {
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(new List<T>(1) { newItem });
                }
            }
        }
    }
}
