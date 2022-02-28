using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.ProductDtos
{
    public class SetProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PrepareTime { get; set; }
        public bool Avilable { get; set; }
        public decimal Price { get; set; }
        public IFormFile ImageFile { get; set; }
        public Guid TagId { get; set; }
    }
}
