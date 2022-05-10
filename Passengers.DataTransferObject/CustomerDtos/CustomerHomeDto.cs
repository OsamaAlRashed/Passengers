using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.CustomerDtos
{
    public class CustomerHomeDto
    {
        public List<ShopDto> NewShops { get; set; }
        public List<ProductDto> NewProducts { get; set; }
        public List<ProductDto> SuggestionProducts { get; set; }
        public List<ProductDto> PopularProducts { get; set; }
        public List<ProductDto> TopProducts { get; set; }
    }

    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public bool Avilable { get; set; }
        public double Rate { get; set; }
        public decimal Price { get; set; }
        public Guid ShopId { get; set; }
        public string ShopImagePath { get; set; }
        public bool ShopOnline { get; set; }
    }

    public class ShopDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public bool Online { get; set; }
    }

}
