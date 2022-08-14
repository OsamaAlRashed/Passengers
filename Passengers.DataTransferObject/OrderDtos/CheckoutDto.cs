using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class CheckoutDto
    {
        public decimal Total { get; set; }
        public int Time { get; set; }
        public decimal DeliverCost { get; set; }
        public List<ShopProductDto> Items { get; set; }
    }

    public class ShopProductDto
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; }
        public decimal ShopSubTotal { get; set; }

        public List<ProductCountDto> Products { get; set; }
    }
    public class ProductCountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public int Count { get; set; }
    }
}
