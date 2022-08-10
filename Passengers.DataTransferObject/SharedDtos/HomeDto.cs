using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SharedDtos
{
    public class HomeDto
    {
        public int AdminCount { get; set; }
        public int DriverCount { get; set; }
        public int CustomerCount { get; set; }
        public int ShopCount { get; set; }

        public OrderStatisticDto OrderStatisticDto { get; set; }
        public ImportStatisticDto ImportStatisticDto { get; set; }
        public ExportStatisticDto ExportStatisticDto { get; set; }
        public List<UserInfoDto> BestDrivers { get; set; }
        public List<UserInfoDto> BestCustomers { get; set; }
        public List<UserInfoDto> BestShops { get; set; }
    }

    public class OrderStatisticDto
    {
        public int OrderCount { get; set; }
        public List<int> OrderMonthly { get; set; }
    }

    public class ImportStatisticDto
    {
        public decimal TotalImport { get; set; }
        public List<decimal> ImportMonthly { get; set; }
    }

    public class ExportStatisticDto
    {
        public decimal TotalExport { get; set; }
        public List<decimal> ExportMonthly { get; set; }
    }

    public class UserInfoDto
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Count { get; set; }
    }

}
