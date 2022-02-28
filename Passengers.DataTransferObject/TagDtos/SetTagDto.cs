using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.TagDtos
{
    public class SetTagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IFormFile LogoFile { get; set; }
        public Guid? ShopId { get; set; }
    }
}
