using Passengers.Models.Main;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderSet = Passengers.Models.Order.Order;

namespace Passengers.Shared.SharedService
{
    public static class SubQueryExtensionMethods
    {
        public static decimal Price(this Product product)
                => product.PriceLogs
                            .Where(x => x.DateCreated <= DateTime.Now && (!x.EndDate.HasValue || DateTime.Now <= x.EndDate))
                            .Select(x => x.Price)
                            .FirstOrDefault();

        public static string ImagePath(this Product product) => product.Documents.Select(x => x.Path).FirstOrDefault();

        public static string ImagePath(this AppUser shop) => shop.Documents.Select(x => x.Path).FirstOrDefault();

        public static double Rate(this Product product) => product.Reviews.Any() ? product.Reviews.Average(x => x.Rate) : 0;

        public static int Buyers(this Product product) => product.OrderDetails.Sum(x => x.Quantity);

        public static OrderStatus Status(this OrderSet order)
                => order.OrderStatusLogs.OrderByDescending(x => x.DateCreated).Select(x => x.Status).FirstOrDefault();

        public static bool IsNotRefused(this OrderSet order, Guid driverId)
                => !order.OrderDrivers.Where(x => x.DriverId == driverId && x.OrderDriverType == OrderDriverType.Refused).Any();

        public static DeliveryCompanyOrderStatus CompanyStatus(this OrderSet order)
                => order.Status().MapCompany();

        public static CustomerOrderStatus CustomerStatus(this OrderSet order)
                => order.Status().MapCustomer();

        public static DriverOrderStatus DriverStatus(this OrderSet order)
                => order.Status().MapDriver();

        public static AppUser Shop(this OrderSet order)
            => order.OrderDetails.Select(x => x.Product.Tag.Shop).FirstOrDefault();

        public static decimal Cost(this OrderSet order)
            => order.OrderDetails.Sum(x => x.Product.Price() * x.Quantity);

        public static decimal TotalCost(this OrderSet order)
            => order.Cost() + (order.DeliveryCost ?? 0);

        public static bool Avilable(this AppUser driver)
            => driver.DriverOnline.HasValue && driver.DriverOnline.Value
            && driver.DriverOrders.Where(x => x.Status() == OrderStatus.Assigned || x.Status() <= OrderStatus.Collected).Count() == 0;

        public static decimal FixedAmount(this AppUser driver, DateTime? date = default)
            => driver.Payments
            .Where(x => !date.HasValue || x.Date == date)
            .Where(payment => payment.Type.IsFixed())
            .Sum(payment => payment.Amount * payment.Type.PaymentSign());

        public static decimal DeliveryAmount(this AppUser driver, DateTime? date = default)
            => driver.Payments
            .Where(x => !date.HasValue || x.Date == date)
            .Where(payment => payment.Type.IsFixed())
            .Sum(payment => payment.Amount * payment.Type.PaymentSign());

        public static bool IsNotRefused(this AppUser driver, Guid orderId)
                => !driver.OrderDrivers.Where(x => x.OrderId == orderId && x.OrderDriverType == OrderDriverType.Refused).Any();

        public static (double, TimeType) GetTime(this OrderSet order)
        {
            int[] duration = new[] { 60, 60, 24, 7, 30 };

            var total = DateTime.Now.Subtract(order.OrderStatusLogs.OrderBy(x => x.DateCreated).Select(x => x.DateCreated).LastOrDefault()).TotalSeconds;
            TimeType timeType = TimeType.Second;
            while(total >= duration[(int)timeType] && timeType < TimeType.Month)
            {
                total /= duration[(int)timeType];
                timeType += 1;
            }

            return (Math.Round(total, 0), timeType);
        }

    }
}
