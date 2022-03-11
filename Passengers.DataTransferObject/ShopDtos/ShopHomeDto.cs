using Passengers.DataTransferObject.OfferDtos;
using Passengers.DataTransferObject.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.ShopDtos
{
    public class ShopHomeDto
    {
        public List<ItemDto> TopOffers { get; set; }
        public List<ItemDto> TopProducts { get; set; }
        public List<ItemDto> PopularProducts { get; set; }
    }

    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public double Rate { get; set; }
        public int Buyers { get; set; }
    }
}
