using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class ResponseCardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public List<ProductCardDto> Products { get; set; }
    }

    public class ProductCardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }

}
