using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class RequestCardDto
    {
        public List<ProductCountDto> Products { get; set; }
        public List<ShopNoteDto> Shops { get; set; }
    }

    public class ProductCountDto
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
    }

    public class ShopNoteDto
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
    }
}
